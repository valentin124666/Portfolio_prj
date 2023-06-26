using System;
using System.Globalization;
using Data.Characteristics;
using DG.Tweening;
using General;
using Managers.Interfaces;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements.Popup
{
    public class UpdateAmmunitionPopupView : MonoBehaviour, IUIPopup
    {
    
        [SerializeField] private Transform[] _pointAniamtion;
        [SerializeField] private Transform _media;
    
        [SerializeField] private Button _buttonOk;

        [SerializeField] private ModelSelection _sword;
        [SerializeField] private ModelSelection _shield;
        [SerializeField] private ModelSelection _message;
        
        [SerializeField] private TMP_Text _textMessageUpdate;
        [SerializeField] private TMP_Text _textPower;
    
        public bool IsActive => gameObject.activeSelf;

        public void Show()
        {
            gameObject.SetActive(true);
            AnimationShowPopup().OnComplete(Activation);

        }

        public void Show(Action callback)
        {
            Show();
        }
        
        private void Activation()
        {
            _buttonOk.onClick.AddListener(Hide);
        }

        public void SetUpgrade(Enumerators.ReceptacleObjectType typeAmmunition,EquipmentCharacteristics data)
        {
            _textMessageUpdate.text = "New " + typeAmmunition;
            _textPower.text = data.power.ToString(CultureInfo.InvariantCulture);
        
            _message.SetModel((int)typeAmmunition);
            switch (typeAmmunition)
            {
                case Enumerators.ReceptacleObjectType.Sword:
                    _sword.SetModel(data.idMod);
                    break;
                case Enumerators.ReceptacleObjectType.Shield:
                    _shield.SetModel(data.idMod);
                    break;
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Reset();
        }

        public void Reset()
        {
            _shield.DisableEverything();
            _sword.DisableEverything();
            _buttonOk.onClick.RemoveAllListeners();
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
