using Core;
using Cysharp.Threading.Tasks;
using Data;
using Gladiators.NPC;
using Gladiators.Player;
using Managers.Interfaces;
using UIElements.Popup;
using UnityEngine;
using Tools;
using UIElements.Pages;
using FighterState = Settings.Enumerators.FighterState;
using Random = UnityEngine.Random;

namespace Managers.Controller
{
    public class BattleController : IController
    {
        private NPCController _npcController;
        private PlayerController _playerController;
        private InGameArenaPagePresenter _arenaColiseumPresenter;

        private PlayerFighterGladiatorPresenter _playerFighter;
        private CharacteristicsFightersPopupView _characteristicsFightersPopup;

        private NPCFighterPresenter _npcFighter;

        private readonly FighterState[] _fighterStates = { FighterState.Attack, FighterState.Idle, FighterState.Idle, FighterState.Attack };
        private FighterState _pastState;
        private int _repetitionFighterStates;

        private bool _isBattle;
        private bool _isBattleReady;

        private BattleData _battleData;

        private float _timerBeforeAction;

        public bool IsInit { get; private set; }


        public void Init()
        {
            _pastState = FighterState.Idle;
            _battleData = GameClient.Instance.GetService<IGameDataManager>().GetDataScriptable<BattleData>();

            _npcController = GameClient.Instance.GetService<IGameplayManager>().GetController<NPCController>();
            _playerController = GameClient.Instance.GetService<IGameplayManager>().GetController<PlayerController>();
            _arenaColiseumPresenter = GameClient.Instance.GetService<IUIManager>().GetPage<InGameArenaPagePresenter>();
            _timerBeforeAction = _battleData.MarkerMovementTime;
            _characteristicsFightersPopup = GameClient.Get<IUIManager>().GetPopup<CharacteristicsFightersPopupView>();

            _isBattle = false;
            _isBattleReady = false;

            IsInit = true;
        }

        public bool CheckAction(FighterState state)
        {
            var check = Random.Range(1, 4) % 2 != 0;
            if (!check && state == FighterState.Attack)
                _npcController.SetStateFighter(FighterState.Protection);

            return check;
        }

        public void StartBattle()
        {
            if (_isBattle)
                return;

            _isBattle = true;
            MainApp.Instance.UpdateEvent += Update;

            _npcController.AddListenerToDeathOfFighter(WinPlayer);
            _playerController.AddListenerToDeathOfFighter(LosePlayer);
            PresentationFighters().Forget();
        }

        private async UniTask PresentationFighters()
        {
            _arenaColiseumPresenter.SetActiveDisplayFast(false);
            _arenaColiseumPresenter.SetActionClicks(false);

            await TaskManager.WaitUntilDelay(0.1f);

            _npcController.SetStateFighter(FighterState.StartBattle);

            await TaskManager.WaitUntilDelay(3);
            _characteristicsFightersPopup.Show(() =>
            {
                _npcController.ReadinessForBattle();
                _playerController.ReadyBattle();
                _isBattleReady = true;
                _arenaColiseumPresenter.SetActionClicks(true);

                _arenaColiseumPresenter.SetActiveDisplay(true);
            });
        }

        public void EndBattle()
        {
            if (!_isBattle)
                return;

            _isBattleReady = false;

            _isBattle = false;
            MainApp.Instance.UpdateEvent -= Update;
            _npcController.RemoveListenerToDeathOfFighter(WinPlayer);
            _playerController.RemoveListenerToDeathOfFighter(LosePlayer);
        }
        
        private void WinPlayer()
        {
            EndBattle();
            _arenaColiseumPresenter.SetActionClicks(false);
            _arenaColiseumPresenter.SetActiveDisplayFast(false);

            InternalTools.ActionDelayed(() =>
            {
                var arenaPopup =  GameClient.Instance.GetService<IUIManager>().GetPopup<ArenaPopupWinView>();
                arenaPopup.Show(() =>
                {
                    _playerController.AddLevel(_npcController.FightExp);
                    _playerController.AddCoin(_npcController.FightMoney);
                    _npcController.NextOpponent();

                });
            }, 3f);
        }

        private void LosePlayer()
        {
            EndBattle();
            _arenaColiseumPresenter.SetActionClicks(false);
            _arenaColiseumPresenter.SetActiveDisplayFast(false);


            InternalTools.ActionDelayed(() => { GameClient.Instance.GetService<IUIManager>().DrawPopup<ArenaPopupLoseView>(); }, 3f);
        }
        
        private FighterState GetState()
        {
            while (true)
            {
                var state = _fighterStates[Random.Range(0, _fighterStates.Length)];
                if (state == _pastState && _pastState == FighterState.Idle) continue;

                if (state == _pastState)
                {
                    _repetitionFighterStates++;
                }
                else
                {
                    _repetitionFighterStates = 1;
                }

                if (_repetitionFighterStates == _battleData.RepetitionFighterStatesMax) continue;

                _pastState = state;
                return state;
            }
        }

        private void Update()
        {
            if (!_isBattleReady) return;

            if (_timerBeforeAction > 0)
            {
                _timerBeforeAction -= Time.deltaTime;
            }
            else
            {
                var state = GetState();

                _npcController.SetStateFighter(state);

                float duration = 0;
                switch (state)
                {
                    case FighterState.Idle:
                        duration = _npcController.FightNPCIdleDuration;
                        break;
                    case FighterState.Attack:
                        duration = _npcController.FightNPCAttackDuration;

                        break;
                    case FighterState.Protection:
                        duration = _npcController.FightNPCProtectionDuration;

                        break;
                }

                _timerBeforeAction = duration + 0.5f;
            }
        }
    }
}