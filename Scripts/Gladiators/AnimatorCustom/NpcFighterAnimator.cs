using System;
using DG.Tweening;
using Settings;
using Random = UnityEngine.Random;

namespace Gladiators.AnimatorCustom
{
    public class NpcFighterAnimator : PassantAnimator
    {
        private Sequence _seqAnimations;
        
        private void Start()
        {
            Init();
            _seqAnimations = DOTween.Sequence();

            _seqAnimations.InsertCallback(0, RandomAnimations);
            _seqAnimations.AppendInterval(Random.Range(2f,4f));
            _seqAnimations.SetLoops(-1);
        }

        private void RandomAnimations()
        {
            SetAttackType(Random.Range(0, 3));

            SetAnimation(Enumerators.FighterAnimations.Attack.ToString());
        }
    }
}