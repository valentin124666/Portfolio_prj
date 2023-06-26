using System;
using Core;
using Managers.Interfaces;
using Settings;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements.Popup
{
    public class ArenaPopupLoseView : MonoBehaviour, IUIPopup
    {
        [SerializeField] private Transform[] _pointAniamtion;
        [SerializeField] private Button _buttonYes;
        [SerializeField] private Transform _media;

        public bool IsActive => gameObject.activeSelf;

        public void Show()
        {
            gameObject.SetActive(true);
            AnimationShowPopup().OnComplete(Activation);
        }

        public void Show(Action callback)
        {
            gameObject.SetActive(true);
            AnimationShowPopup().OnComplete(Activation);
            Activation();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Reset();
        }

        public void Reset()
        {
            _buttonYes.onClick.RemoveAllListeners();
        }

        private void Activation()
        {
            _buttonYes.onClick.AddListener(() => GameClient.Instance.GetService<IGameplayManager>().ChangeAppState(Enumerators.AppState.InGameSquare));
        }

        private Sequence AnimationShowPopup()
        {
            var seq = DOTween.Sequence();
            _media.position = _pointAniamtion[0].position;
            float duration = 0.5f;
            seq.Append(_media.DOMove(_pointAniamtion[1].position, duration / 2));
            seq.Append(_media.DOMove(_pointAniamtion[2].position, duration / 4));
            seq.Append(_media.DOShakePosition( duration / 4,new Vector3(0,10,0)));

            return seq;
        }
    }
}