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
        public bool IsVisualize = true;

        /// <summary>
        /// </summary>
        [Range(1, 256)]
        public int FrameHeight = 1;

        /// <summary>
        /// </summary>
        [Range(1, 256)]
        public int FrameWidth = 84;

        /// <summary>
        /// </summary>
        [Tooltip("Height of Field-of-View in degrees")]
        [Range(1f, 359f)]
        public float FieldHeight = 150; // Horizontal field of view

        /// <summary>
        /// </summary>
        [Tooltip("Width of Field-of-View in degrees")]
        [Range(1f, 359f)]
        public float FieldWidth = 150;

        /// <summary>
        /// ScanFramePerSecond
        /// -1 means refreshing all data at each GetFrame
        /// </summary>
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
        public Color LidarColorHit;

        /// <summary>
        /// </summary>
        public Color LidarColorNoHit;

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
            NumDataPerFrame = FrameHeight * FrameWidth;

            maxDistance = OutterBound - InnerBound;

            StepAngleHeight = FieldHeight / FrameHeight;
            StepAngleWidth  = FieldWidth / FrameWidth;

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

            for (int h = 0; h < FrameHeight; h++) {
                for (int w = 0; w < FrameWidth; w++) {
                    int i = h * FrameWidth + w;
                    Directions[i] = Quaternion.Euler(
                        transform.rotation.x + (-FieldHeight / 2) + StepAngleHeight / 2 + StepAngleHeight * (h),
                        transform.rotation.y + (-FieldWidth / 2) + StepAngleWidth / 2 + StepAngleWidth * (w),
                        transform.rotation.z
                    );
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
                    if (Application.isEditor && IsVisualize) {
                        Debug.DrawLine(origin, origin + direction * Hit.distance,
                          LidarColorHit);
                    }
                    Distances[CurrentFramePointer]       = Hit.distance;
                    NormedDistances[CurrentFramePointer] = Hit.distance / maxDistance;
                } else {
                    Distances[CurrentFramePointer]       = Mathf.Infinity;
                    NormedDistances[CurrentFramePointer] = 1f;
                    if (Application.isEditor && IsVisualize) {
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
