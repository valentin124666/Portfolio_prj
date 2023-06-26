using System;
using Core;
using DG.Tweening;
using Managers.Controller;
using Managers.Interfaces;
using Settings;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace UIElements.Pages
{
    [PrefabInfo(Enumerators.NamePrefabAddressable.InGameSquarePage)]
    public class InGameSquarePagePresenterView : SimpleUIPresenterView<InGameSquarePagePresenter, InGameSquarePagePresenterView>
    {
        [SerializeField] private TMP_Text _counterCoin;
        [SerializeField] private TMP_Text _counterLevel;

        [SerializeField] private Feature _counterHealth;
        [SerializeField] private Feature _counterAttack;
        [SerializeField] private Feature _counterDefense;
        
        [SerializeField] private RectTransform _characteristics;

        public void UpdateCoinCounter(int coin) => _counterCoin.text = coin.ToString();
        public void UpdateLevelCounter(int level) => _counterLevel.text = level.ToString();

        public void UpdateCharacteristics(int health, int attack, int defense, bool showDifference = false)
        {
            if (showDifference)
            {
                _counterHealth.ShowDifference(health);
                _counterAttack.ShowDifference(attack);
                _counterDefense.ShowDifference(defense);
            }

            _counterHealth.SetCharacteristic(health);
            _counterAttack.SetCharacteristic(attack);
            _counterDefense.SetCharacteristic(defense);
        }

        public void ShowCharacteristics()
        {
            _characteristics.DOKill();
            _characteristics.DOAnchorPosX(0, 0.7f);
        }

        public void HideCharacteristics()
        {
            _characteristics.DOKill();
            _characteristics.DOAnchorPosX(-_characteristics.rect.width, 0.6f);
        }

        public void AddCoin()
        {
            GameClient.Get<IGameplayManager>().GetController<PlayerController>().AddCoin(100);
        }

        [Serializable]
        private class Feature
        {
            [SerializeField] private TMP_Text counter;
            [SerializeField] private TMP_Text added;
            [SerializeField] private BounceUIElements _bounce;
            private int current;

            public void ShowDifference(int characteristic)
            {
                var difference = characteristic - current;
                if (difference <= 0) return;
                added.gameObject.SetActive(true);
              
                _bounce.Bounce();
              
                added.text = "+" + difference;
                InternalTools.ActionDelayed(() => { added.gameObject.SetActive(false); }, 1);
            }

            public void SetCharacteristic(int characteristic)
            {
                current = characteristic;
                counter.text = characteristic.ToString();
            }
        }
    }
}