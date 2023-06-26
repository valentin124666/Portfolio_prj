using UnityEngine;

namespace Gladiators.Interfaces
{
   public interface IPlayerPresenter 
   {
      Vector3 PosGladiator { get; }
      void SetActive(bool active);
      void SetPositionAndRotation(Vector3 pos, Quaternion rot);
      void ResetPos();
   }
}
