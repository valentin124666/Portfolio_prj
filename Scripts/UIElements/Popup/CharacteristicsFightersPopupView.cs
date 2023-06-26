using System;
using System.Globalization;
using Core;
using DG.Tweening;
using Managers.Controller;
using Managers.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements.Popup
{
    public class CharacteristicsFightersPopupView : MonoBehaviour, IUIPopup
    {
        [SerializeField] private Transform[] _pointAniamtion;
        [SerializeField] private Transform _media;

        [SerializeField] private Button _buttonOk;
        [SerializeField] private TMP_Text _nameNpc;

        [SerializeField] private Characteristics _characteristicsNPC;
        [SerializeField] private Characteristics _characteristicsPlayer;

        public bool IsActive => gameObject.activeSelf;
        public void Show()
        {
            gameObject.SetActive(true);
            FillTheData();
            AnimationShowPopup().OnComplete(() => { _buttonOk.onClick.AddListener(Hide); });
        }

        public void Show(Action callback)
        {
            gameObject.SetActive(true);
            FillTheData();
            AnimationShowPopup().OnComplete(() =>
            {
                _buttonOk.onClick.AddListener(() =>
                {
                    callback?.Invoke();
                    Hide();
                });
            });
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Reset();
        }

        public void Reset()
        {
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

        private void FillTheData()
        {
            var player = GameClient.Get<IGameplayManager>().GetController<PlayerController>();
            _characteristicsPlayer.Attack.text = player.FightPlayerDamage.ToString(CultureInfo.InvariantCulture);
            _characteristicsPlayer.Defense.text = player.FightPlayerArmor.ToString(CultureInfo.InvariantCulture);
            _characteristicsPlayer.Health.text = player.FightPlayerHealth.ToString(CultureInfo.InvariantCulture);
          
            var npc = GameClient.Get<IGameplayManager>().GetController<NPCController>();
            _nameNpc.text = npc.FightName;
            _characteristicsNPC.Attack.text = npc.FightNPCAttack.ToString(CultureInfo.InvariantCulture);
            _characteristicsNPC.Defense.text = npc.FightNPCArmor.ToString(CultureInfo.InvariantCulture);
            _characteristicsNPC.Health.text = npc.FightNPCHealth.ToString(CultureInfo.InvariantCulture);
            
        }

        [Serializable]
        private class Characteristics
        {
            public TMP_Text Health;
            public TMP_Text Defense;
            public TMP_Text Attack;
        }

    }
}
