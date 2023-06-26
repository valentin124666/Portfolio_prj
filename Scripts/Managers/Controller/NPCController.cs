using System;
using PlaceInterestType = Settings.Enumerators.PlaceInterestType;
using NPCStates = Settings.Enumerators.NPCStates;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using Data;
using Data.Characteristics;
using Gladiators.NPC;
using Level.Interactive;
using Managers.Interfaces;
using Settings;
using Tools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers.Controller
{
    public class NPCController : IController, IDataClient
    {
        public bool IsInit { get; private set; }

        private Transform _poolNPC;

        private List<NPCPresenter> _npcPresenters;
        private NPCFighterPresenter _npcFighter;
        private PlayerController _playerController;
        private LevelController _levelController;
        private readonly Dictionary<PlaceInterestType, int> _locationOfNpc = new();

        private NPCData _npcData;
        private BattleData _battleData;

        #region Options

        public float NPCTimeDoNothing => _npcData.NPCTimeDoNothing;
        public int NPCPoolSize => _npcData.NPCPoolSize;
        public int MinimumNumberNPCShop => _npcData.MinimumNumberNPCShop;
        public float BossSize => _npcData.BossSize;
        public float ProductFlySpeed => _npcData.ProductFlySpeed;
        public float CoinFlySpeed => _npcData.CoinFlySpeed;
        public float PauseBetweenEjectionCoin => _npcData.PauseBetweenEjectionCoin;

        public int IdColor => _npcData.IdColor;
        public int IdAvatar => _npcData.IdAvatar;
        public bool IsBoss => _npcData.IsBoss;

        public int FightIdNPC => _npcData.IdNPC;
        public string FightName => _npcData.Name;

        public float FightNPCHealth => _npcData.Health;
        public float FightNPCArmor => _npcData.Armor;
        public float FightNPCAttack => _npcData.Attack;
        public float FightNPCAttackSpeed => _npcData.AttackSpeed;
        public float FightNPCAttackDuration => _npcData.AttackDuration;
        public float FightNPCProtectionDuration => _npcData.ProtectionDuration;
        public float FightNPCIdleDuration => _npcData.IdleDuration;
        public int FightMoney => _npcData.Money;
        public int FightExp => _npcData.Exp;
        public int FightWeaponModel => _npcData.WeaponModel;
        public int FightShieldModel => _npcData.ShieldModel;
        public int FightHelmetModel => _npcData.HelmetModel;
        public Enumerators.NPCArchetype FightArchetype => (Enumerators.NPCArchetype)_npcData.Archetype;

        #endregion

        public void Init()
        {
            var dataManager = GameClient.Instance.GetService<IGameDataManager>();
            _npcData = dataManager.Registration<NPCData>(this);
            _battleData = dataManager.GetDataScriptable<BattleData>();

            SetFighterData();

            var gameManager = GameClient.Instance.GetService<IGameplayManager>();

            _playerController = gameManager.GetController<PlayerController>();
            _levelController = gameManager.GetController<LevelController>();

            _npcPresenters = new List<NPCPresenter>();
            _poolNPC = new GameObject("[PoolNPC]").transform;
            _locationOfNpc.Add(PlaceInterestType.ShopPosPlace, 0);

            CreateNPCPresenters().Forget();
        }

        private async UniTask CreateNPCPresenters()
        {
            await TaskManager.WaitUntil(() => _levelController.IsInit);
            
            for (var i = 0; i < NPCPoolSize; i++)
            {
                var npc = await CreateNPC();

                var place = i < MinimumNumberNPCShop ? PlaceInterestType.ShopPosPlace : (PlaceInterestType)Random.Range(2, 4);
                if (_locationOfNpc.ContainsKey(place))
                    _locationOfNpc[place]++;

                npc.HeadTowardsTarget(place);
                _npcPresenters.Add(npc);
            }

            _npcFighter = await ResourceLoader.Instantiate<NPCFighterPresenter, NPCFighterPresenterView>(_poolNPC, "", this);
            var pointSpawn = _levelController.GetPointSpawnFighter();

            _npcFighter.SetPositionAndRotation(pointSpawn.position, pointSpawn.rotation);
            SetActionPassant(false);
            SetActionFighter(false);

            IsInit = true;
        }
        private async UniTask<NPCPresenter> CreateNPC()
        {
            var npc = await ResourceLoader.Instantiate<NPCPresenter, NPCPresenterView>(_poolNPC, "", _levelController, this);
            var pointSpawn = _levelController.GetPointSpawn();

            npc.SetPositionAndRotation(pointSpawn.position, pointSpawn.rotation);
            npc.FinishedState += SetNewState;
            return npc;
        }

        public void SetActionPassant(bool active)
        {
            foreach (var presenter in _npcPresenters)
            {
                presenter.SetActive(active);
            }
        }

        public void SetActionFighter(bool active)
        {
            _npcFighter.SetActive(active);
            _npcFighter.SetHealth(FightNPCHealth);
            if (active)
            {
                _playerController.AddListenerToDeathOfFighter(NPCWinBattle);
            }
            else
            {
                _playerController.RemoveListenerToDeathOfFighter(NPCWinBattle);
            }
        }

        private void SetFighterData()
        {
            var data = _battleData.GetNPCFighterData(FightIdNPC);

            _npcData.Name = data.Name;

            _npcData.Health = data.Health;
            _npcData.Armor = data.Armor;
            _npcData.Attack = data.Attack;
            _npcData.AttackDuration = data.AttackDuration;
            _npcData.ProtectionDuration = data.ProtectionDuration;
            _npcData.IdleDuration = data.IdleDuration;
            _npcData.Money = data.Money;
            _npcData.Exp = data.Exp;

            _npcData.IdColor = data.IdColor;
            _npcData.IdAvatar = data.IdAvatar;
            _npcData.IsBoss = data.IsBoss;

            _npcData.WeaponModel = data.WeaponModel;
            _npcData.ShieldModel = data.ShieldModel;
            _npcData.HelmetModel = data.HelmetModel;
            _npcData.Archetype = (ushort)data.Archetype;
        }


        private void SetNewState(NPCPresenter npc)
        {
            switch (npc.CurrentState)
            {
                case NPCStates.Unknown:
                    npc.HeadTowardsTarget(PlaceInterestType.AlleyPosPlace);

                    break;
                case NPCStates.Buy:

                    npc.MoveAwayFromRack();

                    npc.HeadTowardsTarget(PlaceInterestType.AlleyPosPlace);

                    _locationOfNpc[PlaceInterestType.ShopPosPlace]--;

                    break;
                case NPCStates.Movement:
                    if (npc.PlaceInterestType == PlaceInterestType.RackPosPlace && npc.ThereAreGoodsOnRack())
                    {
                        npc.Buy();
                    }
                    else
                    {
                        npc.ToWait();
                    }

                    break;
                case NPCStates.DoNothing:
                    switch (npc.PlaceInterestType)
                    {
                        case PlaceInterestType.AlleyPosPlace:
                            npc.HeadTowardsTarget(PlaceInterestType.AlleyFrontShopPosPlace);

                            break;
                        case PlaceInterestType.AlleyFrontShopPosPlace:
                            if (_locationOfNpc[PlaceInterestType.ShopPosPlace] < MinimumNumberNPCShop)
                            {
                                npc.HeadTowardsTarget(PlaceInterestType.ShopPosPlace);

                                _locationOfNpc[PlaceInterestType.ShopPosPlace]++;
                            }
                            else
                            {
                                npc.HeadTowardsTarget(PlaceInterestType.AlleyPosPlace);
                            }

                            break;
                        case PlaceInterestType.ShopPosPlace:
                            NPCInteractiveObject rack = null;
                            if (_levelController.FreeSpaceRack(ref rack))
                            {
                                npc.TakePlaceAndGoRack(rack);
                            }
                            else
                            {
                                if (_locationOfNpc[PlaceInterestType.ShopPosPlace] >= MinimumNumberNPCShop)
                                {
                                    npc.HeadTowardsTarget(PlaceInterestType.AlleyPosPlace);

                                    _locationOfNpc[PlaceInterestType.ShopPosPlace]--;
                                }
                                else
                                {
                                    npc.HeadTowardsTarget(PlaceInterestType.ShopPosPlace);
                                }
                            }

                            break;
                        case PlaceInterestType.RackPosPlace:

                            if (npc.ThereAreGoodsOnRack())
                            {
                                npc.Buy();
                            }
                            else
                            {
                                npc.MoveAwayFromRack();
                                _locationOfNpc[PlaceInterestType.ShopPosPlace]--;

                                npc.HeadTowardsTarget(PlaceInterestType.AlleyFrontShopPosPlace);
                            }

                            break;
                        case PlaceInterestType.SpawnPosPlace:
                            npc.HeadTowardsTarget(PlaceInterestType.AlleyFrontShopPosPlace);

                            break;
                    }

                    break;
            }
        }

        public void ReadinessForBattle()
        {
            _npcFighter.ReadinessForBattle();
        }

        public void SetStateFighter(Enumerators.FighterState state)
        {
            if (_npcFighter.CurrentState == Enumerators.FighterState.Stunned) return;

            switch (state)
            {
                case Enumerators.FighterState.Idle:
                    _npcFighter.OnIdle();
                    break;
                case Enumerators.FighterState.StartBattle:
                    _npcFighter.OnStartBattle();
                    break;
                case Enumerators.FighterState.Attack:
                    _npcFighter.Attack();
                    break;
                case Enumerators.FighterState.Protection:
                    _npcFighter.OnProtection();
                    break;
                case Enumerators.FighterState.Lose:
                    break;
                case Enumerators.FighterState.Win:
                    break;
                case Enumerators.FighterState.Stunned:
                    _npcFighter.OnStunned(_playerController.NPCStunTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void NPCWinBattle()
        {
            _playerController.RemoveListenerToDeathOfFighter(NPCWinBattle);
            _npcFighter.Win();
        }

        public void NextOpponent()
        {
            _npcData.IdNPC = _npcData.IdNPC + 1 > _battleData.MaxNumberOpponent - 1 ? _battleData.MaxNumberOpponent - 1 : _npcData.IdNPC + 1;

            SetFighterData();
        }

        public void AttackOnNPCFighter(float damage)
        {
            if (_npcFighter.CurrentState == Enumerators.FighterState.Protection)
                damage -= FightNPCArmor;

            _npcFighter.Damage(damage);
        }

        public void AddListenerToDeathOfFighter(Action call)
        {
            _npcFighter.Death += call;
        }

        public void RemoveListenerToDeathOfFighter(Action call)
        {
            _npcFighter.Death -= call;
        }

        public IData GetData()
        {
            return _npcData;
        }
    }
}