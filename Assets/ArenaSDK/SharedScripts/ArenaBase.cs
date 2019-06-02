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
        }

        public GlobalManager
        getGlobalManager()
        {
            return globalManager;
        }
    }
}
