using Gladiators.AnimatorCustom;
using Settings;

namespace Level.Interactive
{
    public class BenchAnimatorCustom : AnimatorCustom
    {
        public void StartWork()
        {
            SetAnimation(Enumerators.WorkbenchShield.PressDown.ToString());
        }
        public void EndWork()
        {
            SetAnimation(Enumerators.WorkbenchShield.PressUp.ToString());
        }

        public void ResetAnimation()
        {
            ResetTriggerAnimation(Enumerators.WorkbenchShield.PressUp.ToString());
            ResetTriggerAnimation(Enumerators.WorkbenchShield.PressDown.ToString());

        }
    }
}
