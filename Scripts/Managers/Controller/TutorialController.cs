using System;
using System.Linq;
using Core;
using Cysharp.Threading.Tasks;
using Data;
using Data.Characteristics;
using General;
using Gladiators.Player;
using Level;
using Managers.Interfaces;
using Tools;
using Tutorial;
using UIElements.Popup;
using UnityEngine;
using static Settings.Enumerators;

namespace Managers.Controller
{
    public class TutorialController : IController, IDataClient
    {
        public bool IsInit { get; private set; }

        private Transform _poolTutorial;

        private CameraData _cameraData;

        private CameraController _cameraController;
        private PlayerController _playerController;
        private LevelController _levelController;

        private MessagePopupView _messagePopup;
        private PointerArrow _pointerArrow;

        private TutorialData _tutorialData;
        private TutorialStep _tutorialStep;

        public void Init()
        {
            _poolTutorial = new GameObject("[PoolTutorial]").transform;
            _cameraData = GameClient.Instance.GetService<IGameDataManager>().GetDataScriptable<GameData>().cameraData;

            _messagePopup = GameClient.Get<IUIManager>().GetPopup<MessagePopupView>();
            _cameraController = GameClient.Get<IGameplayManager>().GetController<CameraController>();
            _levelController = GameClient.Get<IGameplayManager>().GetController<LevelController>();
            _playerController = GameClient.Get<IGameplayManager>().GetController<PlayerController>();

            _tutorialData = GameClient.Get<IGameDataManager>().Registration<TutorialData>(this);
            CreteArrow().Forget();
        }

        private void StepCompleted(TutorialStep step)
        {
            _tutorialData.Steps.Find(item => item.name == step).completed = true;
        }


        #region Steps

        public void CheckTutorial()
        {
            foreach (var step in _tutorialData.Steps.Where(step => !step.completed))
            {
                ActivationStepTutor(step.name);
                return;
            }
        }

        public void ShowStock()
        {
            StepCompleted(_tutorialStep);
            ActivationStepTutor(TutorialStep.ShowStock);
        }

        public void DropMaterial()
        {
            StepCompleted(_tutorialStep);

            ActivationStepTutor(TutorialStep.DropMaterial);
        }

        public void MachineWork()
        {
            StepCompleted(_tutorialStep);

            ActivationStepTutor(TutorialStep.MachineWork);
        }

        public void TakeSwords()
        {
            StepCompleted(_tutorialStep);

            ActivationStepTutor(TutorialStep.TakeSwords);
        }

        public void ShowRack()
        {
            StepCompleted(_tutorialStep);

            ActivationStepTutor(TutorialStep.ShowRack);
        }

        public void TakeMoney()
        {
            StepCompleted(_tutorialStep);

            ActivationStepTutor(TutorialStep.TakeMoney);
        }

        public void ShowColosseum()
        {
            StepCompleted(_tutorialStep);

            ActivationStepTutor(TutorialStep.ShowColosseum);
        }

        public void GreatLoss()
        {
            StepCompleted(_tutorialStep);

            ActivationStepTutor(TutorialStep.GreatLoss);
        }

        public void FinishTutorial()
        {
            StepCompleted(_tutorialStep);

            SetActiveArrow(false);
        }

        private void ActivationStepTutor(TutorialStep tutorialStep)
        {
            _pointerArrow.Reset();
            _tutorialStep = tutorialStep;

            switch (tutorialStep)
            {
                case TutorialStep.PurchaseOfAnAnvil:
                    _levelController.LockLocation(LockLocation.All, true);
                    _levelController.ActivationPurchase(InteractiveObjectName.WorkbenchShield, false);
                    var purchase = _levelController.GetPurchase(InteractiveObjectName.WorkbenchSword);

                    ActivationArrow(0);

                    var playerPurchase = AddComponentPlayer<PlayerPurchase>();
                    playerPurchase.Init(this, purchase);

                    break;
                case TutorialStep.ShowStock:
                    _levelController.LockLocation(LockLocation.All, true);
                    _levelController.ActivationPurchase(InteractiveObjectName.WorkbenchShield, true);

                    Demonstrate(0);

                    ActivationArrow(1);

                    var playerStock = AddComponentPlayer<PlayerStock>();
                    playerStock.Init(this);

                    break;
                case TutorialStep.DropMaterial:
                    _levelController.LockLocation(LockLocation.All, true);

                    ActivationArrow(2);

                    var playerBench = AddComponentPlayer<PlayerBench>();
                    playerBench.Init(this);

                    break;
                case TutorialStep.MachineWork:
                    _levelController.LockLocation(LockLocation.All, true);

                    ActivationArrow(3);
                    var playerMachineWork = AddComponentPlayer<PlayerMachineWork>();
                    playerMachineWork.Init(this, _playerController, _playerController.GetPresenter<PlayerGladiatorPresenter>());
                    break;
                case TutorialStep.TakeSwords:
                    _levelController.LockLocation(LockLocation.All, true);

                    ActivationArrow(4);

                    var playerTakeSwords = AddComponentPlayer<PlayerTakeSwords>();
                    playerTakeSwords.Init(this);

                    break;
                case TutorialStep.ShowRack:
                    _levelController.LockLocation(LockLocation.All, true);

                    ActivationArrow(5);

                    var playerShowRack = AddComponentPlayer<PlayerShowRack>();
                    playerShowRack.Init(this);

                    break;
                case TutorialStep.TakeMoney:
                    _levelController.LockLocation(LockLocation.All, true);

                    ActivationArrow(6);

                    SetActiveArrow(false);

                    var playerTakeMoney = AddComponentPlayer<PlayerTakeMoney>();

                    var table = _levelController.GetLevel<SquareLevelPresent>().GetRack(InteractiveObjectName.WorkbenchSword).GetTable();
                    playerTakeMoney.Init(this, table);

                    break;
                case TutorialStep.ShowColosseum:
                    _levelController.LockLocation(LockLocation.All, true);

                    OpenAndShowColosseum();

                    break;
                case TutorialStep.GreatLoss:
                    _levelController.LockLocation(LockLocation.Gym, true);

                    InternalTools.ActionDelayed(() => { _levelController.LockLocation(LockLocation.Gym, false); }, _cameraData.TutorMoveSpeed + 0.7f);

                    ActivationArrow(8);
                    Demonstrate(3);

                    var playerGreatLoss = AddComponentPlayer<PlayerGreatLoss>();
                    playerGreatLoss.Init(this, _playerController);


                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tutorialStep), tutorialStep, null);
            }
        }

        #endregion

        private void OpenAndShowColosseum()
        {
            MainApp.Instance.LockTouch(true);

            _cameraController.Demonstrate(new[]
                {
                    _levelController.GetPlaceInterest(PlaceInterestType.TutorCamera, 1),
                    _levelController.GetPlaceInterest(PlaceInterestType.TutorCamera, 2)
                },
                () => MainApp.Instance.LockTouch(false));

            InternalTools.ActionDelayed(() => { _levelController.LockLocation(LockLocation.Forge, false); }, _cameraData.TutorMoveSpeed + 0.1f);
            InternalTools.ActionDelayed(() => { ActivationArrow(7); }, (_cameraData.TutorMoveSpeed) * 2 + _cameraData.TutorPause + 0.1f);

            var playerShowColosseum = AddComponentPlayer<PlayerShowColosseum>();
            playerShowColosseum.Init(this);
        }

        public void SetActiveArrow(bool isActive)
        {
            _pointerArrow.SetActive(isActive);
            if (isActive)
                _pointerArrow.Appearance();
        }

        private void ActivationArrow(int number)
        {
            _pointerArrow.SetActive(true);
            var posArrow = _levelController.GetPlaceInterest(PlaceInterestType.TutorArrow, number);
            _pointerArrow.SetPositionAndRotation(posArrow.position, posArrow.rotation);
            _pointerArrow.Appearance();
        }

        private void Demonstrate(int number)
        {
            TaskManager.ExecuteAfterDelay(1, () =>
            {
                var posCam = _levelController.GetPlaceInterest(PlaceInterestType.TutorCamera, number);
                MainApp.Instance.LockTouch(true);

                _cameraController.Demonstrate(new[] { posCam }, () => MainApp.Instance.LockTouch(false));
            });
        }

        private T AddComponentPlayer<T>() where T : Component
        {
            var playerGladiator = _playerController.GetPresenter<PlayerGladiatorPresenter>();
            return playerGladiator.AddComponent<T>();
        }

        private async UniTask CreteArrow()
        {
            _pointerArrow = await ResourceLoader.Instantiate<PointerArrow>(NamePrefabAddressable.PointerArrow.ToString(), _poolTutorial);
            _pointerArrow.SetActive(false);
            IsInit = true;
        }

        public IData GetData()
        {
            return _tutorialData;
        }
    }
}