using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Arena
{
    [ExecuteInEditMode]
    public class Lidar : ArenaBase
    {
        /// <summary>
        /// </summary>
        [Tooltip("If visualize lidar, only take effect in editor mode")]
        public bool IsVisLidar = false;
        [Space(5)]
        [Header("Horizontal parameters")]

        /// <summary>
        /// </summary>
        [Range(1, 256)]
        public int HorizontalRayCount = 16;

        [Tooltip("Width of Field-of-View in degrees")]
        [Range(1f, 359f)]
        public float HorizontalFieldOfView = 150;


        /// <summary>
        /// </summary>


        [Header("Vertical parameters")]
        /// <summary>
        /// </summary>
        [Range(1, 256)]
        public int VerticalRayCount = 1;

        [Tooltip("Height of Field-of-View in degrees")]
        [Range(1f, 359f)]
        public float VerticalFieldOfView = 150; // Horizontal field of view
        [Space(5)]


        /// <summary>
        /// ScanFramePerSecond
        /// -1 means refreshing all data at each GetFrame
        /// </summary>
        [Tooltip(" 'Refresh rate' of the LIDAR sensor. Set value to '-1' for casting all rays each frame")]
        public float ScanFramePerSecond = -1f;

        /// <summary>
        /// Within this bound, lidar returns 0
        /// </summary>
        [Tooltip(
            "Inside this bound, lidar detection returns 1, set to 0 to disable the outter bound. This avoids selfcollision with the agent")
        ]
        [Range(0.0f, 50f)]
        public float InnerBound = 0.5f;

        [Tooltip("Outside this bound, lidar detection returns 1, set to 0 to disable the outter bound.")]
        [Range(0.0f, 50f)]
        public float OutterBound = 5f;

        /// <summary>
        /// </summary>
        public Color LidarColorHit = Color.red;

        /// <summary>
        /// </summary>
        public Color LidarColorNoHit = Color.blue;

        /// <summary>
        /// </summary>
        private Quaternion[] Directions;

        /// <summary>
        /// </summary>
        private RaycastHit Hit;

        /// <summary>
        /// </summary>
        private float[] Distances;

        /// <summary>
        /// </summary>
        private float[] NormedDistances;

        /// <summary>
        /// </summary>
        private int NumDataPerFrame;

        public override void
        Initialize()
        {
            base.Initialize();

            CheckConfig();
            CreateLidar();

            // Debug.Log(GetLogTag() + " Initialize");
        }

        private float maxDistance;

        private float StepAngleHeight;
        private float StepAngleWidth;

        private void
        CheckConfig()
        {
            NumDataPerFrame = VerticalRayCount * HorizontalRayCount;

            maxDistance = OutterBound - InnerBound;

            StepAngleHeight = VerticalFieldOfView / VerticalRayCount;
            StepAngleWidth  = HorizontalFieldOfView / HorizontalRayCount;

            if (OutterBound == 0f) {
                maxDistance = Mathf.Infinity;
            }

            if (maxDistance < 0) {
                Debug.LogError("OutterBound - InnerBound < 0");
            }
        }

        private void
        CreateLidar()
        {
            Directions = new Quaternion[NumDataPerFrame];

            for (int h = 0; h < VerticalRayCount; h++) {
                Quaternion frontDirection = Quaternion.Euler(transform.forward);
                for (int w = 0; w < HorizontalRayCount; w++) {
                    int i = h * HorizontalRayCount + w;
                    frontDirection = Quaternion.Euler(transform.forward)
                      * Quaternion.Euler(new Vector3((-VerticalFieldOfView / 2) + StepAngleHeight / 2
                        + StepAngleHeight * (h), 0, 0));
                    Directions[i] = frontDirection
                      * Quaternion.AngleAxis(((-HorizontalFieldOfView / 2) + StepAngleWidth / 2 + StepAngleWidth * (w)),
                        transform.up);
                }
            }

            Distances       = new float[NumDataPerFrame];
            NormedDistances = new float[NumDataPerFrame];
        }

        void
        Update()
        {
            if (!Application.isPlaying) {
                Initialize();
                GetFrame();
            }
        }

        private float LastTimeGetFrame = 0f;

        /// <summary>
        /// Get lidar frame (in its original shape: h*w).
        /// This is essitially a depth map
        /// </summary>
        public float[,]
        GetFrameImg()
        {
            Step();
            float[,] x = new float[10, 10];
            return x;
        }

        private int CurrentFramePointer = 0;

        /// <summary>
        /// Get lidar frame in a flatten float array.
        /// </summary>
        public float[]
        GetFrame()
        {
            Step();
            return NormedDistances;
        } // GetFrame

        private const int LidarIgnoredLayer = 11;

        private void
        Step()
        {
            if (globalManager != null) {
                IsVisLidar = globalManager.IsVisLidar;
            }

            int NumDataThisRefresh = 0;

            if (Application.isPlaying) {
                float DeltaTime = Time.time - LastTimeGetFrame;
                if (ScanFramePerSecond < 0f) {
                    NumDataThisRefresh = NumDataPerFrame;
                } else {
                    NumDataThisRefresh = (int) (DeltaTime * ScanFramePerSecond * NumDataPerFrame);
                    if (NumDataThisRefresh > 0) {
                        // it is possible that NumDataThisRefresh=0, which means waiting for several GetFrame() to have a positive NumDataThisRefresh
                        LastTimeGetFrame = Time.time;
                    } else if (NumDataThisRefresh > NumDataPerFrame) {
                        // the maximal ScanFramePerSecond results in a complete scan per GetFrame
                        NumDataThisRefresh = NumDataPerFrame;
                    }
                }
            } else {
                NumDataThisRefresh = NumDataPerFrame;
            }

            // only update NumDataThisRefresh data points at each GetFrame()
            for (int i = 0; i < NumDataThisRefresh; i++) {
                Vector3 direction = (Directions[CurrentFramePointer] * gameObject.transform.forward).normalized;
                Vector3 origin    = transform.position + InnerBound * direction;
                if (Physics.Raycast(origin, direction, out Hit,
                  maxDistance, LidarIgnoredLayer)) //  add a layer mask value if you need to ignore certain type of objects
                {
                    if (Application.isEditor && IsVisLidar) {
                        Debug.DrawLine(origin, origin + direction * Hit.distance,
                          LidarColorHit);
                    }
                    Distances[CurrentFramePointer]       = Hit.distance;
                    NormedDistances[CurrentFramePointer] = Hit.distance / maxDistance;
                } else {
                    Distances[CurrentFramePointer]       = Mathf.Infinity;
                    NormedDistances[CurrentFramePointer] = 1f;
                    if (Application.isEditor && IsVisLidar) {
                        Debug.DrawLine(origin, origin + direction * maxDistance, LidarColorNoHit);
                    }
                }

                CurrentFramePointer++;
                if (CurrentFramePointer >= NumDataPerFrame) {
                    CurrentFramePointer = 0;
                }
            }
        } // Step

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
