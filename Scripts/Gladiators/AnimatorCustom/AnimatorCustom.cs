using Gladiators.Interfaces;
using Managers.Controller;
using Settings;
using UnityEngine;

namespace Gladiators.AnimatorCustom
{
    public abstract class AnimatorCustom : MonoBehaviour, IAnimatorCustom
    {
        [SerializeField] protected Animator _animator;

        [SerializeField] private Enumerators.AnimatorUserType _animatorUserType;
        public Enumerators.AnimatorUserType AnimatorUserType => _animatorUserType;
        
        protected AnimatorController _animatorController;

        protected virtual void Registration()
        {
            _animatorController.RegistrationInController(_animatorUserType, this);
        }

        public void SetAnimation(int number)
        {
            _animator.SetTrigger(number);
        }
        public void SetAnimation(string name)
        {
            _animator.SetTrigger(name);
        }
        public void ResetTriggerAnimation(string name)
        {
            _animator.ResetTrigger(name);
        }
    }
}