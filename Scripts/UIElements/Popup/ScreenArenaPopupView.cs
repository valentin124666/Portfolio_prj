using System;
using Core;
using DG.Tweening;
using Managers.Interfaces;
using Settings;
using UnityEngine;

namespace UIElements.Popup
{
    public class ScreenArenaPopupView : MonoBehaviour, IUIPopup
    {
        [SerializeField] private RectTransform[] _points;

        [SerializeField] private RectTransform _foldingScreen;
        public bool IsActive => _isActive;
        public bool _isActive;

        public void Show()
        {
            gameObject.SetActive(true);

            MoveFoldingScreen(_points[0],MainApp.Instance.FoldingScreenSpeed ).OnComplete(() =>
            {
                _isActive = true;
            });
        }

        public void Show(Action callback)
        {
            gameObject.SetActive(true);

            MoveFoldingScreen(_points[0],MainApp.Instance.FoldingScreenSpeed ).OnComplete(() =>
            {

                _isActive = true;
                callback?.Invoke();
            });
        }

        public void Hide()
        {
            float multiplier = GameClient.Get<IGameplayManager>().CurrentState == Enumerators.AppState.InGameArena ? 4 : 1;

            MoveFoldingScreen(_points[1],MainApp.Instance.FoldingScreenSpeed * multiplier).OnComplete(() =>
            {
                gameObject.SetActive(false);
                _isActive = false;
            });
        }

        private Tween MoveFoldingScreen(RectTransform target,float time)
        {
            _foldingScreen.DOKill();

            return _foldingScreen.DOMove(target.position, time);
        }

        public void Reset()
        {
            _foldingScreen.DOKill();
            _foldingScreen.position = _points[0].position;
        }
    }
}