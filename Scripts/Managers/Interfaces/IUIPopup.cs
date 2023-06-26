using System;

namespace Managers.Interfaces
{
    public interface IUIPopup
    {
        bool IsActive { get; }
        void Show();
        void Show(Action callback);
        void Hide();
        void Reset();
    }
}