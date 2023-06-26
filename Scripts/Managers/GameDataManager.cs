using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Core;
using Core.Interfaces;
using Cysharp.Threading.Tasks;
using Data;
using Data.Characteristics;
using UnityEngine;

namespace Managers
{
    public class GameDataManager : IService, IGameDataManager
    {
        private CharacteristicDataStanding _characteristicDataStanding;
        private CharacteristicDataStarting _characteristicDataStarting;
        private List<ScriptableObject> _dataScriptable;

        private Dictionary<IDataClient, string> _clients;

        private static string pathData
        {
            get
            {
#if UNITY_EDITOR
                return Path.Combine(Application.dataPath) + "/Data/Json";
#endif
                return Path.Combine(Application.persistentDataPath);
            }
        }
        
        public async UniTask Init()
        {
            _clients = new Dictionary<IDataClient, string>();
            _dataScriptable = new List<ScriptableObject>();

            _characteristicDataStanding = await ResourceLoader.GetResource<CharacteristicDataStanding>("CharacteristicDataStanding");
            _characteristicDataStarting = await ResourceLoader.GetResource<CharacteristicDataStarting>("CharacteristicDataStarting");

            _dataScriptable.Add(await ResourceLoader.GetResource<ProductCharacteristicsData>("ProductCharacteristics"));
            _dataScriptable.Add(await ResourceLoader.GetResource<EquipmentCharacteristicsData>("EquipmentCharacteristicsData"));
            _dataScriptable.Add(await ResourceLoader.GetResource<GameData>("GameData"));
            _dataScriptable.Add(await ResourceLoader.GetResource<UIData>("UIData"));
            _dataScriptable.Add(await ResourceLoader.GetResource<PoolData>("PoolData"));
            _dataScriptable.Add(await ResourceLoader.GetResource<BattleData>("BattleData"));
            _dataScriptable.Add(await ResourceLoader.GetResource<GymBalanceData>("GymBalanceData"));
            _dataScriptable.Add(await ResourceLoader.GetResource<ColorNPC>("ColorNPC"));
        }

        public T GetDataScriptable<T>() where T : ScriptableObject => (T)_dataScriptable.Find(data => data is T);


        public T Registration<T>(IDataClient client) where T : IData
        {
            var info = typeof(T).GetCustomAttribute<DataInfo>();

            if (info == null)
            {
                Debug.LogError($"[GameDataManager] - Registration: The declared class is missing an attribute 'DataInfo', class {typeof(T)}");
                throw new NullReferenceException();
            }

            object scriptable = _characteristicDataStanding.GetData().First(item => item.GetType() == info.Scriptable);
            var json = LoadJson(info.Json);

            _clients.Add(client, info.Json.GetCustomAttribute<DataName>().Name);

            return (T)Activator.CreateInstance(typeof(T), json, scriptable);
        }

        private object LoadJson(Type typeData)
        {
            var dataName = typeData.GetCustomAttribute<DataName>();

            if (dataName == null)
            {
                Debug.LogError($"[GameDataManager] - LoadJson: The declared class is missing an attribute 'DataName', class {typeof(Type)}");
                throw new NullReferenceException();
            }

            var pathToJson = pathData + dataName.Name;

            if (!File.Exists(pathToJson))
            {
                return _characteristicDataStarting.GetData().First(item => item.GetType() == typeData);
            }

            try
            {
                var json = File.ReadAllText(pathToJson);

                return JsonUtility.FromJson(json, typeData);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[GameDataManager] - LoadJson: {e.Message}");
                throw;
            }
        }

        public static object LoadJsonOrNull(Type typeData)
        {
            var dataName = typeData.GetCustomAttribute<DataName>();

            if (dataName == null)
            {
                Debug.LogError($"[GameDataManager] - LoadJson: The declared class is missing an attribute 'DataName', class {typeof(Type)}");
                throw new NullReferenceException();
            }

            var pathToJson = pathData + dataName.Name;

            if (!File.Exists(pathToJson))
            {
                return null;
            }

            try
            {
                var json = File.ReadAllText(pathToJson);

                return JsonUtility.FromJson(json, typeData);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[GameDataManager] - LoadJson: {e.Message}");
                throw;
            }
        }

        private void SaveJson(string json, string sectionName)
        {
            string pathToJson = pathData + sectionName;

            try
            {
                File.WriteAllText(pathToJson, json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[GameDataManager] - SaveJson: {e.Message}");
                throw;
            }
        }

        public void SaveDataClients()
        {
            if (_clients == null) return;

            foreach (var item in _clients)
            {
                var json = item.Key.GetData().GetDataJson();
                SaveJson(json, item.Value);
            }
        }
    }
}