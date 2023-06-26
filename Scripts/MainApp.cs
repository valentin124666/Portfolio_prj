using System;
using Core;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using Managers.Interfaces;
using Settings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainApp : MonoBehaviour
{
    public event Action LateUpdateEvent;
    public event Action FixedUpdateEvent;
    public event Action UpdateEvent;

    private static MainApp _Instance;

    public static MainApp Instance
    {
        get => _Instance;
        private set => _Instance = value;
    }

    [SerializeField] private Image _foldingScreen;
    [SerializeField] private GameObject _lockTouch;
    [SerializeField] private GameObject _startScreen;
    
    public bool IsLockTouch => _lockTouch.activeSelf;

    [SerializeField] private GameObject _canvas;
    [SerializeField] private Transform _directionalLight;
    public GameObject Canvas => _canvas;
    public Transform DirectionalLight => _directionalLight;

    private GameObject _eventSystem;

    public float FoldingScreenSpeed { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        ActionsFoldingScreen(true, true);

        _eventSystem = EventSystem.current.gameObject;

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        if (Instance != this) return;

        ResourceLoading().Forget();
    }

    private async UniTask ResourceLoading()
    {
        await ResourceLoader.Init();

        await GameClient.Instance.InitServices();
        FoldingScreenSpeed = GameClient.Instance.GetService<IGameDataManager>().GetDataScriptable<GameData>().FoldingScreenSpeed;

        _startScreen.SetActive(false);
        GameClient.Get<IGameplayManager>().ChangeAppState(Enumerators.AppState.InGameSquare);
    }

    private void Update()
    {
        if (Instance == this)
        {
            UpdateEvent?.Invoke();
        }
    }

    public Sequence ActionsFoldingScreen(bool isAction, bool isFast = false, float addedTime = 0)
    {
        _foldingScreen.gameObject.SetActive(true);
        var alpha = isAction ? 1 : 0;
        var color = new Color(0, 0, 0, alpha);

        var seq = DOTween.Sequence();

        if (isFast)
        {
            _foldingScreen.color = color;
        }
        else
        {
            _foldingScreen.DOKill();

            seq.Append(_foldingScreen.DOColor(color, FoldingScreenSpeed));
            seq.InsertCallback(addedTime, () => { _foldingScreen.gameObject.SetActive(isAction); });
        }


        return seq;
    }

    public void LockTouch(bool isActive)
    {
        _lockTouch.SetActive(isActive);
    }

    public void LockClick(bool isActive)
    {
        _eventSystem.SetActive(isActive);
    }

    private void LateUpdate()
    {
        if (Instance == this)
        {
            LateUpdateEvent?.Invoke();
        }
    }

    private void FixedUpdate()
    {
        if (Instance == this)
        {
            FixedUpdateEvent?.Invoke();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            GameClient.Instance.Dispose();
        }
    }

    private void OnApplicationQuit()
    {
        if (Instance == this)
        {
            GameClient.Instance.GetService<IGameDataManager>().SaveDataClients();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (Instance != this) return;

        if (Application.platform == RuntimePlatform.Android)
            GameClient.Instance.GetService<IGameDataManager>().SaveDataClients();
    }
}