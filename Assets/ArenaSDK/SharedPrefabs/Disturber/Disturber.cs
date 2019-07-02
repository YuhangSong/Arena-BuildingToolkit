using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arena
{
    /// <summary>
    /// Attach this script to an GameObject so that this GameObject will be
    /// disturbed every a few seconds.
    /// </summary>
    public class Disturber : MonoBehaviour
    {
        /// <summary>
        /// The interval of the disturb (in seconds).
        /// </summary>
        public float DisturbInterval = 3f;

        /// <summary>
        /// The force of the disturb.
        /// </summary>
        public float DisturbForce = 10f;


        /// <summary>
        /// Initialize and start.
        /// </summary>
        protected void
        Start()
        {
            InvokeRepeating("LaunchDisturb", 0f, DisturbInterval);
        }

        /// <summary>
        /// Disturb.
        /// </summary>
        protected void
        LaunchDisturb()
        {
            GetComponent<Rigidbody>().velocity = (Random.onUnitSphere * DisturbForce);
        }
    }
}
