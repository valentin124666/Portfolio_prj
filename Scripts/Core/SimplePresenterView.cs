using System;
using System.Linq;
using System.Reflection;
using Core.Interfaces;
using UnityEngine;

namespace Core
{
    public abstract class SimplePresenterView<TP, TV> : MonoBehaviour, IView where TP : SimplePresenter<TP, TV> where TV : SimplePresenterView<TP, TV>
    {
        public TP Presenter { get; private set; }

        public IPresenter Instantiate(params object[] args)
        {
            var overriddenType = GetType().GetCustomAttribute<OverrideTypes>()?.Presenter;
            Activator.CreateInstance(overriddenType ?? typeof(TP), new object[] { this }.Concat(args).ToArray());
            return Presenter;
        }

        //note: this is necessary since there will still not be a presenter in view when we calling view methods from constructor
        void IView.SetPresenter(IPresenter presenter)
        {
            Presenter = (TP)presenter;
        }
        
        public abstract void Init();

        public virtual void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public virtual void OnDestroy()
        {
            Presenter?.OnDestroy();
        }
    }
}