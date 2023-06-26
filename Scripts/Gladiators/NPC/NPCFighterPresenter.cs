using System;
using System.Collections.Generic;
using Core;
using Data;
using DG.Tweening;
using General;
using Gladiators.AnimatorCustom;
using Gladiators.Sound;
using Managers.Controller;
using Managers.Interfaces;
using Tools;
using static Settings.Enumerators;
using Random = UnityEngine.Random;

namespace Gladiators.NPC
{
    public class NPCFighterPresenter : SimplePresenter<NPCFighterPresenter, NPCFighterPresenterView>
    {
        public event Action Death;

        private readonly List<INPCModule> _playerModule;
        private readonly NPCController _npcController;
        private readonly PlayerController _playerController;
        private readonly PoolController _poolController;
        private readonly ColorNPC _colorsNpc;

        private readonly FighterAnimatorModule _npcAnimator;
        private readonly NpcSoundView _npcSound;

        private readonly bool[] _probabilityStrongBlow;

        private Sequence _stateSeq;
        private Sequence _attackSeq;

        private float _maxHP;
        private float _currentHP;

        private bool _superKick;

        public FighterState CurrentState { get; private set; }

        public NPCFighterPresenter(NPCFighterPresenterView view, NPCController npcController) : base(view)
        {
            _stateSeq = DOTween.Sequence();
            _colorsNpc = GameClient.Get<IGameDataManager>().GetDataScriptable<ColorNPC>();
            _npcController = npcController;
            _playerController = GameClient.Instance.GetService<IGameplayManager>().GetController<PlayerController>();

            _playerModule = new List<INPCModule>();
            _playerModule.AddRange(View.GetComponents<INPCModule>());

            _npcSound = GetModule<NpcSoundView>();
            
            _probabilityStrongBlow = InternalTools.GetProbability(15);

            foreach (var item in _playerModule)
            {
                item.Init();
            }

            _poolController = GameClient.Instance.GetService<IGameplayManager>().GetController<PoolController>();

            _npcAnimator = GetModule<FighterAnimatorModule>();
        }

        private void OnDamage()
        {
            _npcAnimator.ResetTriggerAnimation(FighterAnimations.Idle.ToString());
            StateMachine(FighterState.Damage);
        }

        public void ReadinessForBattle()
        {
            StateMachine(FighterState.Idle);
            View.imageAlpha.Show(false);
        }

        public void OnIdle()
        {
            StateMachine(FighterState.Idle);
        }

        public void OnStunned(float time)
        {
            _npcAnimator.ResetTriggerAnimation(FighterAnimations.Idle.ToString());

            StateMachine(FighterState.Stunned, time);
        }

        public void OnStartBattle()
        {
            StateMachine(FighterState.StartBattle);
        }

        public void OnProtection()
        {
            StateMachine(FighterState.Protection);
        }

        public void Win()
        {
            StateMachine(FighterState.Win);
        }

        public void Attack()
        {
            StateMachine(FighterState.Attack);
        }

        public override void SetActive(bool active)
        {
            base.SetActive(active);
            _npcSound.StopAll();
            if (active)
            {
                View.SetCurrentAvatar(_npcController.IdAvatar);
                View.SetSwordModel(_npcController.FightWeaponModel);
                View.SetShieldModel(_npcController.FightShieldModel);
                View.SetHelmetModel(_npcController.FightHelmetModel);
                View.SetName(_npcController.FightName);
                
                View.SetColor(_colorsNpc.GetColorById(_npcController.IdColor));
                View.SetSize(_npcController.IsBoss ? _npcController.BossSize : 1);
            }
            else
            {
                View.SetHP(1);
                View.imageAlpha.Hide(true);
            }
        }

        public T GetModule<T>() where T : INPCModule
        {
            return (T)_playerModule.Find(module => module is T);
        }

        private bool CheckState(FighterState state)
        {
            switch (state)
            {
                case FighterState.Idle:
                    if (CurrentState is FighterState.Lose or FighterState.Win)
                    {
                        return false;
                    }

                    break;
                case FighterState.Attack:
                    if (CurrentState is FighterState.Win or FighterState.Lose || _currentHP == 0)
                    {
                        return false;
                    }

                    _npcAnimator.ResetTriggerAnimation(FighterAnimations.Idle.ToString());
                    break;
                case FighterState.Protection:
                    if (CurrentState != FighterState.Idle)
                    {
                        return false;
                    }

                    _npcAnimator.ResetTriggerAnimation(FighterAnimations.Idle.ToString());
                    break;
                case FighterState.Lose:
                    break;
                case FighterState.Win:
                    break;
                case FighterState.Damage:
                    if (CurrentState != FighterState.Idle)
                    {
                        return false;
                    }

                    break;
                case FighterState.StartBattle:
                    break;
                case FighterState.Stunned:
                    if (CurrentState is FighterState.Attack or FighterState.Protection or FighterState.Damage)
                    {
                        return false;
                    }

                    break;
            }

            return true;
        }

        private void StateMachine(FighterState state, params object[] args)
        {
            if (!CheckState(state)) return;

            CurrentState = state;
            _stateSeq?.Kill();
            _stateSeq = DOTween.Sequence();

            switch (state)
            {
                case FighterState.Idle:

                    _npcAnimator.SetAnimation(FighterAnimations.Idle.ToString());

                    break;
                case FighterState.StartBattle:
                    _npcAnimator.SetAnimation(FighterAnimations.BattleStart.ToString());
                    _npcAnimator.SetStartBattleType(Random.Range(0, 2));

                    break;
                case FighterState.Stunned:
                    _attackSeq.Kill();

                    _npcAnimator.SetAnimation(FighterAnimations.Stunned.ToString());
                    
                    _npcSound.PlaySound(SoundName.Stun);
                    
                    var time = InternalTools.GetTypeObject<float>(args[0]);

                    InternalTools.ActionDelayed(() =>
                    {
                        StateMachine(FighterState.Idle);
                    }, time);

                    break;
                case FighterState.Attack:
                    _attackSeq.Kill();
                    _superKick = !_superKick && _probabilityStrongBlow[Random.Range(0, _probabilityStrongBlow.Length)];

                    _npcAnimator.SetAttackType(!_superKick ? Random.Range(0, 3) : 3);

                    _npcAnimator.SetAnimation(FighterAnimations.Attack.ToString());

                    _attackSeq = DOTween.Sequence();
                    _attackSeq.InsertCallback(_npcController.FightNPCAttackSpeed, () =>
                    {
                        StateMachine(FighterState.Idle);

                        _playerController.AttackOnPlayerFighter(_npcController.FightNPCAttack, !_superKick);
                        _attackSeq = null;
                    });

                    break;
                case FighterState.Protection:
                    _npcAnimator.SetAnimation(FighterAnimations.Protection.ToString());
                    _stateSeq.InsertCallback(_npcController.FightNPCProtectionDuration, () => { StateMachine(FighterState.Idle); });

                    break;
                case FighterState.Lose:
                    _attackSeq.Kill();
                    _npcAnimator.SetAnimation(FighterAnimations.Lose.ToString());
                    View.imageAlpha.Hide(false);

                    break;
                case FighterState.Win:
                    _attackSeq.Kill();
                    _npcAnimator.SetAnimation(FighterAnimations.Win.ToString());

                    break;
                case FighterState.Damage:
                    if (_attackSeq != null || (_currentHP <= 0)) return;

                    _attackSeq.Kill();

                    _npcAnimator.SetAnimation(FighterAnimations.Damage.ToString());

                    InternalTools.ActionDelayed(() => { StateMachine(FighterState.Idle); }, 0.8f);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public void SetHealth(float health)
        {
            _maxHP = health;
            _currentHP = health;
        }


        public void Damage(float damage)
        {
            if (damage < 0) damage = 0;

            var message = damage.ToString();

            _currentHP -= damage;


            if (damage > 0)
            {
                var quickMessageItem = _poolController.GetPoolObj<QuickMessageItem>(NamePrefabAddressable.Message);

                quickMessageItem.transform.SetPositionAndRotation(View.PosMessage, View.RotationMessage);
                quickMessageItem.Activation();

                message = "-" + message;
                quickMessageItem.SetMessage(message);
            }

            if (CurrentState == FighterState.Protection)
            {
                _npcSound.PlaySound(SoundName.Def);
            }
            else if (damage > 0)
            {
                _npcSound.PlaySound(SoundName.SuccessfulAttackPlayer);

                View.DamageEffect();
                OnDamage();
            }


            if (_currentHP <= 0)
            {
                StateMachine(FighterState.Lose);
                Death?.Invoke();
                View.SetHP(0);

                return;
            }

            View.SetHP(_currentHP / _maxHP);
        }
    }
}