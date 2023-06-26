using System;
using Core;
using Data;
using DG.Tweening;
using Managers.Interfaces;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements.Popup
{
    public class ColiseumPopupView : MonoBehaviour, IUIPopup
    {   
        [SerializeField] private Transform[] _pointAniamtion;

        [SerializeField] private TMP_Text _massage;

        [SerializeField] private Button _buttonYes;
        [SerializeField] private Button _buttonNo;
        
        [SerializeField] private Transform _media;

        public bool IsActive => gameObject.activeSelf;

        public void Show()
        {
            gameObject.SetActive(true);
            _massage.text = GameClient.Get<IGameDataManager>().GetDataScriptable<UIData>().coliseumPopupData.Massage;

            AnimationShowPopup().OnComplete(Activation);
        }

        public void Show(Action callback)
        {
            gameObject.SetActive(true);
            _massage.text = GameClient.Get<IGameDataManager>().GetDataScriptable<UIData>().coliseumPopupData.Massage;

            AnimationShowPopup().OnComplete(Activation);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Reset();
        }

        public void Reset()
        {
            _buttonYes.onClick.RemoveAllListeners();
            _buttonNo.onClick.RemoveAllListeners();
        }

        private void Activation()
        {
            _buttonYes.onClick.AddListener(() => GameClient.Instance.GetService<IGameplayManager>().ChangeAppState(Enumerators.AppState.InGameArena));
            _buttonNo.onClick.AddListener(Hide);
        }
        private Sequence AnimationShowPopup()
        {
            var seq = DOTween.Sequence();
            _media.position = _pointAniamtion[0].position;
            var duration = 0.5f;
            seq.Append(_media.DOMove(_pointAniamtion[1].position, duration / 2));
            seq.Append(_media.DOMove(_pointAniamtion[2].position, duration / 3));

            return seq;
        }

    }
}