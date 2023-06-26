using System.Linq;
using DG.Tweening;
using Settings;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using FighterState = Settings.Enumerators.FighterState;

namespace General
{
    public class MarkerFighterItem : PoolItem
    {
        public FighterState Type { get; private set; }
        public bool IsPressed { get; private set; }
        private Transform _marker;
        private Vector2 _localPos;

        // private readonly Image _duration;
        private readonly Image _attack;
        private readonly Image _protection;
        private Image _current;

        public float RotZ => transform.eulerAngles.z;

        public MarkerFighterItem(Enumerators.NamePrefabAddressable name, GameObject gameObject) : base(name, gameObject)
        {
            var images = gameObject.GetComponentsInChildren<Image>();
            
            // _duration = images.First(item => item.name == "Line");

            _attack = images.First(item => item.name == "Attack");
            _protection = images.First(item => item.name == "Protection");
            _localPos = _attack.transform.localPosition;

            EnableMeshMarker();
        }

        private void UpdateRotation()
        {
            // _current.transform.rotation = Quaternion.LookRotation(_current.transform.position - _camPos, Vector3.right);
            _current.transform.forward = Vector3.forward;
        }

        public void SetState(FighterState state, float duration)
        {
            IsPressed = true;

            transform.localPosition = Vector3.zero;
            transform.localRotation = quaternion.identity;
            transform.localScale = Vector3.one;

            _current?.gameObject.SetActive(false);
            switch (state)
            {
                case FighterState.Idle:
                    _current = _attack;

                    Type = FighterState.Attack;
                    // _duration.gameObject.SetActive(false);
                    break;
                case FighterState.Attack:

                    _current = _protection;

                    Type = FighterState.Protection;

                    // _duration.gameObject.SetActive(true);
                    //
                    // _duration.color = Color.blue;
                    // _duration.fillAmount = duration;
                    // _duration.transform.localScale = Vector3.one;

                    break;
                case FighterState.Protection:
                    _current = _attack;

                    Type = FighterState.Attack;
                    // _duration.gameObject.SetActive(false);
                    break;
            }

            if (_current != null)
            {
                _current.gameObject.SetActive(true);
                MainApp.Instance.FixedUpdateEvent += UpdateRotation;
            }
        }

        public void Pressed()
        {
            IsPressed = false;
            _current.transform.DOKill();
            _current.DOKill();

            var duration = 0.8f;
            _current.DOFade(0.1f, duration);
            float posY = _current.transform.position.y;
            _current.transform.DOMoveY(posY + 100f, duration).SetEase(Ease.OutSine).OnComplete(ReturnToPool);
        }
        public void Skipped()
        {
            _current.DOKill();

            var duration = 0.7f;
            _current.DOFade(0.1f, duration);
        }

        private void EnableMeshMarker()
        {
            MainApp.Instance.FixedUpdateEvent -= UpdateRotation;

            _attack.gameObject.SetActive(false);
            _protection.gameObject.SetActive(false);
        }

        public void OnCurrentMeshMarker()
        {
            MainApp.Instance.FixedUpdateEvent -= UpdateRotation;
            _current?.gameObject.SetActive(false);
        }

        public override void ReturnToPool()
        {
            base.ReturnToPool();
            _current.transform.DOKill();
            _current.DOKill();
            _current.color = Color.white;
            _current.transform.localPosition = _localPos;
            EnableMeshMarker();
        }
    }
}