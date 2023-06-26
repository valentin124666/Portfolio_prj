using System;
using System.Globalization;
using Core;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using Gladiators.Sound;
using Managers.Controller;
using Managers.Interfaces;
using Settings;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements.Popup
{
    public class ArenaPopupWinView : MonoBehaviour, IUIPopup
    {
        [SerializeField] private TMP_Text _targetXPText;
        [SerializeField] private TMP_Text _currentXPText;
        [SerializeField] private TMP_Text _levelXPText;
        [SerializeField] private TMP_Text _coinText;

        [SerializeField] private Button _buttonYes;
        [SerializeField] private Transform _media;
        [SerializeField] private BounceUIElements _bounceLevel;
        [SerializeField] private SoundCustom _soundCustom;
        [SerializeField] private ImageAlpha _imageAlphaBottom;
        
        [SerializeField] private Transform[] _pointAniamtion;

        private NPCController _npcController;
        private PlayerController _playerController;
        private PlayerUpgrade _playerUpgradeData;

        private int _currentXP;
        private int _addedXp;
        private int _addedCoin;
        public bool IsActive => gameObject.activeSelf;

        public void Show()
        {
            if (!_soundCustom.IsInit)
                _soundCustom.Init();

            gameObject.SetActive(true);
            _imageAlphaBottom.Hide(true);


            var gameData = GameClient.Instance.GetService<IGameplayManager>();
            _npcController = gameData.GetController<NPCController>();
            _playerController = gameData.GetController<PlayerController>();
            _playerUpgradeData = GameClient.Get<IGameDataManager>().GetDataScriptable<GameData>().GetPlayerUpgrade(_playerController.FameLevel);
            
            _addedCoin = _npcController.FightMoney;
            _currentXP = _playerController.CurrentXP;
            _addedXp = _npcController.FightExp;
            
            AnimationShowPopup().OnComplete(() => Activation().Forget());
        }

        public void Show(Action callback)
        {
            Show();
            callback?.Invoke();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Reset();
        }

        public void Reset()
        {
            _addedCoin = default;
            _currentXP = default;
            _addedXp = default;

            _buttonYes.onClick.RemoveAllListeners();
        }

        private async UniTask Activation()
        {
            await AnimationsAddedCoin();
            await AnimationsAddedXp();
            _imageAlphaBottom.Show(false);
            _buttonYes.onClick.AddListener(() => GameClient.Instance.GetService<IGameplayManager>().ChangeAppState(Enumerators.AppState.InGameSquare));
        }

        private async UniTask AnimationsAddedXp()
        {
            await TaskManager.WaitUntilDelay(0.3f);

            while (_addedXp > 0)
            {
                _currentXP++;
                _addedXp--;
                _currentXPText.text = _currentXP.ToString();

                await TaskManager.WaitUntilDelay(0.1f);
                
                if (_currentXP != _playerUpgradeData.NumberNextLevel) continue;
                
                _currentXP = 0;
                _playerUpgradeData = GameClient.Get<IGameDataManager>().GetDataScriptable<GameData>().GetPlayerUpgrade(_playerUpgradeData.Level + 1);

                var completed = false;
                _bounceLevel.Bounce().OnComplete(() => completed = true);

                await TaskManager.WaitUntilDelay(0.1f);

                _currentXPText.text = _currentXP.ToString();
                _targetXPText.text = _playerUpgradeData.NumberNextLevel.ToString();
                _levelXPText.text = _playerUpgradeData.Level.ToString();

                await TaskManager.WaitUntil(() => completed);
            }
        }

        private async UniTask AnimationsAddedCoin()
        {
            await TaskManager.WaitUntilDelay(0.3f);
            
            const float timePause = 0.001f;
            var currentCoin = 0f;
            var added = _addedCoin / 60f;
            var addedCoin = (float)_addedCoin;
            var seq = DOTween.Sequence();

            seq.AppendCallback(() => { _soundCustom.Play(); });
            seq.AppendInterval(0.1f);
            seq.SetLoops(-1);

            while (addedCoin > 0)
            {
                currentCoin += added;
                addedCoin -= added;
                _coinText.text = MathF.Round(currentCoin).ToString(CultureInfo.InvariantCulture);

                await TaskManager.WaitUntilDelay(timePause);
            }
            seq.Kill();
            _coinText.text = _addedCoin.ToString();

            _addedCoin = 0;
        }

        private Sequence AnimationShowPopup()
        {
            _coinText.text = "0";

            _targetXPText.text = _playerUpgradeData.NumberNextLevel.ToString();
            _currentXPText.text = _currentXP.ToString();
            _levelXPText.text = _playerUpgradeData.Level.ToString();

            var seq = DOTween.Sequence();
            _media.position = _pointAniamtion[0].position;
            
            const float duration = 0.5f;
            seq.Append(_media.DOMove(_pointAniamtion[1].position, duration / 2));
            seq.Append(_media.DOMove(_pointAniamtion[2].position, duration / 4));
            seq.Append(_media.DOShakePosition(duration / 4, new Vector3(0, 10, 0)));

            return seq;
        }
    }
}