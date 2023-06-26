using Gladiators.Player;
using Managers.Controller;
using UnityEngine;

namespace Tutorial
{
    public class PlayerGreatLoss : MonoBehaviour
    {
        private bool _isInit;
        private int _numberHeals;
        private TutorialController _tutorialController;
        private PlayerController _playerController;

        public void Init(TutorialController tutorialController,PlayerController playerController)
        {
            _tutorialController = tutorialController;
            _playerController = playerController;
            _numberHeals = playerController.PlayerAddedHealth;
            _isInit = true;
        }

        private void FixedUpdate()
        {
            if (!_isInit) return;
            if (_numberHeals < _playerController.PlayerAddedHealth)
            {
                _tutorialController.FinishTutorial();
            }
            
        }
    }
}