using System;
using UnityEngine;
using UnityEngine.UI;

namespace Level.Interactive
{
    public class Timer : MonoBehaviour
    {
        private Action _timerCallback;

        [SerializeField] private Image _timerImage;

        [SerializeField] private float _timeItem;
        private float _timerItem;

        public bool IsActive => gameObject.activeSelf;

        private float _revert;

        private bool _isPause;

        private void UpdateCustom()
        {
            if (!_timerImage.gameObject.activeSelf || _isPause) return;

            if (_timerItem > 0)
            {
                TakeTime(Time.deltaTime);
            }
            else
            {
                _timerCallback?.Invoke();
                ResetTimer();
            }
        }

        public void SetActive(bool isActive)
        {
            _timerImage.gameObject.SetActive(isActive);
            _isPause = !isActive;
        }

        public void TakeTime(float number)
        {
            _timerItem -= number;
            _timerImage.fillAmount = Mathf.Abs(_revert - _timerItem / _timeItem);
        }

        public void ResetTimer()
        {
            _timerCallback = null;
            MainApp.Instance.LateUpdateEvent -= UpdateCustom;
        }

        public void StartTimer(Action callback, float time = 0, bool isRevert = false)
        {
            _isPause = false;

            _revert = isRevert ? 1 : 0;

            MainApp.Instance.LateUpdateEvent += UpdateCustom;
            var timeItem = time > 0 ? time : _timeItem;
            _timeItem = timeItem;
            _timerItem = timeItem;
            _timerImage.fillAmount = 1;
            _timerCallback = callback;
        }
    }
}