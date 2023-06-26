using System;
using System.Collections.Generic;
using Core;
using DG.Tweening;
using Gladiators.AnimatorCustom;
using Gladiators.Interfaces;
using Gladiators.Sound;
using Managers.Controller;
using Managers.Interfaces;
using Tools;
using UIElements.Pages;
using UnityEngine;
using static Settings.Enumerators;

namespace Gladiators.Player
{
    public class PlayerFighterGladiatorPresenter : SimplePresenter<PlayerFighterGladiatorPresenter, PlayerFighterGladiatorPresenterView>, IPlayerPresenter
    {
        public event Action Death;
        public Vector3 PosGladiator => View.transform.position;

        private readonly InGameArenaPagePresenter _inGameArenaPage;

        private readonly List<IPlayerModule> _playerModule;
        private readonly PlayerAnimatorView _playerAnimator;
        private readonly PlayerSoundView _playerSoundView;

        private readonly PlayerController _playerController;

        private readonly NPCController _NPCController;
        private readonly BattleController _battleController;
        private readonly CameraController _cameraController;

        private Vector3 _defaultPos;
        private Quaternion _defaultRot;

        private Sequence _attackSeq;

        private readonly float _durationDamageEffect;
        private float _maxHP;
        private float _currentHP;
        private float _maxProtection;
        private float _currentProtection;

        public FighterState CurrentState { get; private set; }

        public PlayerFighterGladiatorPresenter(PlayerFighterGladiatorPresenterView view, PlayerController playerController) : base(view)
        {
            _playerController = playerController;

            _inGameArenaPage = GameClient.Instance.GetService<IUIManager>().GetPage<InGameArenaPagePresenter>();

            var gameplayManager = GameClient.Instance.GetService<IGameplayManager>();

            _NPCController = gameplayManager.GetController<NPCController>();
            _battleController = gameplayManager.GetController<BattleController>();
            _cameraController = gameplayManager.GetController<CameraController>();

            _playerModule = new List<IPlayerModule>();
            _playerModule.AddRange(View.GetComponents<IPlayerModule>());

            foreach (var item in _playerModule)
            {
                item.Init(_playerController);
            }

            _playerAnimator = GetModule<PlayerAnimatorView>();
            _playerSoundView = GetModule<PlayerSoundView>();

            _durationDamageEffect = 0.4f;
        }

        private bool CheckState(FighterState state)
        {
            switch (state)
            {
                case FighterState.Idle:
                    break;
                case FighterState.Attack:
                    break;
                case FighterState.Protection:
                    if (CurrentState != FighterState.Idle)
                    {
                        return false;
                    }

                    _playerAnimator.ResetTriggerAnimation(FighterAnimations.Idle.ToString());

                    break;
                case FighterState.Lose:
                    break;
                case FighterState.Win:
                    break;
                case FighterState.Damage:
                    break;
                case FighterState.Dodge:
                    if (CurrentState != FighterState.Idle)
                    {
                        return false;
                    }

                    break;
                case FighterState.StartBattle:
                    break;
                case FighterState.Stunned:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            return true;
        }
        
        public void OnReady()
        {
            StateMachine(FighterState.Idle);
        }

        private void Protection()
        {
            OnProtection();
        }

        private void OnProtection()
        {
            StateMachine(FighterState.Protection);
        }

        private void DodgeRight()
        {
            StateMachine(FighterState.Dodge, true);
        }

        private void DodgeLeft()
        {
            StateMachine(FighterState.Dodge, false);
        }

        private void Attack()
        {
            OnAttack();
        }
        public void Parry()
        {
            _playerSoundView.PlaySound(SoundName.Def);
        }

        public void Win()
        {
            StateMachine(FighterState.Win);
        }
        
        private void StateMachine(FighterState state, params object[] args)
        {
            if (!CheckState(state)) return;

            CurrentState = state;

            switch (state)
            {
                case FighterState.StartBattle:
                    _playerAnimator.SetAnimation(FighterAnimations.BattleStart.ToString());
                    _playerSoundView.PlaySound(SoundName.Audience);

                    break;
                case FighterState.Idle:
                    _playerAnimator.SetAnimation(FighterAnimations.Idle.ToString());
                    MainApp.Instance.LockClick(true);

                    break;
                case FighterState.Attack:
                    _attackSeq.Kill();
                    MainApp.Instance.LockClick(false);

                    _playerAnimator.SetAnimation(FighterAnimations.Attack.ToString());
                    _battleController.CheckAction(CurrentState);

                    _attackSeq = InternalTools.ActionDelayed(() =>
                    {
                        StateMachine(FighterState.Idle);
                        _NPCController.AttackOnNPCFighter(_playerController.FightPlayerDamage);
                    }, _playerController.FightPlayerAttackSpeed);

                    break;
                case FighterState.Protection:
                    MainApp.Instance.LockClick(false);

                    _playerAnimator.SetAnimation(FighterAnimations.Protection.ToString());

                    InternalTools.ActionDelayed(() =>
                    {
                        StateMachine(FighterState.Idle);

                        if (!(_currentProtection <= 0)) return;

                        View.RemoveArmShield();
                        _inGameArenaPage.SetActionBottom(ClickArenaState.Protection, false);
                    }, _playerController.FightPlayerProtectionTime);

                    break;
                case FighterState.Dodge:
                    MainApp.Instance.LockClick(false);

                    var type = InternalTools.GetTypeObject<bool>(args[0]);

                    _playerAnimator.SetAnimation(type ? FighterAnimations.DodgeRight.ToString() : FighterAnimations.DodgeLeft.ToString());

                    InternalTools.ActionDelayed(() =>
                    {
                        StateMachine(FighterState.Idle);
                        _playerAnimator.ResetTriggerAnimation(FighterAnimations.Idle.ToString());
                    }, _playerController.FightPlayerProtectionTime);

                    InternalTools.ActionDelayed(() => { _playerAnimator.SetAnimation(FighterAnimations.Idle.ToString()); }, _playerController.FightPlayerProtectionTime - 0.5f);

                    break;
                case FighterState.Lose:
                    MainApp.Instance.LockClick(true);

                    _inGameArenaPage.ShowDamageScreen(_durationDamageEffect);
                    _attackSeq.Kill();

                    _playerAnimator.SetAnimation(FighterAnimations.Lose.ToString());

                    break;
                case FighterState.Win:
                    MainApp.Instance.LockClick(true);
                    _playerSoundView.PlaySound(SoundName.Audience);
                    _attackSeq.Kill();

                    _playerAnimator.SetAnimation(FighterAnimations.Win.ToString());

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        private void OnAttack()
        {
            _playerAnimator.ResetTriggerAnimation(FighterAnimations.Idle.ToString());
            StateMachine(FighterState.Attack);
        }
        
        private void DamageEffect()
        {
            _playerSoundView.PlaySound(SoundName.SuccessfulAttackNPC);

            _cameraController.DOShake(_durationDamageEffect);
            _inGameArenaPage.ShowDamageScreen(_durationDamageEffect).OnComplete(() => _inGameArenaPage.HideDamageScreen(_durationDamageEffect / 2));
            View.DamageEffect();
        }
        
        private T GetModule<T>() where T : IPlayerModule
        {
            return (T)_playerModule.Find(module => module is T);
        }
        public override void SetActive(bool active)
        {
            base.SetActive(active);
            _playerSoundView.StopAll();
            if (active)
            {
                StateMachine(FighterState.StartBattle);

                _inGameArenaPage.GetTouchAction(ClickArenaState.Attack).onPointerDown += Attack;
                _inGameArenaPage.GetTouchAction(ClickArenaState.Protection).onPointerDown += Protection;
                _inGameArenaPage.GetTouchAction(ClickArenaState.DodgeRight).onPointerDown += DodgeRight;
                _inGameArenaPage.GetTouchAction(ClickArenaState.DodgeLeft).onPointerDown += DodgeLeft;

                View.SetShieldModel(_playerController.IdModelShield);
                View.SetSwordModel(_playerController.IdModelSword);
            }
            else
            {
                _playerAnimator.Reset();
                StateMachine(FighterState.Idle);

                _inGameArenaPage.GetTouchAction(ClickArenaState.Attack).onPointerDown -= Attack;
                _inGameArenaPage.GetTouchAction(ClickArenaState.Protection).onPointerDown -= Protection;
                _inGameArenaPage.GetTouchAction(ClickArenaState.DodgeRight).onPointerDown -= DodgeRight;
                _inGameArenaPage.GetTouchAction(ClickArenaState.DodgeLeft).onPointerDown -= DodgeLeft;

                View.SetHP(1);
            }
        }

        public void SetHealthAndProtection(float health, float protection)
        {
            _maxHP = health;
            _currentHP = health;
            _maxProtection = protection;
            _currentProtection = protection;
        }

        public void Damage(float damage, bool canDodge)
        {
            if (damage < 0) damage = 0;

            if (canDodge && CurrentState == FighterState.Dodge)
            {
                _playerSoundView.PlaySound(SoundName.Dodge);
                return;
            }

            if (CurrentState == FighterState.Protection)
            {
                var def = damage - _currentProtection;

                if (def <= 0)
                {
                    _currentProtection = Mathf.Abs(def);
                    _playerSoundView.PlaySound(SoundName.Def);
                    _inGameArenaPage.SetProtection(_currentProtection / _maxProtection);

                    return;
                }

                _currentProtection = 0;
                _inGameArenaPage.SetProtection(0);

                damage = def;
            }

            if (damage > 0)
                DamageEffect();

            _currentHP -= damage;

            if (_currentHP <= 0)
            {
                StateMachine(FighterState.Lose);
                Death?.Invoke();
                _inGameArenaPage.SetHP(0);

                return;
            }

            _inGameArenaPage.SetHP(_currentHP / _maxHP);
        }
        
        public override void SetPositionAndRotation(Vector3 pos, Quaternion rot)
        {
            _defaultPos = pos;
            _defaultRot = rot;
            base.SetPositionAndRotation(pos, rot);
        }

        public void ResetPos()
        {
            SetPositionAndRotation(_defaultPos, _defaultRot);
        }
    }
}