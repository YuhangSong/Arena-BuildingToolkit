using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arena
{
    public class Lidar : ArenaBase
    {
        /// <summary>
        /// Within this bound, lidar returns 0
        /// </summary>
        public GameObject InnerBound;

        /// <summary>
        /// Exceed this bound, lidar returns 1
        /// </summary>
        public GameObject OutterBound;

        /// <summary>
        /// Direction to the front
        /// </summary>
        public GameObject Front;

        /// <summary>
        /// Within this bound, lidar returns 0
        /// </summary>
        private float InnerBoundR;

        /// <summary>
        /// Exceed this bound, lidar returns 1
        /// </summary>
        private float OutterBoundR;

        /// <summary>
        /// Direction to the front
        /// </summary>
        private Vector3 FrontV;

        public override void
        Initialize()
        {
            base.Initialize();

            // Debug.Log(GetLogTag() + " Initialize");
        }

        private void
        CheckConfig()
        {
            InnerBoundR  = GetBoundRFromBound(InnerBound);
            OutterBoundR = GetBoundRFromBound(OutterBound);
            FrontV       = (Front.transform.position - transform.position)
              / (Front.transform.position - transform.position).magnitude;
        }

        private float
        GetBoundRFromBound(GameObject Bound)
        {
            float BoundR;

            if ((Bound.transform.lossyScale / Bound.transform.lossyScale.magnitude).Equals(Utils.Ones)) {
                BoundR = Bound.transform.lossyScale.magnitude;
            } else {
                Debug.LogError(GetLogTag() + " Bound " + Bound + " is not Ones.");
                BoundR = 0f;
            }
            return BoundR;
        }

        /// <summary>
        /// Get lidar frame (in its original shape: h*w).
        /// This is essitially a depth map
        /// </summary>
        public float[,]
        GetFrame()
        {
            float[,] x = new float[10, 10];

            return x;
        }

        /// <summary>
        /// Get lidar frame in a flatten float array.
        /// </summary>
        public float[]
        GetFlattenFrame()
        {
            float[] x = new float[10];
            return x;
        }

        /// <summary>
        /// Get the log tag of the object.
        /// </summary>
        /// <returns>LogTag.</returns>
        public override string
        GetLogTag()
        {
            return GetComponentInParent<ArenaAgent>().GetLogTag() + "-Lidar";
        }
    }
}
