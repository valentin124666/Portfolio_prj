using Core;
using Managers.Controller;
using Managers.Interfaces;
using Settings;
using TMPro;
using UnityEngine;

namespace Level.Interactive
{
    public class WorkbenchPurchaseView : InteractiveObject
    {
        public override bool AllowAccess => gameObject.activeSelf;
        public override Enumerators.InteractiveObjectType InteractiveObjectType => Enumerators.InteractiveObjectType.ToBuy;

        [SerializeField] private TMP_Text _costText;

        private LevelController _levelController;
        
        public int Cost;

        public override void Init()
        {
            _timeZoneActivation = 1;
            _levelController = GameClient.Instance.GetService<IGameplayManager>().GetController<LevelController>();
            _costText.text = Cost.ToString();
        }

        public override void Interaction()
        {
            gameObject.SetActive(false);

            _levelController.ActivationPurchaseInSquare(this);
        }
    }
}