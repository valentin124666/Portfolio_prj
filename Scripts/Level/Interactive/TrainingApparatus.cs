using System;
using Core;
using Gladiators.Player;
using Managers.Controller;
using Managers.Interfaces;
using Settings;
using UIElements.Popup;
using UnityEngine;

namespace Level.Interactive
{
    public class TrainingApparatus : InteractiveObject
    {
        [SerializeField] private Timer _timer;

        private LevelController _levelController;
        private PlayerController _playerController;

        private IUIManager _uiManager;
        private GymPopupView _gymPopup;
        private InteractiveMiniGamePopup _interactiveMiniGamePopup;
        private Action _callbackInteractive;
        [SerializeField] private GameObject _icon;
        [SerializeField] private GameObject _model;
        [SerializeField] private GameObject[] _disabledDevices;
        [SerializeField] private BenchAnimatorCustom _animatorCustom;
        [SerializeField] private ParticleSystem _particleWork;
        private Collider[] _colliders = Array.Empty<Collider>();

        public override bool AllowAccess => !_gymPopup.IsActive;
        public override bool FinishedWorking => _finishedTraining;
        private bool _finishedTraining;
        public override Enumerators.InteractiveObjectType InteractiveObjectType => Enumerators.InteractiveObjectType.Exercise;


        public override void Init()
        {
            var gameplayManager = GameClient.Get<IGameplayManager>();

            if (_model != null)
                _colliders = _model.GetComponents<Collider>();

            _playerController = GameClient.Get<IGameplayManager>().GetController<PlayerController>();
            _levelController = gameplayManager.GetController<LevelController>();

            var uiManager = GameClient.Get<IUIManager>();
            _gymPopup = uiManager.GetPopup<GymPopupView>();
            _interactiveMiniGamePopup = uiManager.GetPopup<InteractiveMiniGamePopup>();

            _timeZoneActivation = _levelController.BenchTimeZoneActivation(InteractiveName);
            _timer.ResetTimer();
        }

        private void SetActiveDisabledDevices(bool isActive)
        {
            foreach (var item in _disabledDevices)
            {
                item.SetActive(isActive);
            }
        }

        public override void Interaction()
        {
            SetActiveDisabledDevices(false);
            _finishedTraining = false;
            _gymPopup.Show();
            if (_animatorCustom != null)
            {
                _animatorCustom.ResetAnimation();
            }

            _gymPopup.AddListenerButtonYes(() =>
            {
                _timer.SetActive(true);
                _timer.StartTimer(() =>
                {
                    _finishedTraining = true;
                    _playerController.GetPresenter<PlayerGladiatorPresenter>().AddingCharacteristicsEffect();
                });
                MainApp.Instance.LockTouch(true);
                _icon.SetActive(false);
                _callbackInteractive?.Invoke();
                if (_animatorCustom != null)
                {
                    _animatorCustom.StartWork();
                }

                _particleWork.Play();
            });

            _gymPopup.AddListenerButtonNo(() => { _finishedTraining = true; });

            _gymPopup.SetTypeSimulator(InteractiveName);
        }

        public void SetCallbackInteractive(Action callback)
        {
            _callbackInteractive = callback;
        }

        public override void EndInteraction()
        {
            SetActiveDisabledDevices(true);
            _icon.SetActive(true);
            _playerController.DisplayCharacteristics(true);
            if (_animatorCustom != null)
            {
                _animatorCustom.EndWork();
            }

            _particleWork.Stop();

            base.EndInteraction();
            MainApp.Instance.LockTouch(false);

            _interactiveMiniGamePopup.Hide();
            _timer.SetActive(false);
            _finishedTraining = false;
        }
    }
}