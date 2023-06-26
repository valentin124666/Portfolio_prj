using Level.Interactive;
using Managers.Controller;
using UnityEngine;

namespace Tutorial
{
    public class PlayerPurchase : MonoBehaviour
    {
        private bool _isInit;
        private TutorialController _tutorialController;
        private WorkbenchPurchaseView _workbenchPurchase;

        public void Init(TutorialController tutorialController,WorkbenchPurchaseView purchaseView)
        {
            _tutorialController = tutorialController;
            _workbenchPurchase = purchaseView;
            _isInit = true;
        }

        private void FixedUpdate()
        {
            if(!_isInit) return;
            if (!_workbenchPurchase.gameObject.activeSelf)
            {
                Destroy(this);
                _tutorialController.ShowStock();
            }
            
        }
    }
}
