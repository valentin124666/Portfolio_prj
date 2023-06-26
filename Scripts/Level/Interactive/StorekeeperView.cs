using System;
using Core;
using Data;
using Data.Characteristics;
using DG.Tweening;
using General;
using Managers.Controller;
using Managers.Interfaces;
using Receptacle;
using Settings;
using UnityEngine;

namespace Level.Interactive
{
    public class StorekeeperView : MonoBehaviour
    {
        [SerializeField] private string id;
        [SerializeField] private ReceptacleUnidirectional _receptacle;
        [SerializeField] private Enumerators.StorekeeperType _storekeeperType;

        private LevelController _levelController;
        private PoolController _poolController;

        public Enumerators.StorekeeperType StorekeeperType => _storekeeperType;
        public Enumerators.ReceptacleObjectType GetTypeReceptacle() => _receptacle.ReceptacleObjectType;

        public float GetAvailableNumberObjects() => _receptacle.InStock < 0 ? float.PositiveInfinity : _receptacle.InStock;
        public float GetAvailableReceptacleSpace() => _receptacle.FreePlace;
        public bool ThereAreObjects() => _receptacle.ThereAreObjects;

        public void GiveReceptacleObject(int numberRequestedObjects) => _receptacle.ToGiveReceptacleObject(numberRequestedObjects);

        public void TakeReceptacleObject(int numberRequestedObjects) => _receptacle.ToTakeReceptacleObject(numberRequestedObjects);

        public ReceptacleSimple GetReceptacle => _receptacle;

        public void Init(int receptacleSize = 0)
        {
            var gameManager = GameClient.Get<IGameplayManager>();
            _levelController = gameManager.GetController<LevelController>();
            _poolController = gameManager.GetController<PoolController>();

            _receptacle.Init();

            if (receptacleSize > 0)
                _receptacle.SetReceptacleSize(receptacleSize);

            var data = _levelController.ReceptacleRegistration(this);

            ProductCharacteristicsData productCharacteristics = null;


            if (GetTypeReceptacle() == Enumerators.ReceptacleObjectType.Sword || GetTypeReceptacle() == Enumerators.ReceptacleObjectType.Shield)
                productCharacteristics = GameClient.Get<IGameDataManager>().GetDataScriptable<ProductCharacteristicsData>();

            if (data.idMod.Length > 0)
            {
                TakeReceptacleObject(data.idMod.Length);

                for (var i = 0; i < data.idMod.Length; i++)
                {
                    var product = _poolController.GetPoolObj<ReceptacleObject>((Enumerators.NamePrefabAddressable)_receptacle.ReceptacleObjectType);

                    if (productCharacteristics != null)
                        product.Activation(productCharacteristics.GetProductById(data.idMod[i], GetTypeReceptacle()));
                    else
                        product.Activation();
                    
                    product.transform.DOKill();

                    var transformTo = _receptacle.GetWarehousePlace();
                    product.transform.rotation = transformTo.rotation;
                    product.transform.position = transformTo.position;

                    product.transform.SetParent(_receptacle.receptacle);
                    _receptacle.SetReceptacleObject(product);
                }
            }
        }

        public ReceptacleDataJson GetReceptacleData()
        {
            var count = _receptacle.InStock > 0 ? _receptacle.InStock : 0;

            var mod = new int[count];
            for (int i = 0; i < mod.Length; i++)
            {
                mod[i] = _receptacle.GetIdMod(i);
            }

            return new ReceptacleDataJson
            {
                id = id,
                idMod = mod
            };
        }

        public void CreateId()
        {
            id = Guid.NewGuid().ToString();
        }

        public string GetId() => id;
    }
}