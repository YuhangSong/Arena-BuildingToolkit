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

        public virtual void
        Initialize()
        {
            globalManager = GetComponentInParent<GlobalManager>();
            if (globalManager == null) {
                globalManager = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<GlobalManager>();
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
    }
}
