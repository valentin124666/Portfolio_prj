using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Tools
{
    public class BounceUIElements : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;

        [SerializeField] private List<Vector2> _scales;
        [SerializeField] private float _time;

        private void Awake()
        {
            if(_rectTransform==null) _rectTransform= gameObject.GetComponent<RectTransform>();
        
            _scales.Add(_rectTransform.localScale);
        }

        public Sequence Bounce()
        {
            var seq = DOTween.Sequence();
            foreach (var item in _scales)
            {
                seq.Append(_rectTransform.DOScale(item, _time));
            }

            return seq;
        }
    
    }
}
