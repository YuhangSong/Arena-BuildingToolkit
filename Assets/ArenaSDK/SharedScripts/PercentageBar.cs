using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arena
{
    /// <summary>
    /// A object (normally a Cylinder) that display a percentage as the scale of the object.
    /// </summary>
    public class PercentageBar : MonoBehaviour
    {
        /// <summary>
        /// Axises.
        /// </summary>
        public enum Axises {
            x,
            y,
            z
        }

        /// <summary>
        /// Which axis you want the percentage to show.
        /// </summary>
        public Axises Axis;

        /// <summary>
        /// Original scale of the object.
        /// </summary>
        private Vector3 OriginalScale;

        /// <summary>
        /// Start and initialize.
        /// </summary>
        void
        Start()
        {
            this.OriginalScale = this.transform.localScale;
        }

        private float CurrentPercentage = 0f;

        /// <summary>
        /// Update the percentage to display.
        /// </summary>
        /// <param name="Percentage_">The percentage to be updated to.</param>
        public void
        UpdatePercentage(float Percentage_)
        {
            if (Percentage_ != CurrentPercentage) {
                CurrentPercentage = Percentage_;

                if (CurrentPercentage < 0f) {
                    CurrentPercentage = 0f;
                } else if (CurrentPercentage > 1f) {
                    CurrentPercentage = 1f;
                }

                Vector3 OriginalScale_temp = this.OriginalScale;
                if (Axis == Axises.x) {
                    OriginalScale_temp.x = this.OriginalScale.x * CurrentPercentage;
                } else if (Axis == Axises.y) {
                    OriginalScale_temp.y = this.OriginalScale.y * CurrentPercentage;
                } else if (Axis == Axises.z) {
                    OriginalScale_temp.z = this.OriginalScale.z * CurrentPercentage;
                } else {
                    Debug.LogWarning("Not a valid Axis");
                }

                transform.localScale = OriginalScale_temp;
            }
        }
    }
}
