using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

namespace Arena
{
    /// <summary>
    /// Attach SelfDestroy to a GameObject with Collider, it will destroy itself when hit anything.
    /// If hit object with object of ExceptTags, this will not take effect
    /// </summary>
    public class SelfDestroyGate : Gate
    {
        /// <summary>
        /// destroy self when hit anything but ExceptTags
        /// <summary>
        public List<string> ExceptTags = new List<string>();

        protected override void
        TrigEvent(GameObject other)
        {
            // destroy self when hit anything but ExceptTags
            foreach (string Tag_ in ExceptTags) {
                if (other.CompareTag(Tag_)) {
                    return;
                }
            }

            Destroy(gameObject);
        }
    }
}
