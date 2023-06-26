using System;
using Core;
using Data;
using DG.Tweening;
using Managers.Interfaces;
using UnityEngine;

namespace Managers.Controller
{
    public class CameraController : IController
    {
        public bool IsActive;

        private PlayerController _playerController;

        private SkyPlane _skyPlane;

        private Transform _cameraTransform => CameraMain.transform;

        private Vector3 _offSet;
        private Vector3 _gamePosCamera;
        private Vector3 _velocity;

        private float _cameraMovementSpeed;

        public Camera CameraMain { get; private set; }

        public Quaternion RotationAxisY => Quaternion.Euler(0, CameraMain.transform.eulerAngles.y, 0);
        public Vector3 CameraPos => _cameraTransform.position;

        private bool _isLockMoveUpdate;

        public bool IsInit { get; private set; }


        public void Init()
        {
            CameraMain = Camera.main;
            _skyPlane = _cameraTransform.GetComponentInChildren<SkyPlane>();
            _skyPlane.Init();

            _isLockMoveUpdate = false;

            _playerController = GameClient.Get<IGameplayManager>().GetController<PlayerController>();
            _cameraMovementSpeed = GameClient.Instance.GetService<IGameDataManager>().GetDataScriptable<GameData>().cameraData.CameraMovementSpeed;
            _skyPlane.SetSizeByCamera();
            IsInit = true;
        }

        private Vector3 MoveCamera()
        {
            var currentPosCamera = (_playerController.PosPlayer + _offSet);
            return Vector3.SmoothDamp(_cameraTransform.position, currentPosCamera, ref _velocity, _cameraMovementSpeed);
        }

        public void Demonstrate(Transform[] targets, Action callback = null)
        {
            _isLockMoveUpdate = true;

            var oldRot = _cameraTransform.eulerAngles;

            var data = GameClient.Instance.GetService<IGameDataManager>().GetDataScriptable<GameData>().cameraData;

            var seq = DOTween.Sequence();
            for (var i = 0; i < targets.Length; i++)
            {
                var item = targets[i];
                seq.Append(_cameraTransform.DOMove(item.position, data.TutorMoveSpeed));
                seq.Insert((data.TutorMoveSpeed + data.TutorPause) * i, _cameraTransform.DORotate(item.eulerAngles, data.TutorRotSpeed));
                seq.AppendInterval(data.TutorPause);
            }

            seq.OnComplete(() =>
            {
                _cameraTransform.DOKill();
                _cameraTransform.DOMove((_playerController.PosPlayer + _offSet), data.TutorMoveSpeed / 2);
                _cameraTransform.DORotate(oldRot, data.TutorRotSpeed / 2);
                callback?.Invoke();
                _isLockMoveUpdate = false;
            });
        }

        public void ActivationCameraTrack(bool isAnchorMagnet = false)
        {
            if (IsActive)
            {
                ResetAll();
            }

            IsActive = true;
            var anchor = GameClient.Instance.GetService<IGameplayManager>().GetController<LevelController>().GetCameraAnchor();
            _cameraTransform.SetPositionAndRotation(anchor.position, anchor.rotation);

            if (isAnchorMagnet)
            {
                _cameraTransform.SetParent(anchor);
            }
            else
            {
                _offSet = _cameraTransform.position - _playerController.PosPlayer;

                MainApp.Instance.FixedUpdateEvent += FixedUpdate;
            }
        }

        public void SetFieldOfView(float fieldOfView)
        {
            CameraMain.fieldOfView = fieldOfView;
            _skyPlane.SetSizeByCamera();
        }

        private void FixedUpdate()
        {
            if (_isLockMoveUpdate) return;

            _cameraTransform.position = MoveCamera();
        }

        public void DOShake(float duration)
        {
            _cameraTransform.DOShakePosition(duration, new Vector3(0.1f, 0.1f, 0), randomnessMode: ShakeRandomnessMode.Harmonic);
        }

        public void SetChild(Transform child)
        {
            child.SetParent(_cameraTransform);
        }

        private void ResetAll()
        {
            _cameraTransform.SetParent(null);
            IsActive = false;
            _offSet = Vector3.zero;
            MainApp.Instance.FixedUpdateEvent -= FixedUpdate;
        }
    }
}