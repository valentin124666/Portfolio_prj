using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Cysharp.Threading.Tasks;
using Data;
using General;
using Managers.Interfaces;
using Settings;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Managers.Controller
{
    public class PoolController : IController
    {
        public bool IsInit { get; private set; }

        private Transform _poolParent;
        private PoolData _poolData;

        private List<Transform> _sections;

        private Dictionary<Enumerators.NamePrefabAddressable, Queue<PoolItem>> _poolItems;

        public void Init()
        {
            _sections = new List<Transform>();
            _poolData = GameClient.Instance.GetService<IGameDataManager>().GetDataScriptable<PoolData>();
            _poolParent = new GameObject().transform;
            _poolParent.name = "[Pool]";

            GetSection("ParentlessPool");

            _poolItems = new Dictionary<Enumerators.NamePrefabAddressable, Queue<PoolItem>>();

            CreatePool().Forget();
        }

        private bool CheckPoolInstantiate()
        {
            return _poolData.PoolObjs.All(item => _poolItems[item.NamePrefab].Count == item.Count);
        }

        private async UniTask CreatePool()
        {
            foreach (var item in _poolData.PoolObjs)
            {
                if (!_poolItems.ContainsKey(item.NamePrefab))
                {
                    _poolItems.Add(item.NamePrefab, new Queue<PoolItem>());
                }

                CreatePoolItemOptional(item.Count, item.NamePrefab, item.NameSection, item.TypePoolItem).Forget();
            }

            await TaskManager.WaitUntil(CheckPoolInstantiate);
            IsInit = true;
        }

        private Transform GetSection(string nameSection)
        {
            var section = _sections.Find((item) => item.name == $"[{nameSection}]");
            
            if (section != null) return section;
            
            section = new GameObject().transform;
            section.name = $"[{nameSection}]";
            section.SetParent(_poolParent);

            _sections.Add(section);

            return section;
        }

        private async UniTask CreatePoolItemOptional(int count, Enumerators.NamePrefabAddressable namePrefab, string nameSection, string type)
        {
            var obj = await ResourceLoader.GetResource<GameObject>(namePrefab.ToString());

            CreatePoolItem(count,obj,namePrefab,nameSection,type);
        }

        private void CreatePoolItem(int count, GameObject prefab, Enumerators.NamePrefabAddressable namePrefab, string nameSection, string type)
        {
            var typePool = type != "" ? Type.GetType(type, true, true) : typeof(PoolItem);

            for (var i = 0; i < count; i++)
            {
                var obj = Object.Instantiate(prefab, GetSection(nameSection));


                var poolItem = Activator.CreateInstance(typePool, namePrefab, obj);
                var item = (PoolItem)poolItem;

                if (item != null)
                    _poolItems[namePrefab].Enqueue(item);
                else
                    Debug.LogError($"[PoolController]:Type is incorrect{typePool}");
            }
        }

        public T GetPoolObj<T>(Enumerators.NamePrefabAddressable namePrefab) where T : PoolItem
        {
            if (_poolItems[namePrefab] == null)
            {
                Debug.LogError($"[PoolController]: No such pool");
                return null;
            }

            var poolItem = (T)_poolItems[namePrefab].Dequeue();

            var data = _poolData.GetDataByName(namePrefab);

            if (_poolItems[namePrefab].Count <= data.MinimalAmount)
            {
                CreatePoolItemOptional(data.CreateAdditional, data.NamePrefab, data.NameSection, data.TypePoolItem).Forget();
                Debug.LogWarning($"[PoolController]: Additional objects are created under the name{data.NamePrefab}");
            }

            if (poolItem == null)
                Debug.LogError($"[PoolController]: Missing objects in the pool");

            return poolItem;
        }

        public void BackToPool(PoolItem item)
        {
            _poolItems[item.NamePrefab].Enqueue(item);
            item.transform.SetParent(GetSection(_poolData.GetDataByName(item.NamePrefab).NameSection));
        }
        
        public void ResetAll()
        {
            foreach (var obj in _poolItems.SelectMany(item => item.Value.Where(obj => obj.gameObject != null)))
            {
                ResourceLoader.ReleaseInstance(obj.gameObject);
            }

            _poolItems.Clear();

            foreach (var item in _sections)
            {
                Object.Destroy(item);
            }

            _sections.Clear();
        }
    }
}