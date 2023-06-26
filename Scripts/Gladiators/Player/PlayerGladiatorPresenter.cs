using System;
using System.Collections.Generic;
using Core;
using DG.Tweening;
using Gladiators.AnimatorCustom;
using Gladiators.Interfaces;
using Gladiators.Sound;
using Level;
using Level.Interactive;
using Managers.Controller;
using Managers.Interfaces;
using Settings;
using Tools;
using UnityEngine;
using InteractiveObjectType = Settings.Enumerators.InteractiveObjectType;

namespace Gladiators.Player
{
    public class PlayerGladiatorPresenter : SimplePresenter<PlayerGladiatorPresenter, PlayerGladiatorPresenterView>, IPlayerPresenter
    {
        public Vector3 PosGladiator => View.transform.position;

        private readonly JoystickController _joystickController;

        private readonly PlayerAnimatorView _playerAnimator;
        private readonly PlayerMoveView _playerMoveView;
        private readonly PlayerSoundView _playerSoundView;

        private readonly PlayerController _playerController;
        private readonly LevelController _levelController;
        

        private readonly Dictionary<Enumerators.UpdateType, Action> _updates = new()
            { { Enumerators.UpdateType.Update, null }, { Enumerators.UpdateType.FixedUpdate, null }, { Enumerators.UpdateType.LateUpdate, null } };

        private readonly List<IPlayerModule> _playerModule;
        private List<IStateModule> _stateModule;

        public Enumerators.PlayerStates CurrentState;

        
        public PlayerGladiatorPresenter(PlayerGladiatorPresenterView view) : base(view)
        {
            SubscribeUpdates(true);
            var gameplayManager = GameClient.Get<IGameplayManager>();
            _joystickController = gameplayManager.GetController<JoystickController>();
            _playerController = gameplayManager.GetController<PlayerController>();
            _levelController = gameplayManager.GetController<LevelController>();

            _stateModule = new List<IStateModule>();
            _stateModule.AddRange(View.GetComponents<IStateModule>());

            foreach (var item in _stateModule)
            {
                _updates[item.UpdateType] += item.UpdateCustom;
            }

            _playerModule = new List<IPlayerModule>();
            _playerModule.AddRange(View.GetComponents<IPlayerModule>());

            var controller = gameplayManager.GetController<PlayerController>();

            foreach (var item in _playerModule)
            {
                item.Init(controller);
            }

            _playerAnimator = GetModule<PlayerAnimatorView>();
            _playerMoveView = GetModule<PlayerMoveView>();
            _playerSoundView = GetModule<PlayerSoundView>();
            
            _playerMoveView.MoveEvent += SetMovementSpeed;

            StateMachine(Enumerators.PlayerStates.Inaction);

            SetMovementSpeed(0);

            _joystickController.BeganTouch += EnableMovementState;
            _joystickController.EndTouch += DisableMovementState;
        }
        
        private T GetModule<T>() where T : IPlayerModule
        {
            return (T)_playerModule.Find(module => module is T);
        }

        private void SubscribeUpdates(bool subscribe)
        {
            if (subscribe)
            {
                MainApp.Instance.UpdateEvent += Update;
                MainApp.Instance.LateUpdateEvent += LateUpdate;
                MainApp.Instance.FixedUpdateEvent += FixedUpdate;
            }
            else
            {
                MainApp.Instance.UpdateEvent -= Update;
                MainApp.Instance.LateUpdateEvent -= LateUpdate;
                MainApp.Instance.FixedUpdateEvent -= FixedUpdate;
            }
        }

        private void EnableMovementState()
        {
            if (CurrentState == Enumerators.PlayerStates.Interaction)
                View.EndInteraction();

            StateMachine(Enumerators.PlayerStates.Movement);
        }

        private void DisableMovementState()
        {
            if (CurrentState != Enumerators.PlayerStates.Movement)
                return;

            SetMovementSpeed(0);
            _playerMoveView.EndMovement();
            StateMachine(Enumerators.PlayerStates.Inaction);
        }

        private void StateMachine(Enumerators.PlayerStates state, params object[] args)
        {
            CurrentState = state;

            switch (state)
            {
                case Enumerators.PlayerStates.Inaction:

                    _playerAnimator.SetAnimation(Enumerators.PlayerAnimations.Idle.ToString());
                    break;
                case Enumerators.PlayerStates.Movement:

                    _playerMoveView.StartMovement();
                    _playerAnimator.SetAnimation(Enumerators.PlayerAnimations.Run.ToString());
                    break;
                case Enumerators.PlayerStates.Storekeeper:

                    break;
                case Enumerators.PlayerStates.Interaction:
                    
                    var interactiveObject = InternalTools.GetTypeObject<InteractiveObject>(args[0]);
                    var callback = InternalTools.GetTypeObject<Action>(args[1]);
                    
                    var interactiveObjectType = interactiveObject.InteractiveObjectType;
                    _playerAnimator.SetInteractive(true);

                    switch (interactiveObjectType)
                    {
                        case InteractiveObjectType.Crafts:

                            _playerMoveView.EndMovement();
                            _playerAnimator.SetAnimation(Enumerators.PlayerAnimations.Run.ToString());
                            _playerAnimator.SetInteractionType((int)interactiveObject.InteractiveName);
                            _playerMoveView.MovingAPlace(interactiveObject.Workplace, 0.5f).OnComplete(() =>
                            {
                                if (_joystickController.IsTouch)
                                {
                                    EnableMovementState();
                                }
                                else
                                {
                                    _playerAnimator.SetAnimation(Enumerators.PlayerAnimations.Interaction.ToString());
                                    callback.Invoke();
                                }
                            });

                            break;
                        case InteractiveObjectType.ToBuy:
                            
                            var workbenchPurchaseView = (WorkbenchPurchaseView)interactiveObject;

                            if (!_playerController.Payment(workbenchPurchaseView.Cost))
                            {
                                workbenchPurchaseView.EndInteraction();
                                return;
                            }

                            GetModule<PlayerStorekeeperView>().GiveMoney(10, workbenchPurchaseView.Workplace).OnComplete(() => { callback?.Invoke(); });
                            _playerSoundView.PlaySound(Enumerators.SoundName.CraftingZone);

                            break;
                        case InteractiveObjectType.Exercise:
                            var trainingApparatus = (TrainingApparatus)interactiveObject;
                            callback.Invoke();

                            trainingApparatus.SetCallbackInteractive(() =>
                            {
                                _playerSoundView.PlaySound(Enumerators.SoundName.CraftingZone);

                                _playerMoveView.MovingAPlace(interactiveObject.Workplace, 0.5f).OnComplete(() =>
                                {
                                    if (_joystickController.IsTouch)
                                    {
                                        EnableMovementState();
                                    }
                                    else
                                    {
                                        _playerAnimator.SetInteractionType((int)interactiveObject.InteractiveName);

                                        _playerAnimator.SetAnimation(Enumerators.PlayerAnimations.Interaction.ToString());
                                    }
                                });
                            });

                            break;
                    }

                    break;
            }
        }

        private void Update()=>_updates[Enumerators.UpdateType.Update]?.Invoke();
        private void LateUpdate()=>_updates[Enumerators.UpdateType.LateUpdate]?.Invoke();
        private void FixedUpdate()=>_updates[Enumerators.UpdateType.FixedUpdate]?.Invoke();

        private void SetMovementSpeed(float speed) => _playerAnimator.SetMovementSpeed(speed);

        public void EnableInteractionState(InteractiveObject interactiveObject, Action callback)
            =>StateMachine(Enumerators.PlayerStates.Interaction, interactiveObject, callback);

        public void DisableInteractionState()
        {
            if (CurrentState != Enumerators.PlayerStates.Interaction)
                return;
            _playerAnimator.SetInteractive(false);

            StateMachine(Enumerators.PlayerStates.Inaction);
        }

        public void AddingCharacteristicsEffect()
        {
            _playerSoundView.PlaySound(Enumerators.SoundName.AddingCharacteristics);
        }
        
        public T AddComponent<T>() where T : Component
        {
            var component = View.gameObject.AddComponent(typeof(T));
            return (T)component;
        }
        public override void SetActive(bool active)
        {
            base.SetActive(active);
            _playerSoundView.StopAll();

            SubscribeUpdates(active);
        }
        
        public void ResetPos()
        {
            var pos = _levelController.GetPosPlayerSpawn<SquareLevelPresent>();

            SetPositionAndRotation(pos.position, pos.rotation);
        }

    }
}