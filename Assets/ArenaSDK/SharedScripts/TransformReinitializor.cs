using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;

namespace Arena
{
    /// <summary>
    /// Use this method to reinitialize a object's transform, and add forces.
    /// </summary>
    [System.Serializable]
    public class TransformReinitializor : Reinitializor
    {
        /// <summary>
        /// Reference to the GameObject.
        /// </summary>
        public GameObject ReinitializedGameObject;

        /// <summary>
        /// Reference to the GameObjects, use this if you want to added many object.
        /// </summary>
        public List<GameObject> ReinitializedGameObjects;

        private List<GameObject> ReinitializedGameObjectsWithDuplications = new List<GameObject>();

        /// <summary>
        /// Number of duplicatoins of the GameObject.
        /// </summary>
        public int NumDuplications = 0;

        /// <summary>
        /// The orignal position of the object.
        /// </summary>
        private List<Vector3> OriginalPosition = new List<Vector3>();

        /// <summary>
        /// Random position range (Min).
        /// </summary>
        public Vector3 RandomPositionMin;

        /// <summary>
        /// Random position range (Max).
        /// </summary>
        public Vector3 RandomPositionMax;

        /// <summary>
        /// Original eulerAngles of the object.
        /// </summary>
        private List<Vector3> OriginalEulerAngles = new List<Vector3>();

        /// <summary>
        /// Random eular angle range (Min).
        /// </summary>
        public Vector3 RandomEulerAnglesMin;

        /// <summary>
        /// Random eular angle range (Max).
        /// </summary>
        public Vector3 RandomEulerAnglesMax;

        /// <summary>
        /// Original localScale of the object.
        /// </summary>
        private List<Vector3> OriginalScales = new List<Vector3>();

        /// <summary>
        /// Random localScale range (Min).
        /// </summary>
        public Vector3 RandomScaleMin;

        /// <summary>
        /// Random localScale range (Max).
        /// </summary>
        public Vector3 RandomScaleMax;

        /// <summary>
        /// Random localScale range (Min).
        /// </summary>
        public float RandomUniformScaleMin = 0f;

        /// <summary>
        /// Random localScale range (Max).
        /// </summary>
        public float RandomUniformScaleMax = 0f;

        /// <summary>
        /// Random force range (Min).
        /// </summary>
        public Vector3 RandomForceMin;

        /// <summary>
        /// Random force range (Max).
        /// </summary>
        public Vector3 RandomForceMax;

        /// <summary>
        /// If avoid overlap of GameObjects in this TransformReinitializor.
        /// </summary>
        public bool IsAvoidOverlap = false;

        /// <summary>
        /// </summary>
        public float AvoidOverlapRadius = 1f;

        /// <summary>
        /// </summary>
        public int MaxSpawnAttemptsPerGameObject = 10;

        /// <summary>
        /// </summary>
        public bool IsRewardMeanDistanceToCenter = false;

        /// <summary>
        /// </summary>
        public bool IsPunishMeanDistanceToCenter = false;

        public TransformReinitializor()
        { }

        public TransformReinitializor(
            GameObject ReinitializedGameObject_) : this()
        {
            ReinitializedGameObject = ReinitializedGameObject_;
            Initialize();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ReinitializedGameObject_">Reference to the GameObject.</param>
        /// <param name="RandomPositionMin_">Random position range (Min).</param>
        /// <param name="RandomPositionMax_">Random position range (Max).</param>
        /// <param name="RandomEulerAnglesMin_">Random eular angle range (Min).</param>
        /// <param name="RandomEulerAnglesMax_">Random eular angle range (Max).</param>
        /// <param name="RandomForceMin_">Random force range (Min).</param>
        /// <param name="RandomForceMax_">Random force range (Max).</param>
        public TransformReinitializor(
            GameObject ReinitializedGameObject_,
            Vector3 RandomPositionMin_, Vector3 RandomPositionMax_,
            Vector3 RandomEulerAnglesMin_, Vector3 RandomEulerAnglesMax_,
            Vector3 RandomForceMin_, Vector3 RandomForceMax_) : this(ReinitializedGameObject_)
        {
            RandomPositionMin = RandomPositionMin_;
            RandomPositionMax = RandomPositionMax_;

            RandomEulerAnglesMin = RandomEulerAnglesMin_;
            RandomEulerAnglesMax = RandomEulerAnglesMax_;

            RandomForceMin = RandomForceMin_;
            RandomForceMax = RandomForceMax_;
        }

        public void
        Initialize()
        {
            // create ReinitializedGameObjects
            if (ReinitializedGameObject != null) {
                ReinitializedGameObjects.Add(ReinitializedGameObject);
            }

            // create ReinitializedGameObjectsWithDuplications
            if (ReinitializedGameObjects.Count > 0) {
                foreach (GameObject ReinitializedGameObject_ in ReinitializedGameObjects) {
                    ReinitializedGameObjectsWithDuplications.Add(ReinitializedGameObject_);
                }
            }

            foreach (GameObject ReinitializedGameObject_ in ReinitializedGameObjects) {
                for (int i = 0; i < NumDuplications; i++) {
                    GameObject Temp_;
                    Temp_ = GameObject.Instantiate(ReinitializedGameObject_,
                        ReinitializedGameObject_.transform.position,
                        ReinitializedGameObject_.transform.rotation) as GameObject;
                    ReinitializedGameObjectsWithDuplications.Add(Temp_);
                }
            }

            // record all initialize information
            foreach (GameObject ReinitializedGameObject_ in ReinitializedGameObjectsWithDuplications) {
                OriginalPosition.Add(ReinitializedGameObject_.transform.position);
                OriginalEulerAngles.Add(ReinitializedGameObject_.transform.eulerAngles);
                OriginalScales.Add(ReinitializedGameObject_.transform.localScale);
            }

            // MaxSpawnAttemptsPerGameObject should be at least 1
            if (MaxSpawnAttemptsPerGameObject < 1) {
                MaxSpawnAttemptsPerGameObject = 1;
            }

            if (IsRewardMeanDistanceToCenter && IsPunishMeanDistanceToCenter) {
                Debug.LogWarning("IsRewardMeanDistanceToCenter and IsPunishMeanDistanceToCenter is controversial");
            }
        } // Initialize

        public Vector3
        GetGeographicalCenter()
        {
            Vector3 GeographicalCenter = new Vector3();

            foreach (GameObject ReinitializedGameObject_ in ReinitializedGameObjectsWithDuplications) {
                GeographicalCenter += ReinitializedGameObject_.transform.position;
            }
            GeographicalCenter /= ReinitializedGameObjectsWithDuplications.Count;

            return GeographicalCenter;
        }

        public float
        GetMeanDistanceToCenter()
        {
            Vector3 GeographicalCenter = GetGeographicalCenter();
            float MeanDistanceToCenter = 0f;

            foreach (GameObject ReinitializedGameObject_ in ReinitializedGameObjectsWithDuplications) {
                MeanDistanceToCenter +=
                  Vector3.Distance(ReinitializedGameObject_.transform.position, GeographicalCenter);
            }
            MeanDistanceToCenter /= ReinitializedGameObjectsWithDuplications.Count;
            return MeanDistanceToCenter;
        }

        private float LastMeanDistanceToCenter = 0f;

        public float
        GetRewardMeanDistanceToCenter()
        {
            float MeanDistanceToCenter       = GetMeanDistanceToCenter();
            float RewardMeanDistanceToCenter = MeanDistanceToCenter - LastMeanDistanceToCenter;

            LastMeanDistanceToCenter = MeanDistanceToCenter;
            return RewardMeanDistanceToCenter;
        }

        public float
        GetPunishMeanDistanceToCenter()
        {
            return -GetRewardMeanDistanceToCenter();
        }

        /// <summary>
        /// Every Reinitializor should implement this method.
        /// </summary>
        override public void
        Reinitialize()
        {
            for (int i = 0; i < ReinitializedGameObjectsWithDuplications.Count; i++) {
                ReinitializedGameObjectsWithDuplications[i].SetActive(true);

                // whether or not we can spawn in this position
                bool validPosition = false;

                // How many times we've attempted to spawn this obstacle
                int spawnAttempts = 0;

                // While we don't have a valid position
                // and we haven't tried spawning this GameObject too many times
                while (!validPosition && spawnAttempts < MaxSpawnAttemptsPerGameObject) {
                    // Increase our spawn attempts
                    spawnAttempts++;

                    ReinitializedGameObjectsWithDuplications[i].transform.position = new Vector3(
                        OriginalPosition[i].x + Utils.RandomSign_Float()
                        * Random.Range(RandomPositionMin.x, RandomPositionMax.x),
                        OriginalPosition[i].y + Utils.RandomSign_Float()
                        * Random.Range(RandomPositionMin.y, RandomPositionMax.y),
                        OriginalPosition[i].z + Utils.RandomSign_Float()
                        * Random.Range(RandomPositionMin.z, RandomPositionMax.z)
                    );

                    ReinitializedGameObjectsWithDuplications[i].transform.eulerAngles = new Vector3(
                        OriginalEulerAngles[i].x + Utils.RandomSign_Float() * Random.Range(RandomEulerAnglesMin.x,
                        RandomEulerAnglesMax.x),
                        OriginalEulerAngles[i].y + Utils.RandomSign_Float() * Random.Range(RandomEulerAnglesMin.y,
                        RandomEulerAnglesMax.y),
                        OriginalEulerAngles[i].z + Utils.RandomSign_Float() * Random.Range(RandomEulerAnglesMin.z,
                        RandomEulerAnglesMax.z)
                    );

                    if (RandomUniformScaleMax <= 0f) {
                        ReinitializedGameObjectsWithDuplications[i].transform.localScale = new Vector3(
                            OriginalScales[i].x + Utils.RandomSign_Float() * Random.Range(RandomScaleMin.x,
                            RandomScaleMax.x),
                            OriginalScales[i].y + Utils.RandomSign_Float() * Random.Range(RandomScaleMin.y,
                            RandomScaleMax.y),
                            OriginalScales[i].z + Utils.RandomSign_Float() * Random.Range(RandomScaleMin.z,
                            RandomScaleMax.z)
                        );
                    } else {
                        float RandomIncrement_ = Utils.RandomSign_Float() * Random.Range(RandomUniformScaleMin,
                            RandomUniformScaleMax);
                        ReinitializedGameObjectsWithDuplications[i].transform.localScale = new Vector3(
                            OriginalScales[i].x + RandomIncrement_,
                            OriginalScales[i].y + RandomIncrement_,
                            OriginalScales[i].z + RandomIncrement_
                        );
                    }

                    // This position is valid until proven invalid
                    validPosition = true;

                    if (IsAvoidOverlap) {
                        // Go through all previous things
                        for (int j = 0; j < i; j++) {
                            float Distance_ = Vector3.Distance(
                                ReinitializedGameObjectsWithDuplications[j].transform.position,
                                ReinitializedGameObjectsWithDuplications[i].transform.position);
                            if (Distance_ < AvoidOverlapRadius) {
                                validPosition = false;
                                break;
                            }
                        }

                        // if this position is still invlid, but spawnAttempts has reached MaxSpawnAttemptsPerGameObject
                        if ((!validPosition) && (spawnAttempts == MaxSpawnAttemptsPerGameObject)) {
                            Debug.LogWarning(
                                "spawnAttempts has reached MaxSpawnAttemptsPerGameObject, but no still no valid position found, try increase MaxSpawnAttemptsPerGameObject or decrease AvoidOverlapRadius");
                        }
                    }
                }


                if (ReinitializedGameObjectsWithDuplications[i].GetComponent<Rigidbody>() != null) {
                    ReinitializedGameObjectsWithDuplications[i].GetComponent<Rigidbody>().velocity =
                      Vector3.zero;
                    ReinitializedGameObjectsWithDuplications[i].GetComponent<Rigidbody>().angularVelocity =
                      Vector3.zero;
                    ReinitializedGameObjectsWithDuplications[i].GetComponent<Rigidbody>().AddForce(
                        new Vector3(
                            Utils.RandomSign_Float() * Random.Range(RandomForceMin.x, RandomForceMax.x),
                            Utils.RandomSign_Float() * Random.Range(RandomForceMin.y, RandomForceMax.y),
                            Utils.RandomSign_Float() * Random.Range(RandomForceMin.z, RandomForceMax.z)
                        )
                    );
                }
            }

            if (IsRewardMeanDistanceToCenter || IsPunishMeanDistanceToCenter) {
                LastMeanDistanceToCenter = GetMeanDistanceToCenter();
            }
        } // Reinitialize
    }
}
