using UnityEngine;

namespace Managers.Interfaces
{
    public interface IUIManager
    {
        IUIElement CurrentPage { get; }
        void SetPage<T>(bool hideAll = false) where T : IUIElement;
        T GetPage<T>() where T : IUIElement;
        void DrawPopup<T>() where T : IUIPopup;
        void HidePopup<T>() where T : IUIPopup;
        T GetPopup<T>() where T : IUIPopup;

        void ResetAll();

        void HideAllPages();
        void HideAllPopups();

    }
}
