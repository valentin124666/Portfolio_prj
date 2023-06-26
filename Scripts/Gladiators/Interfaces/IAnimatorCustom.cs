using Settings;

namespace Gladiators.Interfaces
{
     public interface IAnimatorCustom 
     {
          void SetAnimation(int number);
          void SetAnimation(string name);
          Enumerators.AnimatorUserType AnimatorUserType { get; }
     }
}
