using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arena
{
    /// <summary>
    /// Attach SelfDeactiveGate to a GameObject with Collider, it will deactive itself when hit anything.
    /// If hit object with object of ExceptTags, this will not take effect
    /// </summary>
    public class SelfDeactiveGate : Gate
    {
        /// <summary>
        /// deactive self when hit anything but ExceptTags
        /// <summary>
        public List<string> ExceptTags = new List<string>();

        protected override void
        TrigEvent(GameObject other)
        {
            // deactive self when hit anything but ExceptTags
            foreach (string Tag_ in ExceptTags) {
                if (other.CompareTag(Tag_)) {
                    return;
                }
            }

            gameObject.SetActive(false);
        }
    }
}
