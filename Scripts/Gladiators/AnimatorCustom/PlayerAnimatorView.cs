using Core;
using General;
using Gladiators.Interfaces;
using Gladiators.Sound;
using Managers.Controller;
using Managers.Interfaces;
using Settings;
using UnityEngine;

namespace Gladiators.AnimatorCustom
{
    public class PlayerAnimatorView : AnimatorCustom, IPlayerModule
    {
        [SerializeField] private PlayerSoundView _playerSoundView;

        private readonly int IsWork = Animator.StringToHash("IsWork");
        private readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
        private readonly int InteractionType = Animator.StringToHash("InteractionType");

        public void Init(PlayerController controller)
        {
            _animatorController = GameClient.Instance.GetService<IGameplayManager>().GetController<AnimatorController>();
            var animationAction = _animator.GetComponent<AnimationAction>();
            
            if (animationAction != null)
                animationAction.animationEvent += (name) => _playerSoundView.PlaySound(name);
            
            Registration();
        }

        public void SetMovementSpeed(float speed)
        {
            _animator.SetFloat(MoveSpeed, speed);
        }

        public void SetInteractionType(int type)
        {
            _animator.SetInteger(InteractionType, type);
        }

        public void SetInteractive(bool isActive)
        {
            _animator.SetBool(IsWork, isActive);
        }

        public void Reset()
        {
            _animator.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            SetAnimation(Enumerators.FighterAnimations.Reset.ToString());
        }
    }
}