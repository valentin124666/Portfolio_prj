using Level.Interactive;
using Managers.Controller;
using Receptacle;
using Settings;
using UnityEngine;

namespace Tutorial
{
    public class PlayerStock : MonoBehaviour
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
            if (storekeeper != null)
            {
                var receptacleObjectType = ((ReceptacleUnidirectional)storekeeper.GetReceptacle).ReceptacleObjectType;
                if (storekeeper.StorekeeperType == Enumerators.StorekeeperType.ToGive&& receptacleObjectType==Enumerators.ReceptacleObjectType.Material)
                {
                    _tutorialController.DropMaterial();
                    Destroy(this);
                }
            }
        }
    }
}
