using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIElements
{
    public class TouchAction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action onPointerDown;
        public event Action onPointerUp;

        [SerializeField] private Image[] _images;

        public bool OffClick;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (OffClick )
                return;

            var blink = 0.4f;

            var scaleAimSmall = 0.9f;

            transform.DOScale(new Vector3(scaleAimSmall, scaleAimSmall, scaleAimSmall), blink / 2).OnComplete(
                () => transform.DOScale(Vector3.one, blink / 2)
            );

            onPointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (OffClick )
                return;

            onPointerUp?.Invoke();
        }

        public void SetClickTracking(bool isTracking)
        {
            foreach (var image in _images)
            {
                image.raycastTarget = isTracking;
            }
        }
    }
}