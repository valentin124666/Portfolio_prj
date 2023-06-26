using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gladiators.Interfaces;
using Managers.Interfaces;
using Settings;
using UnityEngine;

namespace Managers.Controller
{
    public class AnimatorController : IController
    {
        private readonly Dictionary<Enumerators.AnimatorUserType,List<IAnimatorCustom>> _animators = new();
        public bool IsInit { get; private set; }

        public void Init()
        {
            IsInit = true;
        }

        public void RegistrationInController(Enumerators.AnimatorUserType type,IAnimatorCustom animatorCustom)
        {
            if (!_animators.ContainsKey(type))
            {
                _animators[type] = new List<IAnimatorCustom>();
            }
            _animators[type].Add(animatorCustom);
        }
        /// <summary>
        ///  Sets the specified animation to all users of this type, AnimatorUserType.Unknown is all
        /// </summary>
        public void SetAnimations(Enumerators.AnimatorUserType type,int number)
        {
            if (type==Enumerators.AnimatorUserType.Unknown)
            {
                foreach (var item in _animators)
                {
                    foreach (var animatorCustom in item.Value)
                    {
                        animatorCustom.SetAnimation(number);
                    }
                }
            }

            foreach (var item in _animators[type])
            {
                item.SetAnimation(number);
            }
        }
        /// <summary>
        ///  Sets the specified animation to all users of this type, AnimatorUserType.Unknown is all
        /// </summary>
        public void SetAnimations(Enumerators.AnimatorUserType type,Enumerators.PlayerAnimations name)
        {
            if (type==Enumerators.AnimatorUserType.Unknown)
            {
                foreach (var item in _animators)
                {
                    foreach (var animatorCustom in item.Value)
                    {
                        animatorCustom.SetAnimation(name.ToString());
                    }
                }
            }

            foreach (var item in _animators[type])
            {
                item.SetAnimation(name.ToString());
            }
        }

        public Sequence AnimatedPopupDisplay(Transform obj)
        {
            return null;
        }
        
    }
    
}
