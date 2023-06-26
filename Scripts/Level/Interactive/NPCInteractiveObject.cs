using System.Collections.Generic;
using DG.Tweening;
using General;
using Settings;
using UnityEngine;

namespace Level.Interactive
{
    public class NPCInteractiveObject : MonoBehaviour
    {
        [SerializeField] private Enumerators.InteractiveObjectName _interactiveName;
        public Enumerators.InteractiveObjectName InteractiveName => _interactiveName;

        [SerializeField] private StorekeeperView _storekeeperRack;
        [SerializeField] private StorekeeperView _storekeeperTable;

        [SerializeField] private List<PointsInterest> _pointsInterest;

        public bool FreePlaceCount => _pointsInterest.Count > 0 && gameObject.activeSelf;

        public void Init()
        {
            foreach (var item in _pointsInterest)
            {
                item.Init();
            }

            _storekeeperRack.Init();
            _storekeeperTable.Init();
        }
        
        public Transform GetPlaceForMoney() => _storekeeperTable.GetReceptacle.GetWarehousePlace();
        public StorekeeperView GetTable() => _storekeeperTable;
        public PointInterest GetFreePlaceOrNull() => _pointsInterest[0].GetFreePlace();
        public bool ThereAreGoods() => _storekeeperRack.GetAvailableNumberObjects() > 0 && _storekeeperRack.ThereAreObjects();

        public ProductReceptacleObject GetProduct()
        {
            _storekeeperRack.GiveReceptacleObject(1);
            return (ProductReceptacleObject)_storekeeperRack.GetReceptacle.GetReceptacleObject(_storekeeperRack.GetTypeReceptacle());
        }

        public void TakeCoin(ReceptacleObject coin)
        {
            _storekeeperTable.TakeReceptacleObject(1);
            _storekeeperTable.GetReceptacle.SetReceptacleObject(coin);
        }

        public void SetActive(bool isActive, bool isAnimation = false)
        {
            gameObject.SetActive(isActive);

            if (isAnimation)
            {
                transform.DOShakeScale(0.3f, 0.5f, vibrato: 10,
                    randomnessMode: ShakeRandomnessMode.Harmonic).SetEase(Ease.OutBack);
            }
        }
    }
}