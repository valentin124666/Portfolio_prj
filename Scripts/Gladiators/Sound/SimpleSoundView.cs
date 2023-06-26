using System.Collections.Generic;
using Settings;
using UnityEngine;

namespace Gladiators.Sound
{
    public class SimpleSoundView : MonoBehaviour
    {
        [SerializeField] protected List<SoundCustom> _soundCustoms;

        public void Init()
        {
            foreach (var item in _soundCustoms)
            {
                item.Init();
            }
        }

        public void PlayRandomSound()
        {
            StopAll();
            _soundCustoms[Random.Range(0, _soundCustoms.Count)].Play();
        }

        public void PlaySound(Enumerators.SoundName name)
        {
            _soundCustoms.Find(sound => sound.SoundName == name)?.Play();
        }

        public void StopSound(Enumerators.SoundName name)
        {
            _soundCustoms.Find(sound => sound.SoundName == name)?.Stop();
        }

        public void StopAll()
        {
            foreach (var sound in _soundCustoms)
            {
                sound.Stop();
            }
        }
    }
}