using System;
using System.Linq;
using Gladiators.NPC;
using UnityEngine;

namespace Gladiators.AnimatorCustom
{
    public class FighterAnimatorModule : MonoBehaviour, INPCModule
    {
        [SerializeField] private FighterAnimator[] _fighterAnimators;

        private FighterAnimator _currentAnimator;

        public void Init()
        {
            foreach (var animator in _fighterAnimators)
            {
                animator.Init();
            }
        }

        public void SetAnimator(int id)
        {
            _currentAnimator = _fighterAnimators.FirstOrDefault(item => item.Id == id) ?? _fighterAnimators[0];
        }

        public void SetAttackType(int type) => _currentAnimator.SetAttackType(type);
        public void SetStartBattleType(int type) => _currentAnimator.SetStartBattleType(type);
        public void SetAnimation(string name) => _currentAnimator.SetAnimation(name);
        public void ResetTriggerAnimation(string name) => _currentAnimator.ResetTriggerAnimation(name);
    }
}