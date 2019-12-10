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
            if (!globalManager) {
                if (!GetComponent<GlobalManager>()){
                    GameObject gm = GameObject.FindGameObjectWithTag("GlobalManager");
                    if (!gm){
                        Debug.LogError("Cannot find the GlobalManager");
                    }
                    else { globalManager = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<GlobalManager>(); }
                }
                else { globalManager = GetComponent<GlobalManager>(); }
            }
            else { globalManager = GetComponentInParent<GlobalManager>(); }
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
