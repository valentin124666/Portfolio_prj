using System;
using Core;
using Data;
using Data.Characteristics;
using Managers.Controller;
using Managers.Interfaces;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements.Popup
{
    public class GymPopupView : MonoBehaviour, IUIPopup
    {
        public bool IsActive => gameObject.activeSelf;

        [SerializeField] private TMP_Text _message;
        [SerializeField] private TMP_Text _currentBonus;
        [SerializeField] private TMP_Text _addedBonus;
        [SerializeField] private TMP_Text _cost;
        [SerializeField] private TMP_Text _lockLevel;

        [SerializeField] private Image _health;
        [SerializeField] private Image _attack;
        [SerializeField] private Image _defense;

        [SerializeField] private Button _buttonYes;
        [SerializeField] private Button _buttonNo;

        [SerializeField] private GameObject _lock;
        [SerializeField] private GameObject _lockLevelMessage;
        [SerializeField] private GameObject _infoBottomYes;
        
        private PlayerController _playerController;

        private Enumerators.InteractiveObjectName _typeSimulator;

        public void Show()
        {
            gameObject.SetActive(true);
            _playerController = GameClient.Get<IGameplayManager>().GetController<PlayerController>();
        }

        public void Show(Action callback)
        {
            gameObject.SetActive(true);
            _playerController = GameClient.Get<IGameplayManager>().GetController<PlayerController>();
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
            Reset();
        }

        public void AddListenerButtonYes(Action callback)
        {
            _buttonYes.onClick.AddListener(() =>
                {
                    UpgradeCharacteristic();

                    callback?.Invoke();
                    Hide();
                }
            );
        }

        public void AddListenerButtonNo(Action callback)
        {
            _buttonNo.onClick.AddListener(() =>
                {
                    callback?.Invoke();
                    Hide();
                }
            );
        }

        public void SetTypeSimulator(Enumerators.InteractiveObjectName type)
        {
            _typeSimulator = type;
            var dataManager = GameClient.Get<IGameDataManager>();
            var data = dataManager.GetDataScriptable<UIData>().gymPopupData;
            var dataGym = dataManager.GetDataScriptable<GymBalanceData>();
            GymBalance gymBalance = null;

            var playerBonus = 0;

            switch (_typeSimulator)
            {
                case Enumerators.InteractiveObjectName.TrainingApparatusBarbell:

                    _message.text = data.MassageBarbell;
                    _health.gameObject.SetActive(true);
                    _addedBonus.text = "+" + dataGym.Health.step;
                    _currentBonus.text = _playerController.PlayerAddedHealth.ToString();

                    gymBalance = dataGym.Health;
                    
                    playerBonus = _playerController.PlayerAddedHealth;
                    
                    break;
                case Enumerators.InteractiveObjectName.TrainingApparatusMakiwara:

                    _message.text = data.MassageMakiwara;
                    _defense.gameObject.SetActive(true);
                    _addedBonus.text = "+" + dataGym.Defense.step;
                    _currentBonus.text = _playerController.PlayerDefense.ToString();
                    
                    gymBalance = dataGym.Defense;

                    playerBonus = _playerController.PlayerDefense;
                    
                    break;
                case Enumerators.InteractiveObjectName.TrainingApparatusPunchingBag:

                    _message.text = data.MassagePunchingBag;
                    _attack.gameObject.SetActive(true);
                    _addedBonus.text = "+" + dataGym.Attack.step;
                    _currentBonus.text = _playerController.PlayerAttack.ToString();

                    gymBalance = dataGym.Attack;

                    playerBonus = _playerController.PlayerAttack;

                    break;
                default:
                    _message.text = "How did you do this ?";
                    gymBalance = new GymBalance();
                    break;
            }

            var cost = gymBalance.GetCost(playerBonus);
            _cost.text = cost.ToString();

            if (cost > _playerController.CurrentCoin)
                LockBottomYes();

            var requiredLevel = gymBalance.GetLimit(_playerController.FameLevel);
            if (requiredLevel <= playerBonus)
                LockBottomYesByLevel(gymBalance.GetComingChange(requiredLevel));
        }

        private void UpgradeCharacteristic()
        {
            var dataGym = GameClient.Get<IGameDataManager>().GetDataScriptable<GymBalanceData>();

            switch (_typeSimulator)
            {
                case Enumerators.InteractiveObjectName.TrainingApparatusBarbell:

                    if (_playerController.Payment(dataGym.Health.GetCost(_playerController.PlayerAddedHealth)))
                        _playerController.AddHealth(dataGym.Health.step);

                    break;
                case Enumerators.InteractiveObjectName.TrainingApparatusMakiwara:

                    if (_playerController.Payment(dataGym.Defense.GetCost(_playerController.PlayerDefense)))
                        _playerController.AddDefense(dataGym.Defense.step);

                    break;
                case Enumerators.InteractiveObjectName.TrainingApparatusPunchingBag:

                    if (_playerController.Payment(dataGym.Attack.GetCost(_playerController.PlayerAttack)))
                        _playerController.AddAttack(dataGym.Attack.step);

                    break;
            }
        }

        private void LockBottomYes()
        {
            _lock.SetActive(true);
            _buttonYes.onClick.RemoveAllListeners();
            _buttonYes.enabled = false;
        }

        private void LockBottomYesByLevel(int requiredLevel)
        {
            _infoBottomYes.SetActive(false);

            _lock.SetActive(true);
            _lockLevelMessage.SetActive(true);

            _lockLevel.text = "Level " + requiredLevel;

            _buttonYes.onClick.RemoveAllListeners();
            _buttonYes.enabled = false;
        }
        
        public void Reset()
        {
            _lock.SetActive(false);
            _lockLevelMessage.SetActive(false);

            _infoBottomYes.SetActive(true);
            _health.gameObject.SetActive(false);
            _attack.gameObject.SetActive(false);
            _defense.gameObject.SetActive(false);

            _typeSimulator = Enumerators.InteractiveObjectName.Unknown;
            _buttonYes.onClick.RemoveAllListeners();
            _buttonYes.enabled = true;

            _buttonNo.onClick.RemoveAllListeners();
        }
    }
}