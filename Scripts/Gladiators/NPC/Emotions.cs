using System.Linq;
using Settings;
using Tools;
using UnityEngine;

namespace Gladiators.NPC
{
    public class Emotions : MonoBehaviour
    {
        [SerializeField] private ImagesEmotions[] _imagesEmotions;

        private ImagesEmotions _currentEmotion;
    
        public void ActivationEmotions(Enumerators.EmotionsType typeActive)
        {
            var emotion = _imagesEmotions.First(item => item.Type == typeActive);
            if(emotion==null)return;

            _currentEmotion = emotion;
            _currentEmotion.SetActive(true);
            InternalTools.ActionDelayed(() => { _currentEmotion.SetActive(false); }, 1f);
        }
    }
}
