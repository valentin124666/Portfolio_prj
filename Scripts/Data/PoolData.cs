using System;
using System.Collections.Generic;
using Settings;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Custom menu/Data/PoolData")]
    public class PoolData : ScriptableObject
    {
        public List<PoolObj> PoolObjs;

        public PoolObj GetDataByName(Enumerators.NamePrefabAddressable namePrefab) => PoolObjs.Find(item => item.NamePrefab == namePrefab);
    }

    [Serializable]
    public struct PoolObj
    {
        public string TypePoolItem;
        public Enumerators.NamePrefabAddressable NamePrefab;
        public int Count;
        public string NameSection;
        public int MinimalAmount;
        public int CreateAdditional;
    }
}