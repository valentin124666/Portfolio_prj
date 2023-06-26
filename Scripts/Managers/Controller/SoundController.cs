using System.Collections.Generic;
using System.Linq;
using Gladiators.Interfaces;
using Managers.Interfaces;
using Settings;

namespace Managers.Controller
{
    public class SoundController : IController
    {
        public bool IsInit { get; private set; }
        private readonly Dictionary<Enumerators.SoundType,List<ISoundCustom>> _sounds = new();

        public void Init()
        {
            IsInit = true;
        }
        public void RegistrationInController(Enumerators.SoundType type,ISoundCustom animatorCustom)
        {
            if (!_sounds.ContainsKey(type))
            {
                _sounds[type] = new List<ISoundCustom>();
            }
            _sounds[type].Add(animatorCustom);
        }

        public void SetVolume(float volumePower)
        {
            foreach (var sound in _sounds.SelectMany(item => item.Value))
            {
                sound.SetVolume(volumePower);
            }
        }
    }
}
