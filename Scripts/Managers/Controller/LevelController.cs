using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Cysharp.Threading.Tasks;
using Data.Characteristics;
using Level;
using Level.Interactive;
using Managers.Interfaces;
using Settings;
using Tools;
using UnityEngine;

namespace Managers.Controller
{
    public class LevelController : IController, IDataClient
    {
        public bool IsInit { get; private set; }

        private Transform _poolLevel;

        private List<ILevel> _levels;
        private List<StorekeeperView> _storekeepers;
        private ILevel _activeLevel;

        private LevelData _levelData;
        public void Init()
        {
            _storekeepers = new List<StorekeeperView>();
            _poolLevel = new GameObject("[PoolLevel]").transform;

            _levelData = GameClient.Instance.GetService<IGameDataManager>().Registration<LevelData>(this);

            CreateLevelPresenters().Forget();
        }

        private async UniTask CreateLevelPresenters()
        {
            await TaskManager.WaitUntil(() => GameClient.Get<IGameplayManager>().GetController<PoolController>().IsInit);
            _levels = new List<ILevel>
            {
                await ResourceLoader.Instantiate<SquareLevelPresent, SquareLevelPresentView>(_poolLevel, ""),
                await ResourceLoader.Instantiate<ArenaColiseumPresenter, ArenaColiseumPresenterView>(_poolLevel, "")
            };
            IsInit = true;
        }

        private BenchData GetBenchData(Enumerators.InteractiveObjectName name) => _levelData.Benches.FirstOrDefault(item => item.name == name);
        
        public void LockLocation(Enumerators.LockLocation lockType, bool isLock) => GetLevel<SquareLevelPresent>().LockLocation(lockType, isLock);
        public void ActivationPurchase(Enumerators.InteractiveObjectName purchaseName, bool isActive) => GetLevel<SquareLevelPresent>().ActivationPurchase(purchaseName, isActive);
        public WorkbenchPurchaseView GetPurchase(Enumerators.InteractiveObjectName purchaseName) => GetLevel<SquareLevelPresent>().GetPurchase(purchaseName);
        
        public bool BenchIsActivation(Enumerators.InteractiveObjectName name) => GetBenchData(name).isActivation;
        public float BenchTimeZoneActivation(Enumerators.InteractiveObjectName name) => GetBenchData(name).timeZoneActivation;
        public ushort BenchReceptacleVolumeForMaterials(Enumerators.InteractiveObjectName name) => GetBenchData(name).receptacleVolumeForMaterials;
        public ushort BenchReceptacleVolumeForProducts(Enumerators.InteractiveObjectName name) => GetBenchData(name).receptacleVolumeForProducts;

        public PointInterest GetPlaceInterest(Enumerators.PlaceInterestType typePoint) => GetLevel<SquareLevelPresent>().GetRandomPoint(typePoint);
        public Transform GetPlaceInterest(Enumerators.PlaceInterestType typePoint, int number) => GetLevel<SquareLevelPresent>().GetPoint(typePoint, number);

        public Transform GetPointSpawn() => GetLevel<SquareLevelPresent>().GetPointTransform(Enumerators.PlaceInterestType.SpawnPosPlace);
        public Transform GetPointSpawnFighter() => GetLevel<ArenaColiseumPresenter>().GetPointSpawnFighter();
        public bool FreeSpaceRack(ref NPCInteractiveObject rack) => GetLevel<SquareLevelPresent>().FreeSpaceRack(ref rack);

        public ReceptacleDataJson ReceptacleRegistration(StorekeeperView storekeeper)
        {
            _storekeepers.Add(storekeeper);

            var data = _levelData.ReceptacleData.Find(item => item.id == storekeeper.GetId());

            if (data != null) return data;

            data = new ReceptacleDataJson
            {
                id = storekeeper.GetId(),
                idMod = Array.Empty<int>()
            };
            _levelData.ReceptacleData.Add(data);

            return data;
        }

        public Transform GetCameraAnchor()
        {
            if (_activeLevel == null)
            {
                Debug.LogError("[LevelController] [GetCameraAnchor] No active level");
            }

            return _activeLevel.GetCameraAnchor();
        }

        public Transform GetPosPlayerSpawn<T>() where T : ILevel
        {
            return _levels.Find(level => level is T).GetPosPlayerSpawn();
        }


        public void ActivationLevel<T>() where T : ILevel
        {
            _activeLevel?.SetActive(false);

            _activeLevel = GetLevel<T>();
            if (_activeLevel == null)
            {
                Debug.LogError("[LevelController] [GetCameraAnchor] No such level");
                return;
            }

            _activeLevel.SetActive(true);
        }

        public void ActivationPurchaseInSquare(WorkbenchPurchaseView purchaseView)
        {
            var name = GetLevel<SquareLevelPresent>().ActivationPurchase(purchaseView);
            GetBenchData(name).isActivation = true;
        }
        
        public T GetLevel<T>() where T : ILevel
        {
            return (T)_levels.Find(item => item is T);
        }

        public IData GetData()
        {
            _levelData.ReceptacleData.Clear();

            foreach (var item in _storekeepers)
            {
                _levelData.ReceptacleData.Add(item.GetReceptacleData());
            }

            return _levelData;
        }
    }
}