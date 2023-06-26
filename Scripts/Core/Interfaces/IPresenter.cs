using UnityEngine;

namespace Core.Interfaces
{
    public interface IPresenter {
        bool IsActive { get; }
        void SetParent(Transform parent);
        void SetActive(bool active);
        void Translate(Vector3 pos);
        void Destroy();
    }
}
