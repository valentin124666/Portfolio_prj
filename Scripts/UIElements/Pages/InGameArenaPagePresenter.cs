using System;
using DG.Tweening;
using Managers.Interfaces;
using Settings;
using static Settings.Enumerators;

namespace UIElements.Pages
{
    public class InGameArenaPagePresenter : SimpleUIPresenter<InGameArenaPagePresenter, InGameArenaPagePresenterView>, IUIElement
    {
        public InGameArenaPagePresenter(InGameArenaPagePresenterView view) : base(view)
        {
            GetTouchAction(ClickArenaState.Attack).onPointerDown += DisablePressAttack;
            GetTouchAction(ClickArenaState.Protection).onPointerDown += DisablePressProtection;
            GetTouchAction(ClickArenaState.DodgeRight).onPointerDown += DisablePressDodgeRight;
            GetTouchAction(ClickArenaState.DodgeLeft).onPointerDown += DisablePressDodgeLeft;
            SetActionClicks(true);
        }

        public void SetHP(float hpBar) => View.SetHP(hpBar);
        public void SetProtection(float hpBar) => View.SetProtection(hpBar);
        public Tween ShowDamageScreen(float duration) => View.ShowDamageScreen(duration);
        public void HideDamageScreen(float duration) => View.HideDamageScreen(duration);

        public void SetActiveDisplay(bool isActive)
        {
            if (isActive)
            {
                View.imageAlpha.Show(false);
            }
            else
            {
                View.imageAlpha.Hide(false);
            }
        }

        public void SetActiveDisplayFast(bool isActive)
        {
            if (isActive)
            {
                View.imageAlpha.Show(true);
            }
            else
            {
                View.imageAlpha.Hide(true);
            }
        }

        public TouchAction GetTouchAction(ClickArenaState type)
        {
            TouchAction touchAction;
            switch (type)
            {
                case ClickArenaState.Attack:
                    touchAction = View.ArenaTouchAction.Attack;
                    break;
                case ClickArenaState.Protection:
                    touchAction = View.ArenaTouchAction.Protection;

                    break;
                case ClickArenaState.DodgeRight:
                    touchAction = View.ArenaTouchAction.DodgeRight;

                    break;
                case ClickArenaState.DodgeLeft:
                    touchAction = View.ArenaTouchAction.DodgeLeft;

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return touchAction;
        }

        private void DisablePressAttack()
        {
            DisablePress(ClickArenaState.Attack);
        }

        private void DisablePressProtection()
        {
            DisablePress(ClickArenaState.Protection);
        }

        private void DisablePressDodgeRight()
        {
            DisablePress(ClickArenaState.DodgeRight);
        }

        private void DisablePressDodgeLeft()
        {
            DisablePress(ClickArenaState.DodgeLeft);
        }

        private void DisablePress(ClickArenaState state)
        {
            var touch = GetTouchAction(state);

            if (touch.OffClick)
                return;

            touch.OffClick = true;
            View.ActivationTimer(() => touch.OffClick = false, state);
        }

        public void SetActionClicks(bool isAction)
        {
            View.ArenaTouchAction.SetActivePressedAll(isAction);
        }

        public void SetActionBottom(ClickArenaState clickArenaState, bool isActive)
        {
            View.ArenaTouchAction.SetActivePressed(clickArenaState, isActive);
            View.SetActiveTimer(clickArenaState, isActive);
        }

        public void Show()
        {
            SetActive(true);
            View.Show();
        }

        public void Hide()
        {
            SetActive(false);
            Reset();
        }

        public void Reset()
        {
            HideDamageScreen(0);
            SetActionClicks(true);
            View.SetActiveTimer(ClickArenaState.Protection, true);

            SetActiveDisplayFast(true);
        }
    }
}