using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arena {
    /// <summary>
    /// Used to reinitialize a light object
    /// </summary>
    public class LightReinitializor : Reinitializor
    {
        /// <summary>
        /// How much randomness to add on the light.
        /// </summary>
        private float RandomIntensity;

        /// <summary>
        /// Record of original light intensity.
        /// </summary>
        private float OriginalIntensity;

        /// <summary>
        /// Reference to the light object.
        /// </summary>
        private Light light;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="light_">Reference to the light object.</param>
        /// <param name="RandomIntensity_">How much randomness to add on the light.</param>
        public LightReinitializor(Light light_, float RandomIntensity_)
        {
            light = light_;
            OriginalIntensity = light.intensity;
            RandomIntensity   = RandomIntensity_;
        }

        /// <summary>
        /// Reinitialize.
        /// </summary>
        override public void
        Reinitialize()
        {
            light.intensity = OriginalIntensity + Random.Range(-RandomIntensity, RandomIntensity);
        }
    }
}
