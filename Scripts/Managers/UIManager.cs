using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Interfaces;
using Cysharp.Threading.Tasks;
using Managers.Interfaces;
using Settings;
using UIElements.Pages;
using UIElements.Popup;
using UnityEngine;

namespace Managers
{
    public class UIManager : IService, IUIManager
    {
        private List<IUIElement> _uiPages;
        private List<IUIPopup> _uiPopups;

        private GameObject _canvas { get; set; }
        public IUIElement CurrentPage { get; private set; }

        public async UniTask Init()
        {
            _canvas = MainApp.Instance.Canvas;

            _uiPages = new List<IUIElement>();

            _uiPopups = new List<IUIPopup>();
            await CreateUI();

            HideAllPopups();
            HideAllPages();
        }

        private async UniTask CreateUI()
        {
            _uiPages.Add(await ResourceLoader.Instantiate<InGameArenaPagePresenter, InGameArenaPagePresenterView>(_canvas.transform, ""));
            _uiPages.Add(await ResourceLoader.Instantiate<InGameSquarePagePresenter, InGameSquarePagePresenterView>(_canvas.transform, ""));

            _uiPopups.Add(await ResourceLoader.Instantiate<JoystickPopupView>(Enumerators.NamePrefabAddressable.Stick.ToString(), _canvas.transform));
            _uiPopups.Add(await ResourceLoader.Instantiate<InteractiveMiniGamePopup>(Enumerators.NamePrefabAddressable.InteractiveMiniGamePopup.ToString(), _canvas.transform));
            _uiPopups.Add(await ResourceLoader.Instantiate<ArenaPopupWinView>(Enumerators.NamePrefabAddressable.ArenaPopup.ToString(), _canvas.transform));
            _uiPopups.Add(await ResourceLoader.Instantiate<ArenaPopupLoseView>(Enumerators.NamePrefabAddressable.ArenaPopupLose.ToString(), _canvas.transform));
            _uiPopups.Add(await ResourceLoader.Instantiate<ColiseumPopupView>(Enumerators.NamePrefabAddressable.ColiseumPopup.ToString(), _canvas.transform));
            _uiPopups.Add(await ResourceLoader.Instantiate<ScreenArenaPopupView>(Enumerators.NamePrefabAddressable.FoldingScreenArena.ToString(), _canvas.transform));
            _uiPopups.Add(await ResourceLoader.Instantiate<GymPopupView>(Enumerators.NamePrefabAddressable.GymPopup.ToString(), _canvas.transform));
            _uiPopups.Add(await ResourceLoader.Instantiate<UpdateAmmunitionPopupView>(Enumerators.NamePrefabAddressable.UpdateAmmunitionPopup.ToString(), _canvas.transform));
            _uiPopups.Add(await ResourceLoader.Instantiate<MessagePopupView>(Enumerators.NamePrefabAddressable.MessagePopup.ToString(), _canvas.transform));
            _uiPopups.Add(await ResourceLoader.Instantiate<CharacteristicsFightersPopupView>(Enumerators.NamePrefabAddressable.CharacteristicsFightersPopup.ToString(), _canvas.transform));
        }

        public void ResetAll()
        {
            foreach (var page in _uiPages)
                page.Reset();

            foreach (var popup in _uiPopups)
                popup.Reset();
        }

        public T GetPage<T>() where T : IUIElement
        {
            IUIElement page = null;
            foreach (var _page in _uiPages.OfType<T>())
            {
                page = _page;
                break;
            }

            return (T)page;
        }

        public T GetPopup<T>() where T : IUIPopup
        {
            IUIPopup popup = null;
            foreach (var _popup in _uiPopups.OfType<T>())
            {
                popup = _popup;
                break;
            }

            return (T)popup;
        }

        public void SetPage<T>(bool hideAll = false) where T : IUIElement
        {
            if (hideAll)
            {
                HideAllPages();
            }
            else
            {
                CurrentPage?.Hide();
            }

            foreach (var _page in _uiPages.OfType<T>())
            {
                CurrentPage = _page;
                break;
            }

            CurrentPage.Show();
        }

        public void DrawPopup<T>() where T : IUIPopup
        {
            IUIPopup popup = null;
            foreach (var _popup in _uiPopups.OfType<T>())
            {
                popup = _popup;
                break;
            }

            popup.Show();
        }

        public void HidePopup<T>() where T : IUIPopup
        {
            foreach (var _popup in _uiPopups.OfType<T>())
            {
                _popup.Hide();
                break;
            }
        }

        public void HideAllPages()
        {
            foreach (var _page in _uiPages)
            {
                _page.Hide();
            }
        }

        public void HideAllPopups()
        {
            foreach (var _popup in _uiPopups)
            {
                _popup.Hide();
            }
        }
    }
}