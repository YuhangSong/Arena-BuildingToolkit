using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arena
{
    /// <summary>
    /// </summary>
    public class ArenaMovingObject : ArenaBase
    {
        /// <summary>
        /// </summary>
        private float LastTimeUpdateMovement = -1f;

        /// <summary>
        /// </summary>
        protected float DeltTimeUpdateMovement = 0f;

        /// <summary>
        /// </summary>
        public virtual void
        UpdateMovement()
        {
            if (LastTimeUpdateMovement == -1f) {
                DeltTimeUpdateMovement = 0f;
            } else {
                DeltTimeUpdateMovement = Time.time - LastTimeUpdateMovement;
            }
            LastTimeUpdateMovement = Time.time;
        }
    }
}
