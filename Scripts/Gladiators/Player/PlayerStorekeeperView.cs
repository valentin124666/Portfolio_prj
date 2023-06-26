using System;
using Core;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using General;
using Gladiators.Interfaces;
using Gladiators.Sound;
using Level.Interactive;
using Managers.Controller;
using Managers.Interfaces;
using Receptacle;
using Settings;
using Tools;
using UnityEngine;

namespace Gladiators.Player
{
    public class PlayerStorekeeperView : MonoBehaviour, IPlayerModule, IStateModule
    {
        public Enumerators.UpdateType UpdateType => Enumerators.UpdateType.Update;

        [SerializeField] private ReceptacleUniversal _receptacle;
        [SerializeField] private Transform _basket;
        [SerializeField] private PlayerSoundView _playerSoundView;
        
        private StorekeeperView _storekeeperCoin;
        private PoolController _poolController;
        private PlayerController _playerController;

        private float _timerTakeCoin;

        public void Init(PlayerController controller)
        {
            var gameplayManager = GameClient.Get<IGameplayManager>();
            _poolController = gameplayManager.GetController<PoolController>();
            _playerController = controller;

            _receptacle.Init();
            _receptacle.SetReceptacleSize(controller.PlayerBackpackSize);
        }
        
        private void TakeReceptacleObject(StorekeeperView storekeeper)
        {
            var numberRequestedObjects =
                _receptacle.FreePlace < storekeeper.GetAvailableNumberObjects() ? _receptacle.FreePlace : storekeeper.GetAvailableNumberObjects();

            _receptacle.ToTakeReceptacleObject((int)numberRequestedObjects, storekeeper.GetTypeReceptacle());
            storekeeper.GiveReceptacleObject((int)numberRequestedObjects);
            FlightReceptacleObject((int)numberRequestedObjects, storekeeper.GetReceptacle, _receptacle, storekeeper.GetTypeReceptacle()).Forget();
        }

        private void GiveReceptacleObject(StorekeeperView storekeeper)
        {
            var numberRequestedObjects =
                _receptacle.InStockSpecific(storekeeper.GetTypeReceptacle()) < storekeeper.GetAvailableReceptacleSpace()
                    ? _receptacle.InStockSpecific(storekeeper.GetTypeReceptacle())
                    : storekeeper.GetAvailableReceptacleSpace();

            _receptacle.ToGiveReceptacleObject((int)numberRequestedObjects, storekeeper.GetTypeReceptacle());
            
            FlightReceptacleObject((int)numberRequestedObjects, _receptacle, storekeeper.GetReceptacle, storekeeper.GetTypeReceptacle(), () =>
            {
                storekeeper.TakeReceptacleObject((int)numberRequestedObjects);
            }).Forget();
        }

        private void SetActiveBasket()
        {
            _basket.gameObject.SetActive(_receptacle.receptacle.childCount > 0);
        }

        private async UniTask FlightReceptacleObject(int numberRequestedObjects, ReceptacleSimple from, ReceptacleSimple to, Enumerators.ReceptacleObjectType type, Action callback = null)
        {
            var currentNumber = 0;

            while (currentNumber < numberRequestedObjects)
            {
                var obj = from.GetReceptacleObject(type);
                _playerSoundView.PlaySound(Enumerators.SoundName.TransferItemsBetweenWarehouses);
                var transformTo = to.GetWarehousePlace();
                obj.transform.DOKill();
                obj.transform.rotation = transformTo.rotation;
                var path = new[] { (transformTo.position + obj.transform.position) / 2 + Vector3.up * _playerController.FlightCurvatureReceptacleObject, transformTo.position };
                obj.transform.DOPath(path, _playerController.SpeedMovementReceptacleObject, pathType: PathType.CatmullRom).SetEase(Ease.Linear).OnComplete(() =>
                {
                    obj.transform.SetParent(to.receptacle);
                    obj.transform.SetPositionAndRotation(transformTo.position, transformTo.rotation);
                    to.SetReceptacleObject(obj);
                    SetActiveBasket();
                });

                await TaskManager.WaitUntilDelay(_playerController.PauseBetweenEjectionReceptacleObject);
                currentNumber++;
            }
            callback?.Invoke();
        }

        private void TakeMoney()
        {
            if (_storekeeperCoin == null)
                return;

            if (_timerTakeCoin > 0)
            {
                _timerTakeCoin -= Time.deltaTime;
            }
            else
            {
                if ((int)_storekeeperCoin.GetAvailableNumberObjects() > 0)
                {
                    _storekeeperCoin.GiveReceptacleObject(1);
                    var coin = _storekeeperCoin.GetReceptacle.GetReceptacleObject(_storekeeperCoin.GetTypeReceptacle());
                    FlightOfMoney(coin, _receptacle.receptacle.position);
                    
                    _playerSoundView.PlaySound(Enumerators.SoundName.Coin);
                    
                    _timerTakeCoin = _playerController.PauseBetweenEjectionReceptacleObject / 2;
                    _playerController.AddCoin(1);
                }
                else
                {
                    _storekeeperCoin = null;
                }
            }
        }

        public Sequence GiveMoney(int iteration, Transform target)
        {
            var giveCoin = DOTween.Sequence();
            giveCoin.AppendCallback(() =>
            {
                var coin = _poolController.GetPoolObj<ReceptacleObject>(Enumerators.NamePrefabAddressable.Coin);

                coin.Activation();
                coin.transform.SetParent(target);
                coin.transform.SetPositionAndRotation(_basket.position, _basket.rotation);

                var position = target.position;
                FlightOfMoney(coin, position);
            });
            giveCoin.AppendInterval(_playerController.PauseBetweenEjectionReceptacleObject / 2);
            giveCoin.SetLoops(iteration);
            return giveCoin;
        }

        private void FlightOfMoney(PoolItem coin, Vector3 to)
        {
            coin.transform.DOKill();
            var path = new[] { (to + coin.transform.position) / 2 + Vector3.up * _playerController.FlightCurvatureReceptacleObject, to };

            coin.transform.DOPath(path, _playerController.SpeedMovementReceptacleObject , pathType: PathType.CatmullRom)
                .OnComplete(coin.ReturnToPool);
        }

        private void OnTriggerEnter(Collider other)
        {
            var storekeeper = other.GetComponent<StorekeeperView>();
            
            if (storekeeper == null) return;
            
            if (storekeeper.GetTypeReceptacle() == Enumerators.ReceptacleObjectType.Coin)
            {
                _storekeeperCoin = storekeeper;
            }
            else switch (storekeeper.StorekeeperType)
            {
                case Enumerators.StorekeeperType.ToGive:
                    TakeReceptacleObject(storekeeper);
                    break;
                case Enumerators.StorekeeperType.ToTake:
                    GiveReceptacleObject(storekeeper);
                    break;
            }
        }
        
        public void UpdateCustom()
        {
            TakeMoney();
        }
    }
}