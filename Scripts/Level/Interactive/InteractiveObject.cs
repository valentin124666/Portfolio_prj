using DG.Tweening;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Level.Interactive
{
    public abstract class InteractiveObject : MonoBehaviour
    {
        [SerializeField] private Enumerators.InteractiveObjectName _interactiveName;
        public Enumerators.InteractiveObjectName InteractiveName => _interactiveName;

        [SerializeField] protected Transform _workplace;
        [SerializeField] protected Image _timerIterationStart;

        public Transform Workplace => _workplace;
        
        protected float _timeZoneActivation { get; set; }
        public virtual bool FinishedWorking { get; }
        public virtual Enumerators.InteractiveObjectType InteractiveObjectType { get; }
        public virtual bool AllowAccess { get; }

        public abstract void Init();

        public abstract void Interaction();

        public virtual void UpdateCustom()
        {
        }

        public virtual void SetActive(bool isActive, bool isAnimation = false)
        {
            gameObject.SetActive(isActive);
        }

        public virtual void EndInteraction()
        {
            _timerIterationStart.DOKill();
            _timerIterationStart.fillAmount = 1;
        }

        public Tween ZoneActivation()
        {
            return _timerIterationStart.DOFillAmount(0, _timeZoneActivation);
        }
    }
}