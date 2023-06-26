using System;
using Managers.Controller;
using UnityEngine;

namespace Tutorial
{
    public class PlayerEarnMoney : MonoBehaviour
    {
        private bool _isInit;
        private TutorialController _tutorialController;
        private PlayerController _playerController;

        public void Init(TutorialController tutorialController, PlayerController playerController)
        {
            _tutorialController = tutorialController;
            _playerController = playerController;
            _isInit = true;
        }

        private void FixedUpdate()
        {
            if(!_isInit) return;

            if (_playerController.CurrentCoin >= 25)
            {
                _tutorialController.ShowColosseum();
                Destroy(this);
            }
        }
    }
}
