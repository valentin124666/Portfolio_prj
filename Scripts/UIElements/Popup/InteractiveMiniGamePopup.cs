using System;
using DG.Tweening;
using Managers.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UIElements.Popup
{
    public class InteractiveMiniGamePopup : MonoBehaviour, IUIPopup, IPointerClickHandler
    {
        [SerializeField] private Transform _bottom;
        [SerializeField] private TMP_Text _message;

        private int _removeMessageVia;

        public event Action ClickEvent;

        public bool IsActive => gameObject.activeSelf;
        private bool _isClick;
        
        public void Show()
        {
            _removeMessageVia = 3;
            _message.transform.localScale = Vector3.one;
            
            gameObject.SetActive(true);
            _isClick = true;
        }

        public void Show(Action callback)
        {
            _removeMessageVia = 3;
            _message.transform.localScale = Vector3.one;

            gameObject.SetActive(true);
            _isClick = true;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Reset();
        }


        public void Reset()
        {
            _isClick = false;
            _bottom.DOKill();
            _bottom.localScale = Vector3.one;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isClick)
                return;
            
            _isClick = false;
            _bottom.DOKill();
            _bottom.localScale = Vector3.one;
           
            var blink = 0.2f;
            var scaleAimSmall = 0.9f;

            _bottom.DOScale(new Vector3(scaleAimSmall, scaleAimSmall, scaleAimSmall), blink / 2).OnComplete(
                () => transform.DOScale(Vector3.one, blink / 2).OnComplete(() => _isClick = true)
            );

            ClickEvent?.Invoke();
            if(_removeMessageVia==0)
            {
                _message.transform.DOScale(0, 0.3f);
            }

            _removeMessageVia--;
        }
    }
}