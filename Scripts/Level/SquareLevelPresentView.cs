using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using DG.Tweening;
using Gladiators.Sound;
using Level.Interactive;
using Managers.Controller;
using Managers.Interfaces;
using Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Level
{
    [PrefabInfo(Enumerators.NamePrefabAddressable.SquareLevel)]
    public class SquareLevelPresentView : SimplePresenterView<SquareLevelPresent, SquareLevelPresentView>
    {
        private LevelController _levelController;

        [SerializeField] private GameObject _lockForge;
        [SerializeField] private GameObject _lockGym;

        [SerializeField] private SimpleSoundView _sound;

        [SerializeField] private Transform[] _cameraPos;
        [SerializeField] private Transform[] _posPlayerSpawn;

        [SerializeField] private StorekeeperView[] _noInteractive;
        [SerializeField] private InteractiveObject[] _interactiveObjects;
        [SerializeField] private List<NPCInteractiveObject> _NPCInteractiveObect;

        [SerializeField] private Purchase[] _purchases;

        [SerializeField] private PointsInterest[] _pointsInterest;

        private int _numberPosCum;
        private int _numberPosPlayer;

        public override void Init()
        {
            _levelController = GameClient.Instance.GetService<IGameplayManager>().GetController<LevelController>();
            _numberPosCum = 0;
            _numberPosPlayer = 0;
            _sound.Init();
            for (int i = 0; i < _pointsInterest.Length; i++)
            {
                _pointsInterest[i].Init();
            }

            for (int i = 0; i < _noInteractive.Length; i++)
            {
                _noInteractive[i].Init();
            }

            for (int i = 0; i < _interactiveObjects.Length; i++)
            {
                _interactiveObjects[i].Init();
            }

            for (int i = 0; i < _NPCInteractiveObect.Count; i++)
            {
                _NPCInteractiveObect[i].Init();
            }

            foreach (var item in _purchases)
            {
                var isActive = _levelController.BenchIsActivation(item.objectPurchase.InteractiveName);

                _NPCInteractiveObect.Find(rack => rack.InteractiveName == item.objectPurchase.InteractiveName)?.SetActive(isActive);
                item.objectPurchase.SetActive(isActive);
                item.workbenchPurchase.SetActive(!isActive);
            }

            MainApp.Instance.LateUpdateEvent += UpdateCustom;
        }

        public Purchase GetPurchase(WorkbenchPurchaseView workbenchPurchaseView) => _purchases.First(item => item.workbenchPurchase == workbenchPurchaseView);

        private void UpdateCustom()
        {
            for (int i = 0; i < _interactiveObjects.Length; i++)
            {
                _interactiveObjects[i].UpdateCustom();
            }
        }

        public void ActivationPurchase(Enumerators.InteractiveObjectName purchaseName, bool isActive)
        {
            _purchases.First(item => item.objectPurchase.InteractiveName == purchaseName).workbenchPurchase.SetActive(isActive);
        }

        public WorkbenchPurchaseView GetPurchase(Enumerators.InteractiveObjectName purchaseName)
        {
            return _purchases.First(item => item.objectPurchase.InteractiveName == purchaseName).workbenchPurchase;
        }

        public void ActivationRack(Enumerators.InteractiveObjectName interactiveName, bool isAnimation = false)
        {
            _NPCInteractiveObect.Find(item => item.InteractiveName == interactiveName).SetActive(true, isAnimation);
        }

        public PointsInterest GetPointsInterest(Enumerators.PlaceInterestType typePoint)
            => _pointsInterest.First(item => item.Type == typePoint);

        public NPCInteractiveObject GetFreeSpaceRack()
        {
            var freeRack = _NPCInteractiveObect.FindAll(item => item.FreePlaceCount);
            if (freeRack.Count == 0)
                return null;

            return freeRack[Random.Range(0, freeRack.Count)];
        }

        public NPCInteractiveObject GetRack(Enumerators.InteractiveObjectName type)
            => _NPCInteractiveObect.Find(item => item.InteractiveName == type);

        public Transform GetCameraAnchor()
        {
            var i = _numberPosCum;
            _numberPosCum = 1;

            return _cameraPos[i];
        }

        public Transform GetPosPlayerSpawn()
        {
            var i = _numberPosPlayer;
            _numberPosPlayer = 1;
            return _posPlayerSpawn[i];
        }

        public void LockLocation(Enumerators.LockLocation lockType, bool isLock)
        {
            switch (lockType)
            {
                case Enumerators.LockLocation.All:
                    _lockForge.SetActive(isLock);
                    _lockGym.SetActive(isLock);
                    break;
                case Enumerators.LockLocation.Forge:
                    if (isLock)
                    {
                        _lockForge.SetActive(true);
                    }
                    else
                    {
                        var scale = _lockForge.transform.localScale;
                        _lockForge.transform.DOScale(0, 0.5f).OnComplete(() =>
                        {
                            _lockForge.SetActive(false);
                            _lockForge.transform.localScale = scale;
                        });
                    }
                    break;

                case Enumerators.LockLocation.Gym:
                    if (isLock)
                    {
                        _lockGym.SetActive(true);
                    }
                    else
                    {
                        var scale = _lockGym.transform.localScale;
                        _lockGym.transform.DOScale(0, 0.5f).OnComplete(() =>
                        {
                            _lockGym.SetActive(false);
                            _lockGym.transform.localScale = scale;
                        });
                    }
                    break;
            }
        }

        public override void SetActive(bool active)
        {
            if (!active)
            {
                _sound.StopAll();
            }

            base.SetActive(active);

            if (active)
            {
                _sound.PlayRandomSound();
            }
        }
    }
}