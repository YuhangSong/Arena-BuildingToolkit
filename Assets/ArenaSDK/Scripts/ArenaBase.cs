using UnityEngine;

namespace Arena
{
    /// <summary>
    /// Base of Arena.
    /// All scipts under the GlobalManager should inherit this class.
    /// Exception: ArenaAgent (Since it inherits Agent from MLAgents.
    /// </summary>
    public class ArenaBase : MonoBehaviour
    {
        /// <summary>
        /// Reference to the GlobalManager.
        /// </summary>
        protected GlobalManager globalManager;

        void
        Start()
        {
            Initialize();
        }

        public virtual void
        Initialize()
        {
            globalManager = GetComponentInParent<GlobalManager>();
            if (globalManager == null) {
                globalManager = GetComponent<GlobalManager>();
            }
            if (globalManager == null) {
                Debug.LogError("Cannot find the GlobalManager");
            }
        }

        public GlobalManager
        getGlobalManager()
        {
            return globalManager;
        }

        /// <summary>
        /// Get the log tag of the object.
        /// </summary>
        /// <returns>LogTag.</returns>
        public virtual string
        GetLogTag()
        {
            return tag;
        }
    }
}
