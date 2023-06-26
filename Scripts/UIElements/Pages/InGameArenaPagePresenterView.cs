using System;
using System.Collections.Generic;
using Core;
using Data;
using DG.Tweening;
using General;
using Level.Interactive;
using Managers.Controller;
using Managers.Interfaces;
using Tools;
using UnityEngine;
using UnityEngine.UI;
using static Settings.Enumerators;

namespace UIElements.Pages
{
    [PrefabInfo(NamePrefabAddressable.InGameArenaPage)]
    public class InGameArenaPagePresenterView : SimpleUIPresenterView<InGameArenaPagePresenter, InGameArenaPagePresenterView>
    {
        public ArenaTouchAction ArenaTouchAction;
        
        [SerializeField] private List<TimerType> _timers;

        [SerializeField] private Image _HPBar;
        [SerializeField] private Image _protectionBar;
        [SerializeField] private Image _damageScreen;
        [SerializeField] private ImageAlpha _imageAlpha;

        public ImageAlpha imageAlpha => _imageAlpha;

        private float _colorAlpha;

        private BattleData _battleData;

        public override void Init()
        {
            base.Init();
            _battleData = GameClient.Get<IGameDataManager>().GetDataScriptable<BattleData>();
            
            _colorAlpha = _damageScreen.color.a;
            _damageScreen.DOFade(0, 0);
            
            foreach (var timer in _timers)
            {
                timer.timer.ResetTimer();
                timer.timer.gameObject.SetActive(false);
            }

            _HPBar.fillAmount = 1;
            _protectionBar.fillAmount = 1;
            _imageAlpha.Hide(true);
        }
        
        public Tween ShowDamageScreen(float duration) =>_damageScreen.DOFade(_colorAlpha, duration);
        public Tween HideDamageScreen(float duration) =>_damageScreen.DOFade(0, duration);
        public void SetHP(float hpBar)=>_HPBar.fillAmount = hpBar;
        public void SetProtection(float hpBar) => _protectionBar.fillAmount = hpBar;

        public void Show()
        {
            _HPBar.fillAmount = 1;
            _protectionBar.fillAmount = 1;
        }

        public void SetActiveTimer(ClickArenaState type, bool isActive)
        {
            var timerType = _timers.Find(item => item.type == type);
            timerType.timer.SetActive(isActive);
        }
        
        public void ActivationTimer(Action callback, ClickArenaState type)
        {
            var timerType = _timers.Find(item => item.type == type);

            if (timerType == null)
            {
                Debug.LogError("[InGameArenaPagePresenterView] [ActivationTimer]: there is no such timer");
                callback?.Invoke();
                return;
            }

            timerType.timer.gameObject.SetActive(true);

            float time = 0;
            switch (type)
            {
                case ClickArenaState.Attack:
                    time = _battleData.CooldownAttack;
                    break;
                case ClickArenaState.Protection:
                    time = _battleData.CooldownProtection;
                    break;
                case ClickArenaState.DodgeLeft:
                    time = _battleData.CooldownDodge;
                    break;
                case ClickArenaState.DodgeRight:
                    time = _battleData.CooldownDodge;
                    break;
            }

            timerType.timer.StartTimer(() =>
            {
                timerType.timer.gameObject.SetActive(false);
                callback?.Invoke();
            }, time, true);
        }

        [Serializable]
        private class TimerType
        {
            public ClickArenaState type;
            public Timer timer;
        }
    }
}