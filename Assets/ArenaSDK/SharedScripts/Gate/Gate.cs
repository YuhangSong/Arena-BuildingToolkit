using UnityEngine;

namespace Arena {
    /// <summary>
    /// Gate is trigged either in OnCollisionEnter or OnTriggerEnter
    /// </summary>
    public class Gate : ArenaBase
    {
        void
        Start()
        {
            Initialize();
        }

        void
        OnCollisionEnter(Collision other)
        {
            TrigEvent(other.gameObject);
        }

        void
        OnTriggerEnter(Collider other)
        {
            TrigEvent(other.gameObject);
        }

        protected virtual void
        TrigEvent(GameObject other)
        { } // TrigEvent
    }
}
