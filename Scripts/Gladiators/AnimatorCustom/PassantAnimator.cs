using Core;
using Gladiators.NPC;
using Managers.Interfaces;
using UnityEngine;

namespace Gladiators.AnimatorCustom
{
    public class PassantAnimator : AnimatorCustom,INPCModule
    {
        private readonly int AttackType = Animator.StringToHash("AttackType");

        public void Init()
        {
            _animatorController = GameClient.Instance.GetService<IGameplayManager>().GetController<Managers.Controller.AnimatorController>();
            Registration();
        }
        protected void SetAttackType(int type)
        {
            _animator.SetInteger(AttackType, type);
        }
    }
}
