using Core;
using Managers.Interfaces;
using UnityEngine;

namespace Gladiators.AnimatorCustom
{
    public class FighterAnimator :AnimatorCustom
    {
        [SerializeField] private int id;
        public int Id => id;
        
        private readonly int AttackType = Animator.StringToHash("AttackType");
        private readonly int BattleStartType = Animator.StringToHash("BattleStartType");

        public void Init()
        {
            _animatorController = GameClient.Instance.GetService<IGameplayManager>().GetController<Managers.Controller.AnimatorController>();
            Registration();
        }
        public void SetAttackType(int type)
        {
            _animator.SetInteger(AttackType, type);
        }
        public void SetStartBattleType(int type)
        {
            _animator.SetInteger(BattleStartType, type);
        }
    }
}
