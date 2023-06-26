using System;
using Level.Interactive;
using Managers.Controller;
using Receptacle;
using Settings;
using UnityEngine;

namespace Tutorial
{
    public class PlayerBench : MonoBehaviour
    {
        private bool _isInit;
        private TutorialController _tutorialController;
        
        public void Init(TutorialController tutorialController)
        {
            _tutorialController = tutorialController;
            _isInit = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!_isInit) return;
            
            var storekeeper = other.GetComponent<StorekeeperView>();
            
            if (storekeeper == null) return;
            
            var receptacleObjectType = ((ReceptacleUnidirectional)storekeeper.GetReceptacle).ReceptacleObjectType;
            
            if (storekeeper.StorekeeperType != Enumerators.StorekeeperType.ToTake 
                || receptacleObjectType != Enumerators.ReceptacleObjectType.Material) return;
            
            _tutorialController.MachineWork();
            Destroy(this);
        }
    }
}
