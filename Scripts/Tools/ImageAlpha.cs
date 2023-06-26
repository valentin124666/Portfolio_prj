using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    public class ImageAlpha : MonoBehaviour
    {
        [SerializeField] private Image[] _images;
        [SerializeField] private TMP_Text[] _texts;

        private Dictionary<GameObject, float> _colorsAlpha;

        [SerializeField] private float _time;

        private void Awake()
        {
            _colorsAlpha = new Dictionary<GameObject, float>();
            foreach (var image in _images)
            {
                _colorsAlpha.Add(image.gameObject, image.color.a);
            }

            foreach (var text in _texts)
            {
                _colorsAlpha.Add(text.gameObject, text.color.a);
            }
        }

        public void Hide(bool isFast)
        {
            if(_colorsAlpha==null) return;

            if (isFast)
            {
                foreach (var image in _images)
                {
                    var color = image.color;
                    color.a = 0;
                    image.color = color;
                }

                foreach (var text in _texts)
                {
                    var color = text.color;
                    color.a = 0;
                    text.color = color;
                }
            }
            else
            {
                foreach (var image in _images)
                {
                    image.DOFade(0, _time);
                }

                foreach (var text in _texts)
                {
                    text.DOFade(0, _time);
                }
            }
        }

        public void Show(bool isFast)
        {
            if(_colorsAlpha==null) return;
        
            if (isFast)
            {
                foreach (var image in _images)
                {
                    var color = image.color;
                    color.a = _colorsAlpha[image.gameObject];
                    image.color = color;
                }

                foreach (var text in _texts)
                {
                    var color = text.color;
                    color.a = _colorsAlpha[text.gameObject];
                    text.color = color;
                }
            }
            else
            {
                foreach (var image in _images)
                {
                    image.DOFade(_colorsAlpha[image.gameObject], _time);
                }

                foreach (var text in _texts)
                {
                    text.DOFade(_colorsAlpha[text.gameObject], _time);
                }
            }
        }
    }
}