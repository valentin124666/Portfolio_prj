using System;
using Core;
using Cysharp.Threading.Tasks;
using Data;
using Managers.Interfaces;
using Settings;
using Tools;
using UIElements;
using UIElements.Popup;
using UnityEngine;

namespace Managers.Controller
{
    public class JoystickController : IController
    {
        public event Action BeganTouch;
        public event Action EndTouch;

        private JoystickPopupView _joystick;

        private JoystickPopupView Joystick
        {
            get
            {
                if (GameClient.Instance.GetService<IGameplayManager>().CurrentState == Enumerators.AppState.InGameSquare)
                {
                    return _joystick;
                }

                return null;
            }
        }

        private JoystickData _joystickData;

        private Vector3 _touchStartPos;
        private Vector3 _touchCurrentPos;
        private Vector3 _joystickDirection;

        public bool IsTouch { get; private set; }
        public bool IsInit { get; private set; }

        public void Init()
        {
            _joystickData = GameClient.Instance.GetService<IGameDataManager>().GetDataScriptable<GameData>().joystickData;

            _joystick = GameClient.Instance.GetService<IUIManager>().GetPopup<JoystickPopupView>();

            MainApp.Instance.UpdateEvent += Update;
            IsInit = true;
        }

        private void Update()
        {
            if (TouchUtility.TouchCount > 0 && !InternalTools.IsPointerOverUIObject())
            {
                Joystick?.Show();

                var touch = TouchUtility.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        IsTouch = true;

                        BeganTouch?.Invoke();
                        _touchStartPos = Input.mousePosition;

                        Joystick?.SetPosStick(_touchStartPos);
                        break;

                    case TouchPhase.Moved:
                        if (_touchStartPos == Vector3.zero)
                            _touchStartPos = Input.mousePosition;

                        _touchCurrentPos = Input.mousePosition;
                        var normalized = (_touchCurrentPos - _touchStartPos).normalized;
                        var magnitude = (_touchCurrentPos - _touchStartPos).magnitude;

                        if (magnitude > _joystickData.MaxDistansStick)
                        {
                            _touchStartPos += normalized * (magnitude - _joystickData.MaxDistansStick);
                            Joystick?.SetPosStick(_touchStartPos);

                            magnitude = _joystickData.MaxDistansStick;
                        }

                        if (magnitude >= _joystickData.BeginningMoveSmallStick)
                        {
                            Joystick?.MoveStick(normalized * magnitude);
                        }

                        _joystickDirection = new Vector3(normalized.x, 0, normalized.y);
                        break;
                    case TouchPhase.Ended:
                        IsTouch = false;
                        EndTouch?.Invoke();

                        Joystick?.Hide();
                        _joystickDirection = Vector3.zero;
                        break;
                }
            }
            else if (IsTouch)
            {
                IsTouch = false;
                EndTouch?.Invoke();

                Joystick?.Hide();
                _joystickDirection = Vector3.zero;
            }
        }

        public void GetJoystickDirectionAndMagnitude(out Vector3 direction, out float magnitude)
        {
            direction = _joystickDirection;

            magnitude = (_touchCurrentPos - _touchStartPos).magnitude * _joystickData.JoystickSensitivity;
            if (magnitude > _joystickData.JoystickMaxMagnitude)
                magnitude = _joystickData.JoystickMaxMagnitude;
        }
    }
}