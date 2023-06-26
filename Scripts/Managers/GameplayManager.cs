using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Interfaces;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using Level;
using Managers.Controller;
using Managers.Interfaces;
using Settings;
using Tools;
using UIElements.Pages;
using UIElements.Popup;
using UnityEngine;

namespace Managers
{
    public class GameplayManager : IService, IGameplayManager
    {
        private IUIManager _uIManager;
        private List<IController> _controllers;
        public Enumerators.AppState CurrentState { get; private set; }
        public bool IsPause { get; private set; }

        private GameData _gameData;

        public async UniTask Init()
        {
            _uIManager = GameClient.Get<IUIManager>();

            _gameData = GameClient.Get<IGameDataManager>().GetDataScriptable<GameData>();

            await FillControllers();
        }
        
        public T GetController<T>() where T : IController
        {
            return (T)_controllers.Find(controller => controller is T);
        }

        private bool ControllerReadiness()
        {
            return _controllers.All(t => t.IsInit);
        }

        private async UniTask FillControllers()
        {
            _controllers = new List<IController>()
            {
                new TutorialController(),
                new PoolController(),
                new LevelController(),
                new PlayerController(),
                new NPCController(),
                new CameraController(),
                new JoystickController(),
                new AnimatorController(),
                new SoundController(),
                new BattleController(),
            };

            foreach (var item in _controllers)
                item.Init();

            await TaskManager.WaitUntil(ControllerReadiness);
            GetController<TutorialController>().CheckTutorial();
        }

        public void StartGameplay()
        {
            IsPause = false;
        }

        public void ChangeAppState(Enumerators.AppState stateTo)
        {
            Sequence seq;
            switch (stateTo)
            {
                case Enumerators.AppState.AppStart:
                    _uIManager.HideAllPopups();
                    break;
                case Enumerators.AppState.InGameSquare:
                    seq = CurrentState==Enumerators.AppState.Unknown ? DOTween.Sequence() : MainApp.Instance.ActionsFoldingScreen(true, addedTime: _gameData.DelayBeforeTransitionLevels);

                    
                    if (CurrentState == Enumerators.AppState.InGameArena)
                        _uIManager.GetPopup<ScreenArenaPopupView>().Show();

                    seq.OnComplete(() =>
                    {
                        MainApp.Instance.DirectionalLight.eulerAngles = new Vector3(50, 227,0);

                        _uIManager.HideAllPopups();
                        _uIManager.SetPage<InGameSquarePagePresenter>();

                        GetController<BattleController>().EndBattle();
                        GetController<LevelController>().ActivationLevel<SquareLevelPresent>();
                        GetController<PlayerController>().ActivationGladiator(stateTo);
                        var cam = GetController<CameraController>();

                        cam.ActivationCameraTrack();
                        cam.SetFieldOfView(_gameData.cameraData.CameraFileOfViewSquare);

                        GetController<NPCController>().SetActionPassant(true);
                        GetController<NPCController>().SetActionFighter(false);
                        MainApp.Instance.ActionsFoldingScreen(false);
                    });
                    break;
                case Enumerators.AppState.InGameArena:
                    seq = MainApp.Instance.ActionsFoldingScreen(true, addedTime: _gameData.DelayBeforeTransitionLevels);
                    _uIManager.GetPopup<ScreenArenaPopupView>().Show();

                    seq.OnComplete(() =>
                    {
                        MainApp.Instance.DirectionalLight.eulerAngles = new Vector3(36, 367,0);

                        _uIManager.HideAllPopups();
                        _uIManager.SetPage<InGameArenaPagePresenter>();

                        GetController<BattleController>().StartBattle();
                        GetController<LevelController>().ActivationLevel<ArenaColiseumPresenter>();
                        GetController<PlayerController>().ActivationGladiator(stateTo);

                        var cam = GetController<CameraController>();

                        cam.ActivationCameraTrack(true);
                        cam.SetFieldOfView(_gameData.cameraData.CameraFileOfViewArena);

                        GetController<NPCController>().SetActionPassant(false);
                        GetController<NPCController>().SetActionFighter(true);
                        MainApp.Instance.ActionsFoldingScreen(false);
                    });

                    break;
            }

            CurrentState = stateTo;
        }
    }
}