using Core.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

    namespace Core
    {
        public abstract class SimplePresenter<TP, TV> : IPresenter where TP : SimplePresenter<TP, TV> where TV : SimplePresenterView<TP, TV> {
            protected TV View { get; }
            public bool IsDestroyed { get; protected set; }
            public virtual bool IsActive => View.gameObject.activeSelf;

            protected SimplePresenter(TV view) {
                View = view;
                ((IView) View).SetPresenter((TP) this);
                View.Init();
            }
            
            public void SetParent(Transform parent) {
                View.transform.SetParent(parent, false);
            }

            public virtual void SetActive(bool active) {
                
                View.SetActive(active);
            }

            public void Translate(Vector3 pos)
            {
               View.transform.Translate(pos);
            }

            public virtual void SetPositionAndRotation(Vector3 pos , Quaternion rot)
            {
                View.transform.SetPositionAndRotation(pos,rot);
            }

            public virtual void Destroy() {
                IsDestroyed = true;

                if (View != null) {
                    Object.Destroy(View.gameObject);
                }
            }

            public void OnDestroy() {
                if(View!=null)
                ResourceLoader.ReleaseInstance(View.gameObject);
            }
        }
    }
