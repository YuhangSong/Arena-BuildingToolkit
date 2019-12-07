using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arena {
    /// <summary>
    /// Used to reinitialize a ReinitializedLight object
    /// </summary>
    public class LightReinitializor : Reinitializor
    {
        /// <summary>
        /// How much randomness to add on the ReinitializedLight.
        /// </summary>
        private float RandomIntensity;

        /// <summary>
        /// Record of original ReinitializedLight intensity.
        /// </summary>
        private float OriginalIntensity;

        /// <summary>
        /// Reference to the ReinitializedLight object.
        /// </summary>
        private Light ReinitializedLight;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ReinitializedLight_">Reference to the ReinitializedLight object.</param>
        /// <param name="RandomIntensity_">Randomlized light intensity will be uniform between [OriginalIntensity-RandomIntensity,OriginalIntensity+RandomIntensity].</param>
        public LightReinitializor(Light ReinitializedLight_, float RandomIntensity_)
        {
            ReinitializedLight = ReinitializedLight_;
            OriginalIntensity  = ReinitializedLight.intensity;
            RandomIntensity    = RandomIntensity_;
        }

        /// <summary>
        /// Reinitialize.
        /// </summary>
        override public void
        Reinitialize()
        {
            float NewIntensity_ = OriginalIntensity + Random.Range(-RandomIntensity, RandomIntensity);

            if (NewIntensity_ < 0f) {
                NewIntensity_ = 0f;
            }
            ReinitializedLight.intensity = NewIntensity_;
        }
    }
}
