using System;
using Core;
using Data;
using DG.Tweening;
using Gladiators.Interfaces;
using Managers.Controller;
using Managers.Interfaces;
using Settings;
using UnityEngine;

namespace Gladiators.Player
{
    public class PlayerMoveView : MonoBehaviour, IPlayerModule, IStateModule
    {
        public event Action<float> MoveEvent;
        public Enumerators.UpdateType UpdateType => Enumerators.UpdateType.FixedUpdate;

        [SerializeField] private Rigidbody _rigidbody;
        private CameraController _cameraController;
        private JoystickController _joystickController;

        private float _playerMovementSpeed;

        private bool _isMoveJoystick;

        public void Init(PlayerController controller)
        {
            _cameraController = GameClient.Get<IGameplayManager>().GetController<CameraController>();
            _joystickController = GameClient.Get<IGameplayManager>().GetController<JoystickController>();

            _playerMovementSpeed = controller.PlayerMovementSpeed;
        }
        
        private void AddForce(Vector3 directionForce)
        {
            _rigidbody.AddForce(directionForce * _playerMovementSpeed, ForceMode.Force);
        }
        
        private void Rotate(Quaternion rotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.5f);
        }

        public void UpdateCustom()
        {
            if (!_isMoveJoystick) return;
            
            _joystickController.GetJoystickDirectionAndMagnitude(out Vector3 joystickDirection, out float magnitude);

            if (joystickDirection != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(_cameraController.RotationAxisY * joystickDirection);

                Rotate(targetRotation);
            }

            if (magnitude >= 0.5)
            {

                AddForce(_cameraController.RotationAxisY * (magnitude * joystickDirection));
            }

            var speedAnimations = joystickDirection != Vector3.zero ? magnitude : 0;
            MoveEvent?.Invoke(speedAnimations);
        }

        public Tween MovingAPlace(Transform place, float duration)
        {
            if (_isMoveJoystick)
            {
                Debug.LogError("Joystick movement not disabled");
                return null;
            }

            var eulerAngles = place.eulerAngles;
            var durationRot = duration / 2;
            transform.DORotate(eulerAngles, durationRot);
            return transform.DOMove(place.position, durationRot);
        }
        
        public void StartMovement()
        {
            if (_isMoveJoystick)
                return;

            _isMoveJoystick = true;
        }

        public void EndMovement()
        {
            if (!_isMoveJoystick)
                return;

            _isMoveJoystick = false;

            _rigidbody.velocity = Vector3.zero;
        }
    }
}