using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arena
{
    /// <summary>
    /// This function is for accumulating a scaler.
    /// This is especially useful when applying force on agent.
    /// Specifically, you can accumulate the force if an action is called repeatly.
    /// </summary>
    public class Accumulator
    {
        /// <summary>
        /// Base value the scaler start accumulate from
        /// </summary>
        private float Base;

        /// <summary>
        /// Accumulate Speed of the scaler.
        /// </summary>
        private float Speed;

        /// <summary>
        /// Max of the scaler.
        /// </summary>
        private float Max = -1f;

        /// <summary>
        /// Current value of the scaler.
        /// </summary>
        private float Current;

        /// <summary>
        /// Constructor. Set properties when construct.
        /// </summary>
        /// <param name="Base_">Base value the scaler start accumulate from.</param>
        /// <param name="Speed_">Accumulate Speed of the scaler.</param>
        public Accumulator(float Base_, float Speed_)
        {
            Base    = Base_;
            Speed   = Speed_;
            Current = Base;
        }

        public Accumulator(float Base_, float Speed_, float Max_) : this(Base_, Speed_)
        {
            Max = Max_;
        }

        /// <summary>
        /// Get current value of the scaler.
        /// </summary>
        /// <returns>current value of the scaler.</returns>
        public float
        getCurrent()
        {
            return Current;
        }

        /// <summary>
        /// Accumulate the scaler.
        /// </summary>
        public void
        Accumulate()
        {
            Current += Speed;
            if (Max > 0) {
                if (Current > Max) {
                    Current = Max;
                }
            }
        }

        /// <summary>
        /// Reset the scaler to Base.
        /// </summary>
        public void
        Reset()
        {
            Current = Base;
        }
    }
}
