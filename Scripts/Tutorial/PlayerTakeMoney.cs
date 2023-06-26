using System;
using Level.Interactive;
using Managers.Controller;
using Receptacle;
using Settings;
using Tools;
using UnityEngine;

namespace Tutorial
{
    public class PlayerTakeMoney : MonoBehaviour
    {
        private bool _isInit;
        private bool _isCoin;
        private TutorialController _tutorialController;
        private StorekeeperView _storekeeperView;

        public void Init(TutorialController tutorialController, StorekeeperView storekeeperView)
        {
            _tutorialController = tutorialController;
            _storekeeperView = storekeeperView;
            _isInit = true;
        }

        private void FixedUpdate()
        {
            if (!_isInit || _isCoin) return;

            if (_storekeeperView.GetAvailableNumberObjects() > 0)
            {
                _isCoin = true;
                _tutorialController.SetActiveArrow(true);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isInit || !_isCoin) return;

            var storekeeper = other.GetComponent<StorekeeperView>();
            
            if (storekeeper == null) return;
            
            var receptacleObjectType = ((ReceptacleUnidirectional)storekeeper.GetReceptacle).ReceptacleObjectType;
            
            if (storekeeper.StorekeeperType == Enumerators.StorekeeperType.ToGive && receptacleObjectType == Enumerators.ReceptacleObjectType.Coin)
            {
                _isInit = false;
                TaskManager.ExecuteAfterDelay(0.5f, () =>
                {
                    _tutorialController.ShowColosseum();
                    Destroy(this);
                });
            }
        }
    }
}