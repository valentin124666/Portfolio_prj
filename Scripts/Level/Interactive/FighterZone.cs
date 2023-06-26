using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Level.Interactive
{
    public class FighterZone : MonoBehaviour
    {
        [SerializeField] private Image _timerIterationStart;
        [SerializeField] private float _timeZoneActivation = 1;

        public Tween ZoneActivation()
        {
            return _timerIterationStart.DOFillAmount(0, _timeZoneActivation);
        }
        public void EndInteraction()
        {
            _timerIterationStart.DOKill();
            _timerIterationStart.fillAmount = 1;
        }

    }
}
