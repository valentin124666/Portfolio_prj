using UnityEngine;

namespace Gladiators.Interfaces
{
    public interface ISoundCustom
    {
        void Play();
        void Stop();
        void Pause();
        void SetVolume(float volumePower);
    }
}
