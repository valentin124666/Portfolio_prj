using System;
using System.Collections.Generic;
using Core;
using General;
using Managers.Controller;
using Managers.Interfaces;
using Settings;
using UnityEngine;

namespace Receptacle
{
    [Serializable]
    public abstract class ReceptacleSimple
    {
        [SerializeField] protected int _count;

        [SerializeField] protected Transform _receptacle;
        public Transform receptacle => _receptacle;

        protected List<ReceptacleObject> _storageObject = new();
        [SerializeField] protected Transform[] _warehousePlace;
        protected PoolController _poolController;
        protected int _warehousePlaceItem;

        public bool ThereAreObjects => _storageObject.Count > 0;
        public virtual void Init()
        {
            _poolController = GameClient.Get<IGameplayManager>().GetController<PoolController>();
        }
        

        public Transform GetWarehousePlace()
        {
            var obj = _warehousePlace[_warehousePlaceItem];

            SetNumberWarehousePlaceItem(true);
            return obj;
        }

        protected void SetNumberWarehousePlaceItem(bool add)
        {
            if (add)
            {
                if (_warehousePlaceItem < _warehousePlace.Length - 1)
                    _warehousePlaceItem++;
                else
                    _warehousePlaceItem = 0;
            }
            else
            {
                if (0 < _warehousePlaceItem)
                    _warehousePlaceItem--;
                else
                    _warehousePlaceItem = _warehousePlace.Length - 1;
            }
        }

        public abstract ReceptacleObject GetReceptacleObject(Enumerators.ReceptacleObjectType type);

        public void SetReceptacleObject(ReceptacleObject receptacleObject)
        {
            _storageObject.Add(receptacleObject);
        }

        public int GetIdMod(int id)
        {
            if (id > _storageObject.Count - 1) return 0;
            return _storageObject[id].GetCurrentIdMod();
        }

        public void SetReceptacleSize(int count)
        {
            _count = count;
        }
    }

    [Serializable]
    public class ReceptacleUnidirectional : ReceptacleSimple
    {
        public Enumerators.ReceptacleObjectType ReceptacleObjectType;

        /// <summary>
        /// -1 is infinity.
        /// </summary>
        public int InStock;

        public float FreePlace => (_count < 0 ? float.PositiveInfinity : _count - InStock);

        public void ToTakeReceptacleObject(int countObject)
        {
            if (InStock == -1) return;

            if (countObject <= FreePlace)
            {
                InStock += countObject;
            }
            else
            {
                Debug.LogError("Little storage space");
            }
        }

        public void ToGiveReceptacleObject(int countObject)
        {
            if (InStock == -1) return;
            if (countObject <= InStock)
            {
                InStock -= countObject;
            }
            else
            {
                Debug.LogError("Not enough objects");
            }
        }

        public override ReceptacleObject GetReceptacleObject(Enumerators.ReceptacleObjectType type)
        {
            if (type != ReceptacleObjectType)
            {
                Debug.LogError("Request for an object of the wrong type");
                throw new System.NullReferenceException();
            }

            if (_storageObject.Count > 0)
            {
                var receptacleObject = _storageObject[^1];
                _storageObject.Remove(receptacleObject);
                SetNumberWarehousePlaceItem(false);

                return receptacleObject;
            }
            var obj = _poolController.GetPoolObj<ReceptacleObject>((Enumerators.NamePrefabAddressable)ReceptacleObjectType);

            obj.Activation();
            obj.transform.SetParent(_receptacle);
            obj.transform.SetPositionAndRotation(_warehousePlace[0].position, _warehousePlace[0].rotation);

            return obj;
        }
    }

    [Serializable]
    public class ReceptacleUniversal : ReceptacleSimple
    {
        [NonSerialized] public List<Enumerators.ReceptacleObjectType> ReceptacleObjectType = new List<Enumerators.ReceptacleObjectType>();

        public int InStock => ReceptacleObjectType.Count;
        public float FreePlace => _count - InStock;

        public int InStockSpecific(Enumerators.ReceptacleObjectType type)
            => ReceptacleObjectType.FindAll((item) => item == type).Count;

        public void ToTakeReceptacleObject(int countObject, Enumerators.ReceptacleObjectType type)
        {
            if (countObject <= FreePlace)
            {
                for (int i = 0; i < countObject; i++)
                {
                    ReceptacleObjectType.Add(type);
                }
            }
            else
            {
                Debug.LogError("Little storage space");
            }
        }

        public void ToGiveReceptacleObject(int countObject, Enumerators.ReceptacleObjectType type)
        {
            if (countObject <= InStockSpecific(type))
            {
                for (int i = 0; i < countObject; i++)
                {
                    ReceptacleObjectType.Remove(type);
                }
            }
            else
            {
                Debug.LogError("Little storage space");
            }
        }

        public override ReceptacleObject GetReceptacleObject(Enumerators.ReceptacleObjectType type)
        {
            _storageObject.Reverse();
            var receptacleObject = _storageObject.Find((item) => item.ReceptacleObjectType == type);

            if (receptacleObject == null)
            {
                Debug.LogError("Request for an object of the wrong type");
                return null;
            }

            SetNumberWarehousePlaceItem(false);

            _storageObject.Remove(receptacleObject);
            _storageObject.Reverse();

            return receptacleObject;
        }
    }
}