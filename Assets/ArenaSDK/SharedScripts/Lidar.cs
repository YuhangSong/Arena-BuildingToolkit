using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arena
{
    public class Lidar : MonoBehaviour
    {
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
    }
}
