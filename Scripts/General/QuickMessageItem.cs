using DG.Tweening;
using Settings;
using TMPro;
using UnityEngine;

namespace General
{
    public class QuickMessageItem : PoolItem
    {
        private readonly TMP_Text _message;

        private readonly float _time;
        private readonly float _distance;
    
        public QuickMessageItem(Enumerators.NamePrefabAddressable name, GameObject gameObject) : base(name, gameObject)
        {
            _message = gameObject.GetComponentInChildren<TMP_Text>();
            _time = 1f;
            _distance = 0.7f;
        }

        public override void Activation(params object[] args)
        {
            base.Activation(args);
            float target = transform.position.y + _distance;
            transform.DOMoveY(target, _time).OnComplete(ReturnToPool);
            _message.DOFade(0, _time);
        }

        public override void ReturnToPool()
        {
            base.ReturnToPool();
            _message.DOKill();
            _message.color = Color.white;
        }

        public void SetMessage(string message)
        {
            _message.SetText(message);
        }
    }
}
