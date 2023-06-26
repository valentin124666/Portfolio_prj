using UnityEngine;

namespace Level
{
    public interface ILevel
    {
        Transform GetCameraAnchor();
        Transform GetPosPlayerSpawn();
        void SetActive(bool active);
        void OnDestroy();
    }
}
