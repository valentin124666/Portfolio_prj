using System.Collections.Generic;
using Gladiators.Interfaces;
using Managers.Controller;
using UnityEngine;
using static Settings.Enumerators;

namespace Gladiators.Sound
{
    public class PlayerSoundView : SimpleSoundView, IPlayerModule
    {
        public void Init(PlayerController controller)
        {
            Init();
        }
    }
}
