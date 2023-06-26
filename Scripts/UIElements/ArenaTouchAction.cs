using System;
using Settings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIElements
{
    public class ArenaTouchAction : MonoBehaviour
    {
        [SerializeField] private TouchAction _protection;
        [SerializeField] private TouchAction _attack;
        [SerializeField] private TouchAction _dodgeRight;
        [SerializeField] private TouchAction _dodgeLeft;

        public TouchAction Protection => _protection;
        public TouchAction Attack => _attack;
        public TouchAction DodgeLeft => _dodgeLeft;
        public TouchAction DodgeRight => _dodgeRight;

        public void SetActivePressedAll(bool isActive)
        {
            _protection.SetClickTracking(isActive);
            _attack.SetClickTracking(isActive);
            _dodgeRight.SetClickTracking(isActive);
            _dodgeLeft.SetClickTracking(isActive);
        }

        public void SetActivePressed(Enumerators.ClickArenaState clickArenaState,bool isActive)
        {
            switch (clickArenaState)
            {
                case Enumerators.ClickArenaState.Attack:
                    _attack.SetClickTracking(isActive);

                    break;
                case Enumerators.ClickArenaState.Protection:
                    _protection.SetClickTracking(isActive);

                    break;
                case Enumerators.ClickArenaState.DodgeRight:
                    _dodgeRight.SetClickTracking(isActive);

                    break;
                case Enumerators.ClickArenaState.DodgeLeft:
                    _dodgeLeft.SetClickTracking(isActive);
                    break;
            }
        }
    }
}
