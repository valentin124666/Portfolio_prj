using Gladiators.Interfaces;
using Managers.Controller;
using UnityEngine;
using static Settings.Enumerators;

namespace Gladiators.Sound
{
    public class SoundCustom : MonoBehaviour, ISoundCustom
    {
        [SerializeField] private SoundType _soundType;
        [SerializeField] private SoundName _soundName;
        public SoundName SoundName => _soundName;
        public bool IsInit { get; private set; }

        [SerializeField] private AudioSource _audioSource;
        private float _startVolume = 1;
        private readonly SoundController _soundController = new();

        public void Init()
        {
            _startVolume = _audioSource.volume;
            Registration();
            IsInit = true;
        }

        private void Registration()
        {
            _soundController.RegistrationInController(_soundType, this);
        }


        public void Play()
        {
            _audioSource.Play();
        }

        public void Stop()
        {
            _audioSource.Stop();
        }

        public void Pause()
        {
            _audioSource.Pause();
        }

        void ISoundCustom.SetVolume(float volumePower)
        {
            _audioSource.volume = _startVolume * volumePower;
        }
    }
}