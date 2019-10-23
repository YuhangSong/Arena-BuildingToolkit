using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arena
{
    public class Lidar : ArenaBase
    {
        /// <summary>
        /// </summary>
        public int FrameHeight = 1;

        /// <summary>
        /// </summary>
        public int FrameWidth = 84;

        /// <summary>
        /// ScanFramePerSecond
        /// -1 means refreshing all data at each GetFrame
        /// </summary>
        public float ScanFramePerSecond = -1f;

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

        /// <summary>
        /// </summary>
        private int NumDataPerFrame;

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

            NumDataPerFrame = FrameHeight * FrameWidth;
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

        private float LastTimeGetFrame = 0f;

        /// <summary>
        /// Get lidar frame (in its original shape: h*w).
        /// This is essitially a depth map
        /// </summary>
        public float[,]
        GetFrame()
        {
            float DeltaTime       = Time.time - LastTimeGetFrame;
            int NumDataPerRefresh = 0;

            if (ScanFramePerSecond < 0f) {
                NumDataPerRefresh = NumDataPerFrame;
            } else {
                NumDataPerRefresh = (int) (DeltaTime * ScanFramePerSecond * NumDataPerFrame);
                if (NumDataPerRefresh > 0) {
                    // it is possible that NumDataPerRefresh=0, which means waiting for several GetFrame() to have a positive NumDataPerRefresh
                    LastTimeGetFrame = Time.time;
                } else if (NumDataPerRefresh > NumDataPerFrame)     {
                    // the maximal ScanFramePerSecond results in a complete scan per GetFrame
                    NumDataPerRefresh = NumDataPerFrame;
                }
            }

            // only update NumDataPerRefresh data points at each GetFrame()
            float[,] x = new float[10, 10];

            return x;
        }

        /// <summary>
        /// Get lidar frame in a flatten float array.
        /// </summary>
        public float[]
        GetFlattenFrame()
        {
            float[,] y = GetFrame();
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
