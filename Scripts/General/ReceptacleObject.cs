using Core;
using Settings;
using Tools;
using UnityEngine;

namespace General
{
    public class ReceptacleObject : PoolItem
    {
        public Enumerators.ReceptacleObjectType ReceptacleObjectType => (Enumerators.ReceptacleObjectType)NamePrefab;


        public ReceptacleObject(Enumerators.NamePrefabAddressable name, GameObject gameObject) : base(name, gameObject)
        {
           
        }
        public virtual int GetCurrentIdMod() => 0;

    }
}