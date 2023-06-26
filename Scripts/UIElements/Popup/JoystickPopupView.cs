using System;
using Managers.Interfaces;
using UnityEngine;

namespace UIElements.Popup
{
    public class JoystickPopupView : MonoBehaviour, IUIPopup
    {
        [SerializeField] private RectTransform _stikTransform;
        [SerializeField] private RectTransform _stikSmallTransform;

        public bool IsActive => gameObject.activeSelf;
        
        public void SetPosStick(Vector3 pos) => _stikTransform.position = pos;

        public void MoveStick(Vector3 localPos)
        {
            _stikSmallTransform.localPosition = localPos;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Show(Action callback)
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _stikSmallTransform.localPosition = Vector3.zero;
        }

        public void Reset()
        {
            _stikSmallTransform.localPosition = Vector3.zero;
        }
    }
}