using System;
using Gladiators.Player;
using Managers.Controller;
using Settings;
using UnityEngine;

namespace Tutorial
{
    public class PlayerMachineWork : MonoBehaviour
    {
        private bool _isInit;
        private bool _isInteraction;
        private TutorialController _tutorialController;
        private PlayerController _playerController;
        private PlayerGladiatorPresenter _playerGladiatorPresenter;

        public void Init(TutorialController tutorialController, PlayerController playerController, PlayerGladiatorPresenter playerGladiatorPresenter)
        {
            _tutorialController = tutorialController;
            _playerController = playerController;
            _playerGladiatorPresenter = playerGladiatorPresenter;
            _isInit = true;
        }

        private void FixedUpdate()
        {
            if (!_isInit) return;

            if (_playerGladiatorPresenter.CurrentState == Enumerators.PlayerStates.Interaction && !_isInteraction)
            {
                _tutorialController.SetActiveArrow(false);
                _isInteraction = true;
            }

            if (_isInteraction)
            {
                if (_playerGladiatorPresenter.CurrentState != Enumerators.PlayerStates.Interaction && _playerController.IdModelSword != 0)
                {
                    _tutorialController.TakeSwords();
                    _isInteraction = false;

                    Destroy(this);
                }
                else if (_playerGladiatorPresenter.CurrentState != Enumerators.PlayerStates.Interaction && _playerController.IdModelSword == 0)
                {
                    _isInteraction = false;

                    _tutorialController.SetActiveArrow(true);
                }
            }
        }
    }
}