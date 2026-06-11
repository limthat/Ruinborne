using UnityEngine;

namespace Ruinborne.Core
{
    public abstract class ManagerBase<T> : MonoBehaviour where T : ManagerBase<T>
    {
        protected virtual void Awake()
        {
            ServiceLocator.Register<T>((T)this);
        }

        protected virtual void OnDestroy()
        {
            ServiceLocator.Unregister<T>();
        }
    }
}
