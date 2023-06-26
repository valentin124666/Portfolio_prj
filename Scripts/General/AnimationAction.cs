using System;
using Settings;
using UnityEngine;

namespace General
{
    public class AnimationAction : MonoBehaviour
    {
        public event Action<Enumerators.SoundName> animationEvent;

        public void AnimationSound( Enumerators.SoundName name)
        {
            animationEvent?.Invoke(name);
        }
    
    }
}
