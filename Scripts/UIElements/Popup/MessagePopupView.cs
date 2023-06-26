using System;
using DG.Tweening;
using Managers.Interfaces;
using TMPro;
using UnityEngine;

namespace UIElements.Popup
{
    public class MessagePopupView : MonoBehaviour, IUIPopup
    {
        [SerializeField] private Transform[] _pointAniamtion;

        [SerializeField] private Transform _media;

        public bool IsActive => gameObject.activeSelf;
        public void Show()
        {
            gameObject.SetActive(true);
            AnimationShowPopup();
        }

        public void Show(Action callback)
        {
            Show();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Reset();
        }

        public void Reset()
        {
            
        }
        private Sequence AnimationShowPopup()
        {
            var seq = DOTween.Sequence();
            _media.position = _pointAniamtion[0].position;
            const float duration = 0.5f;
            seq.Append(_media.DOMove(_pointAniamtion[1].position, duration / 2));
            seq.Append(_media.DOMove(_pointAniamtion[2].position, duration / 3));

            return seq;
        }

    }
}
