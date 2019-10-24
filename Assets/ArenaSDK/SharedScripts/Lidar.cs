using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Arena
{
    [RequireComponent(typeof(LidarCast))]
    public class Lidar : ArenaBase
    {
        public float[] GetFlattenFrame()
        {
            return gameObject.GetComponent<LidarCast>().LidarValue;
        }
    }
}
