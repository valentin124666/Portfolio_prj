using Core;
using Managers.Controller;
using Managers.Interfaces;
using Settings;
using UIElements.Popup;
using UnityEngine;

namespace Tutorial
{
    public class PlayerShowColosseum : MonoBehaviour
    {
        private bool _isInit;
        private bool _isArena;
        private TutorialController _tutorialController;
        private ScreenArenaPopupView _screenArenaPopup;
        private IGameplayManager _gameplayManager;

        public void Init(TutorialController tutorialController)
        {
            _tutorialController = tutorialController;
            _screenArenaPopup = GameClient.Get<IUIManager>().GetPopup<ScreenArenaPopupView>();
            _gameplayManager = GameClient.Get<IGameplayManager>();
            MainApp.Instance.FixedUpdateEvent += FixedUpdateCustom;
            _isInit = true;
        }

        private void FixedUpdateCustom()
        {
            if (!_isInit) return;

            if (!_isArena && _gameplayManager.CurrentState == Enumerators.AppState.InGameArena)
            {
                _isArena = true;
                _tutorialController.SetActiveArrow(false);
            }

            if (_isArena && _gameplayManager.CurrentState == Enumerators.AppState.InGameSquare)
            {
                if (!_screenArenaPopup._isActive)
                {
                    _tutorialController.GreatLoss();
                    MainApp.Instance.FixedUpdateEvent -= FixedUpdateCustom;
                    Destroy(this);
                }
            }
        }
    }
}