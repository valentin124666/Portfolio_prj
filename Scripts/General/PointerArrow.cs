using DG.Tweening;
using UnityEngine;

namespace General
{
    public class PointerArrow : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        private static readonly int _reset = Animator.StringToHash("Reset");

        public void Reset()
        {
            _animator.SetTrigger(_reset);
            SetActive(false);
        }

        public void SetPositionAndRotation(Vector3 pos, Quaternion rot) => transform.SetPositionAndRotation(pos, rot);
        public void SetActive(bool isActive) => gameObject.SetActive(isActive);

        public void Appearance()
        {
            transform.DOKill();
            Vector3 scale = new Vector3(1.5f,1.5f,1.5f);
            
            transform.localScale = Vector3.zero;

            
            transform.DOScale(scale, 1f).SetEase(Ease.OutBack);
            
        }
    }
}