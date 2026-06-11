using Fusion;
using Ruinborne.Core;
using UnityEngine;

namespace Ruinborne.Core
{
    public abstract class NetworkManagerBase<T> : SimulationBehaviour
        where T : NetworkManagerBase<T>
    {
        protected virtual void OnEnable()
        {
            ServiceLocator.Register<T>((T)this);
            OnServerStarted();
        }

        protected virtual void OnDisable()
        {
            ServiceLocator.Unregister<T>();
            OnServerStopped();
        }

        protected virtual void OnServerStarted() { }
        protected virtual void OnServerStopped() { }
    }
}
