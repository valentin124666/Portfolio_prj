using System;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using Data;
using Data.Characteristics;
using Gladiators.Interfaces;
using Gladiators.Player;
using Level;
using Managers.Interfaces;
using Settings;
using Tools;
using UIElements.Pages;
using UIElements.Popup;
using UnityEngine;

namespace Managers.Controller
{
    public class PlayerController : IController, IDataClient
    {
        public bool IsInit { get; private set; }

        private LevelController _levelController;
        private NPCController _npcController;
        private InGameSquarePagePresenter _inGameSquare;

        private List<IPlayerPresenter> _playerPresenters;
        private IPlayerPresenter _currentPlayer;
        private IGameDataManager _dataManager;

        private Transform _poolLevel;
        private EquipmentCharacteristics _sword;
        private EquipmentCharacteristics _shield;
        private PlayerData _playerData;

        public Vector3 PosPlayer => _currentPlayer.PosGladiator;

        #region Options

        public int IdModelSword => _sword.idMod;
        public int IdModelShield => _shield.idMod;

        public float PlayerMovementSpeed => _playerData.PlayerMovementSpeed;
        public float SpeedMovementReceptacleObject => _playerData.SpeedMovementReceptacleObject;
        public float FlightCurvatureReceptacleObject => _playerData.FlightCurvatureReceptacleObject;
        public float PauseBetweenEjectionReceptacleObject => _playerData.PauseBetweenEjectionReceptacleObject;
        public float NPCStunTime => _playerData.NPCStunTime;
        public int FactorHealthShield => _playerData.FactorHealthShield;
        public int PlayerBackpackSize => _playerData.PlayerBackpackSize;

        public int PlayerDefense => _playerData.Defense;
        public int PlayerAttack => _playerData.Attack;
        public int PlayerAddedHealth => _playerData.AddedHealth;

        public float FightPlayerHealth => _playerData.Health + _playerData.AddedHealth;
        public float FightPlayerArmor => _shield.power + _playerData.Defense;
        public float FightPlayerDamage => _sword.power + _playerData.Attack;
        public float FightPlayerAttackSpeed => _playerData.AttackSpeed;
        public float FightPlayerPauseBeforeAttack => _playerData.PauseBeforeAttack;
        public float FightPlayerProtectionTime => _playerData.ProtectionTime;
        public float FightPlayerDodgeTime => _playerData.DodgeTime;
        public float AccelerationsProductionAClick => _playerData.AccelerationsProductionAClick;
        public int CurrentCoin => _playerData.PlayerCurrentCoin;
        public int FameLevel => _playerData.FameLevel;
        public int CurrentXP => _playerData.NumberNextFameLevel;

        #endregion

        public void Init()
        {
            _inGameSquare = GameClient.Instance.GetService<IUIManager>().GetPage<InGameSquarePagePresenter>();

            var gameManager = GameClient.Instance.GetService<IGameplayManager>();
            _levelController = gameManager.GetController<LevelController>();
            _npcController = gameManager.GetController<NPCController>();

            _dataManager = GameClient.Instance.GetService<IGameDataManager>();
            _playerData = _dataManager.Registration<PlayerData>(this);

            _sword = _dataManager.GetDataScriptable<EquipmentCharacteristicsData>().GetEquipmentSword(_playerData.IdModelSword);
            _shield = _dataManager.GetDataScriptable<EquipmentCharacteristicsData>().GetEquipmentShield(_playerData.IdModelShield);

            _poolLevel = new GameObject("[PoolPlayer]").transform;

            DisplayMoney();
            DisplayCharacteristics();

            _inGameSquare.UpdateLevelCounter(FameLevel);

            _playerPresenters = new List<IPlayerPresenter>();

            CreatePlayerPresenters().Forget();
        }

        private async UniTask CreatePlayerPresenters()
        {
            var gladiator = await ResourceLoader.Instantiate<PlayerGladiatorPresenter, PlayerGladiatorPresenterView>(_poolLevel, "");
            gladiator.SetActive(false);

            await TaskManager.WaitUntil(() => _levelController.IsInit);

            var pos = _levelController.GetPosPlayerSpawn<SquareLevelPresent>();
            gladiator.SetPositionAndRotation(pos.position, pos.rotation);
            _playerPresenters.Add(gladiator);

            var fighterGladiator = await ResourceLoader.Instantiate<PlayerFighterGladiatorPresenter, PlayerFighterGladiatorPresenterView>(_poolLevel, "", this);
            fighterGladiator.SetActive(false);

            pos = _levelController.GetPosPlayerSpawn<ArenaColiseumPresenter>();
            fighterGladiator.SetPositionAndRotation(pos.position, pos.rotation);
            _playerPresenters.Add(fighterGladiator);

            IsInit = true;
        }

        public T GetPresenter<T>() where T : IPlayerPresenter
        {
            return (T)_playerPresenters.Find(controller => controller is T);
        }

        public void AddListenerToDeathOfFighter(Action call)
        {
            GetPresenter<PlayerFighterGladiatorPresenter>().Death += call;
        }

        public void RemoveListenerToDeathOfFighter(Action call)
        {
            GetPresenter<PlayerFighterGladiatorPresenter>().Death -= call;
        }

        public void CheckAmmunition(Enumerators.ReceptacleObjectType type, ProductCharacteristics data)
        {
            var popup = GameClient.Get<IUIManager>().GetPopup<UpdateAmmunitionPopupView>();

            switch (type)
            {
                case Enumerators.ReceptacleObjectType.Sword:
                    if (IdModelSword != data.idMod)
                    {
                        _playerData.IdModelSword = data.idMod;
                        _sword = _dataManager.GetDataScriptable<EquipmentCharacteristicsData>().GetEquipmentSword(_playerData.IdModelSword);

                        popup.Show();
                        popup.SetUpgrade(type, _sword);
                    }

                    break;
                case Enumerators.ReceptacleObjectType.Shield:
                    if (IdModelShield != data.idMod)
                    {
                        _playerData.IdModelShield = data.idMod;
                        _shield = _dataManager.GetDataScriptable<EquipmentCharacteristicsData>().GetEquipmentShield(_playerData.IdModelShield);

                        popup.Show();
                        popup.SetUpgrade(type, _shield);
                    }

                    break;
            }

            DisplayCharacteristics();
        }

        public void AddLevel(int addExp)
        {
            _playerData.NumberNextFameLevel += addExp;
            var playerUpgrades = _dataManager.GetDataScriptable<GameData>().GetPlayerUpgrade(FameLevel);

            if (playerUpgrades == null) return;

            if (_playerData.NumberNextFameLevel >= playerUpgrades.NumberNextLevel)
            {
                var surplus = _playerData.NumberNextFameLevel - playerUpgrades.NumberNextLevel;
                _playerData.FameLevel++;
                _playerData.NumberNextFameLevel = surplus;
            }

            _inGameSquare.UpdateLevelCounter(FameLevel);
        }

        public void AddDefense(int bonus)
        {
            _playerData.Defense += bonus;
        }

        public void AddAttack(int bonus)
        {
            _playerData.Attack += bonus;
        }

        public void AddHealth(int bonus)
        {
            _playerData.AddedHealth += bonus;
        }

        public void AddCoin(int coin)
        {
            if (coin < 0)
            {
                Debug.LogError("It is not possible to add a negative number");
                return;
            }

            _playerData.PlayerCurrentCoin += coin;
            DisplayMoney();
        }

        private void DisplayMoney()
        {
            _inGameSquare.MonetaryOperations(CurrentCoin);
        }

        public void DisplayCharacteristics(bool showDifference = false)
        {
            _inGameSquare.UpdateCharacteristics((int)FightPlayerHealth, (int)FightPlayerDamage, (int)FightPlayerArmor, showDifference);
        }

        public bool Payment(int money)
        {
            if (money < 0)
            {
                Debug.LogError("You can't give a negative amount");
                return false;
            }

            if (money > _playerData.PlayerCurrentCoin) return false;

            _playerData.PlayerCurrentCoin -= money;
            DisplayMoney();
            return true;
        }

        public void ActivationGladiator(Enumerators.AppState state)
        {
            if (_currentPlayer != null)
            {
                _currentPlayer.ResetPos();
                _currentPlayer.SetActive(false);
            }

            switch (state)
            {
                case Enumerators.AppState.InGameSquare:
                    _npcController.RemoveListenerToDeathOfFighter(PlayerFighterWin);
                    _currentPlayer = GetPresenter<PlayerGladiatorPresenter>();
                    _currentPlayer.SetActive(true);
                    break;
                case Enumerators.AppState.InGameArena:
                    _npcController.AddListenerToDeathOfFighter(PlayerFighterWin);
                    var playerPresenter = GetPresenter<PlayerFighterGladiatorPresenter>();
                    playerPresenter.SetActive(true);
                    playerPresenter.SetHealthAndProtection(FightPlayerHealth, FightPlayerArmor * FactorHealthShield);
                    _currentPlayer = playerPresenter;
                    break;
            }
        }

        private void PlayerFighterWin()
        {
            GetPresenter<PlayerFighterGladiatorPresenter>().Win();
        }

        public void ReadyBattle()
        {
            var player = GetPresenter<PlayerFighterGladiatorPresenter>();
            player.OnReady();
        }

        public void AttackOnPlayerFighter(float damage, bool canDodge)
        {
            var player = GetPresenter<PlayerFighterGladiatorPresenter>();
            switch (player.CurrentState)
            {
                case Enumerators.FighterState.Protection when !canDodge:
                    _npcController.SetStateFighter(Enumerators.FighterState.Stunned);
                    player.Parry();
                    return;
                case Enumerators.FighterState.Protection:
                    damage -= FightPlayerArmor;
                    break;
                case Enumerators.FighterState.Win:
                    return;
            }

            player.Damage(damage, canDodge);
        }
        
        public IData GetData()
        {
            return _playerData;
        }
    }
}