using Core;
using Data;
using DG.Tweening;
using General;
using Managers.Controller;
using Managers.Interfaces;
using Settings;
using Tools;
using UIElements.Popup;
using UnityEngine;
using ReceptacleObject = General.ReceptacleObject;

namespace Level.Interactive
{
    public class Bench : InteractiveObject
    {
        [SerializeField] private Enumerators.ReceptacleObjectType _productType;
        [SerializeField] private Transform _workbenchCenter;
        [SerializeField] private Timer _timer;

        [SerializeField] private StorekeeperView _storekeeperMaterial;
        [SerializeField] private StorekeeperView _storekeeperProduct;

        [SerializeField] private BenchAnimatorCustom _benchAnimator;

        private PoolController _poolController;
        private LevelController _levelController;
        private PlayerController _playerController;

        private ProductCharacteristicsData _productData;

        private IUIManager _uIManager;

        private ReceptacleObject _materialOnBench;
        private ReceptacleObject _productOnBench;

        [SerializeField] private float _productionTimeItem;
        private int _rightAmountMaterial;
        private bool _isInteraction;

        private bool IsMaterialOnBench => _materialOnBench != null;

        public override bool AllowAccess => CheckTool();
        public override bool FinishedWorking => !CheckTool() && !IsMaterialOnBench;
        public override Enumerators.InteractiveObjectType InteractiveObjectType => Enumerators.InteractiveObjectType.Crafts;

        public override void Init()
        {
            _uIManager = GameClient.Get<IUIManager>();
            var gameplayManager = GameClient.Get<IGameplayManager>();
            _levelController = gameplayManager.GetController<LevelController>();
            _poolController = gameplayManager.GetController<PoolController>();
            _playerController = gameplayManager.GetController<PlayerController>();

            _productData = GameClient.Get<IGameDataManager>().GetDataScriptable<ProductCharacteristicsData>();

            _storekeeperMaterial.Init(_levelController.BenchReceptacleVolumeForMaterials(InteractiveName));
            _storekeeperProduct.Init(_levelController.BenchReceptacleVolumeForProducts(InteractiveName));

            _timeZoneActivation = _levelController.BenchTimeZoneActivation(InteractiveName);

            _rightAmountMaterial = 1;
            _timer.ResetTimer();
            _timerIterationStart.gameObject.SetActive(CheckTool());
        }

        public override void UpdateCustom()
        {
            if (!_timerIterationStart.gameObject.activeSelf)
                _timerIterationStart.gameObject.SetActive(CheckTool());

            if (!_isInteraction || !CheckTool()) return;

            if (!IsMaterialOnBench)
            {
                PutMaterial();
            }
        }

        public override void Interaction()
        {
            if (!CheckTool())
            {
                return;
            }

            _isInteraction = true;
            _timer.SetActive(true);
            ActivationMiniGame();
        }

        public override void SetActive(bool isActive, bool isAnimation = false)
        {
            base.SetActive(isActive, isAnimation);
            if (isAnimation)
            {
                transform.DOShakeScale(0.3f, 0.5f, vibrato: 10,
                    randomnessMode: ShakeRandomnessMode.Harmonic).SetEase(Ease.OutBack);
            }
        }

        public override void EndInteraction()
        {
            base.EndInteraction();

            _isInteraction = false;
            _timer.SetActive(false);
            _timerIterationStart.gameObject.SetActive(CheckTool());
            DeactivateMiniGame();
        }

        private void PutMaterial()
        {
            _materialOnBench = _storekeeperMaterial.GetReceptacle.GetReceptacleObject(Enumerators.ReceptacleObjectType.Material);

            _storekeeperMaterial.GiveReceptacleObject(_rightAmountMaterial);

            _materialOnBench.transform.DORotate(_workbenchCenter.eulerAngles, _productionTimeItem / 4);

            var path = new[] { (_workbenchCenter.position + _materialOnBench.transform.position) / 2 + Vector3.up * 1.5f, _workbenchCenter.position };


            _materialOnBench.transform.DOPath(path, _productionTimeItem / 4, pathType: PathType.CatmullRom).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (_benchAnimator != null)
                        _benchAnimator.StartWork();

                    _timer.StartTimer(EndOfProduction);
                });
        }

        private void EndOfProduction()
        {
            _productOnBench = CreateProduct();

            _storekeeperProduct.TakeReceptacleObject(1);
            _storekeeperProduct.GetReceptacle.SetReceptacleObject(_productOnBench);

            _materialOnBench.ReturnToPool();
            _materialOnBench = null;

            _productOnBench.transform.gameObject.SetActive(true);
            var target = _storekeeperProduct.GetReceptacle.GetWarehousePlace();
            _productOnBench.transform.DORotate(target.eulerAngles, _productionTimeItem / 4);

            var path = new[] { (target.position + _productOnBench.transform.position) / 2 + Vector3.up * 1.5f, target.position };

            if (_benchAnimator != null)
                _benchAnimator.EndWork();

            _productOnBench.transform.DOPath(path, _productionTimeItem / 4, pathType: PathType.CatmullRom).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    InternalTools.ActionDelayed(() => { _playerController.CheckAmmunition(_productType, _productData.GetProductByLevel(_playerController.FameLevel, _productType)); }, 0.2f);

                    _productOnBench = null;
                });
        }

        private void ActivationMiniGame()
        {
            var miniGame = _uIManager.GetPopup<InteractiveMiniGamePopup>();
            miniGame.Show();
            miniGame.ClickEvent += MiniGame;
        }

        private void DeactivateMiniGame()
        {
            var miniGame = _uIManager.GetPopup<InteractiveMiniGamePopup>();
            miniGame.Hide();
            miniGame.ClickEvent -= MiniGame;
        }

        private void MiniGame()
        {
            TakeTimeOffTimer();
        }

        private void TakeTimeOffTimer() => _timer.TakeTime(_playerController.AccelerationsProductionAClick);

        private ReceptacleObject CreateProduct()
        {
            var product = _poolController.GetPoolObj<ProductReceptacleObject>((Enumerators.NamePrefabAddressable)_productType);
            var data = _productData.GetProductByLevel(_playerController.FameLevel, _productType);

            product.Activation(data);

            product.transform.SetParent(_storekeeperProduct.transform);

            product.gameObject.SetActive(false);
            product.transform.position = _workbenchCenter.position;

            return product;
        }

        private bool CheckTool() => (_storekeeperMaterial.GetAvailableNumberObjects() > 0 && _storekeeperProduct.GetAvailableReceptacleSpace() > 0);
    }
}