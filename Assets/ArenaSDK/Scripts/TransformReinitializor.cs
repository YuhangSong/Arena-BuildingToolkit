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

        private List<GameObject> AllReinitializedGameObjects = new List<GameObject>();

        [Tooltip("If recursively apply transiform reinitialize for all its child objects")]
        public bool IsRecursively;

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
        /// Random localScale range (Min). This can override RandomScaleMin and RandomScaleMax.
        /// </summary>
        public float RandomUniformScaleMin;

        /// <summary>
        /// Random localScale range (Max). This can override RandomScaleMin and RandomScaleMax.
        /// </summary>
        public float RandomUniformScaleMax;

        /// <summary>
        /// Random localScale range (Min).
        /// </summary>
        public Vector3 RandomScaleMin;

        /// <summary>
        /// Random localScale range (Max).
        /// </summary>
        public Vector3 RandomScaleMax;

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
        public bool IsStepReward_MeanDistanceToCenter = false;

        /// <summary>
        /// </summary>
        public bool IsPunishMeanDistanceToCenter = false;

        /// <summary>
        /// </summary>
        public bool IsRewardShapeOfGroup = false;

        /// <summary>
        /// </summary>
        public char ShapeOfGroupToCompare = 'A';

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

        /// <summary>
        /// Reference to the GlobalManager.
        /// </summary>
        private GlobalManager globalManager;

        public void
        Initialize(GlobalManager globalManager_)
        {
            globalManager = globalManager_;
            Initialize();
        }

        private void
        Initialize()
        {
            // create ReinitializedGameObjects
            if (ReinitializedGameObject != null) {
                ReinitializedGameObjects.Add(ReinitializedGameObject);
            }

            // create AllReinitializedGameObjects
            if (ReinitializedGameObjects.Count > 0) {
                foreach (GameObject ReinitializedGameObject_ in ReinitializedGameObjects) {
                    AllReinitializedGameObjects.Add(ReinitializedGameObject_);
                }
            }

            foreach (GameObject ReinitializedGameObject_ in ReinitializedGameObjects) {
                for (int i = 0; i < NumDuplications; i++) {
                    GameObject Temp_;
                    Temp_ = GameObject.Instantiate(ReinitializedGameObject_,
                        ReinitializedGameObject_.transform.position,
                        ReinitializedGameObject_.transform.rotation) as GameObject;
                    AllReinitializedGameObjects.Add(Temp_);
                }
            }

            List<GameObject> tmp = new List<GameObject>();
            if (IsRecursively) {
                foreach (GameObject ReinitializedGameObject_ in AllReinitializedGameObjects) {
                    foreach (Transform child in ReinitializedGameObject_.GetComponentsInChildren<Transform>()) {
                        tmp.Add(child.gameObject);
                    }
                }
            }
            AllReinitializedGameObjects.AddRange(tmp);

            // record all initialize information
            foreach (GameObject ReinitializedGameObject_ in AllReinitializedGameObjects) {
                OriginalPosition.Add(ReinitializedGameObject_.transform.position);
                OriginalEulerAngles.Add(ReinitializedGameObject_.transform.eulerAngles);
                OriginalScales.Add(ReinitializedGameObject_.transform.localScale);
            }

            // MaxSpawnAttemptsPerGameObject should be at least 1
            if (MaxSpawnAttemptsPerGameObject < 1) {
                MaxSpawnAttemptsPerGameObject = 1;
            }

            if (IsStepReward_MeanDistanceToCenter && IsPunishMeanDistanceToCenter) {
                Debug.LogWarning("IsStepReward_MeanDistanceToCenter and IsPunishMeanDistanceToCenter is controversial");
            }

            RandomScaleMin        += Vector3.one;
            RandomScaleMax        += Vector3.one;
            RandomUniformScaleMin += 1f;
            RandomUniformScaleMax += 1f;
        } // Initialize

        /// <summary>
        /// Every Reinitializor should implement this method.
        /// </summary>
        public override void
        Reinitialize()
        {
            for (int i = 0; i < AllReinitializedGameObjects.Count; i++) {
                AllReinitializedGameObjects[i].SetActive(true);

                // whether or not we can spawn in this position
                bool validPosition = false;

                // How many times we've attempted to spawn this obstacle
                int spawnAttempts = 0;

                // While we don't have a valid position
                // and we haven't tried spawning this GameObject too many times
                while (!validPosition && spawnAttempts < MaxSpawnAttemptsPerGameObject) {
                    // Increase our spawn attempts
                    spawnAttempts++;

                    AllReinitializedGameObjects[i].transform.position = new Vector3(
                        OriginalPosition[i].x + Utils.RandomSign_Float()
                        * Random.Range(RandomPositionMin.x, RandomPositionMax.x),
                        OriginalPosition[i].y + Utils.RandomSign_Float()
                        * Random.Range(RandomPositionMin.y, RandomPositionMax.y),
                        OriginalPosition[i].z + Utils.RandomSign_Float()
                        * Random.Range(RandomPositionMin.z, RandomPositionMax.z)
                    );

                    AllReinitializedGameObjects[i].transform.eulerAngles = new Vector3(
                        OriginalEulerAngles[i].x + Utils.RandomSign_Float() * Random.Range(RandomEulerAnglesMin.x,
                        RandomEulerAnglesMax.x),
                        OriginalEulerAngles[i].y + Utils.RandomSign_Float() * Random.Range(RandomEulerAnglesMin.y,
                        RandomEulerAnglesMax.y),
                        OriginalEulerAngles[i].z + Utils.RandomSign_Float() * Random.Range(RandomEulerAnglesMin.z,
                        RandomEulerAnglesMax.z)
                    );

                    if ((RandomUniformScaleMax == 1f) && (RandomUniformScaleMin == 1f)) {
                        AllReinitializedGameObjects[i].transform.localScale = new Vector3(
                            OriginalScales[i].x * Random.Range(RandomScaleMin.x,
                            RandomScaleMax.x),
                            OriginalScales[i].y * Random.Range(RandomScaleMin.y,
                            RandomScaleMax.y),
                            OriginalScales[i].z * Random.Range(RandomScaleMin.z,
                            RandomScaleMax.z)
                        );
                    } else {
                        float RandomUniformScale = Random.Range(RandomUniformScaleMin,
                            RandomUniformScaleMax);
                        AllReinitializedGameObjects[i].transform.localScale = new Vector3(
                            OriginalScales[i].x * RandomUniformScale,
                            OriginalScales[i].y * RandomUniformScale,
                            OriginalScales[i].z * RandomUniformScale
                        );
                    }

                    // This position is valid until proven invalid
                    validPosition = true;

                    if (IsAvoidOverlap) {
                        // Go through all previous things
                        for (int j = 0; j < i; j++) {
                            float Distance_ = Vector3.Distance(
                                AllReinitializedGameObjects[j].transform.position,
                                AllReinitializedGameObjects[i].transform.position);
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


                if (AllReinitializedGameObjects[i].GetComponent<Rigidbody>() != null) {
                    AllReinitializedGameObjects[i].GetComponent<Rigidbody>().velocity =
                      Vector3.zero;
                    AllReinitializedGameObjects[i].GetComponent<Rigidbody>().angularVelocity =
                      Vector3.zero;
                    AllReinitializedGameObjects[i].GetComponent<Rigidbody>().AddForce(
                        new Vector3(
                            Utils.RandomSign_Float() * Random.Range(RandomForceMin.x, RandomForceMax.x),
                            Utils.RandomSign_Float() * Random.Range(RandomForceMin.y, RandomForceMax.y),
                            Utils.RandomSign_Float() * Random.Range(RandomForceMin.z, RandomForceMax.z)
                        )
                    );
                }
            }

            if (IsStepReward_MeanDistanceToCenter || IsPunishMeanDistanceToCenter) {
                Last_MeanDistanceToCenter = GetMeanDistanceToCenter();
            }
            if (IsRewardShapeOfGroup) {
                Last_EpisodeReward_ShapeOfGroup = GetEpisodeReward_ShapeOfGroup();
            }
        } // Reinitialize

        /// <summary>
        /// Step reward function.
        /// </summary>
        public float
        StepRewardFunction()
        {
            float StepReward_ = 0f;

            if (IsStepReward_MeanDistanceToCenter) {
                StepReward_ += GetStepReward_MeanDistanceToCenter();
            } else if (IsPunishMeanDistanceToCenter) {
                StepReward_ += GetStepPunish_MeanDistanceToCenter();
            }

            if (IsRewardShapeOfGroup) {
                StepReward_ += GetStepReward_ShapeOfGroup();
            }

            return StepReward_;
        }

        private float Last_MeanDistanceToCenter = 0f;

        public float
        GetStepReward_MeanDistanceToCenter()
        {
            float MeanDistanceToCenter = GetMeanDistanceToCenter();
            float StepReward_MeanDistanceToCenter = MeanDistanceToCenter - Last_MeanDistanceToCenter;

            Last_MeanDistanceToCenter = MeanDistanceToCenter;

            return StepReward_MeanDistanceToCenter * globalManager.RewardDistanceCoefficient;
        }

        public float
        GetStepPunish_MeanDistanceToCenter()
        {
            return -GetStepReward_MeanDistanceToCenter();
        }

        public Vector3
        GetGeographicalCenter()
        {
            Vector3 GeographicalCenter = new Vector3();

            foreach (GameObject ReinitializedGameObject_ in AllReinitializedGameObjects) {
                GeographicalCenter += ReinitializedGameObject_.transform.position;
            }
            GeographicalCenter /= AllReinitializedGameObjects.Count;

            return GeographicalCenter;
        }

        public float
        GetMeanDistanceToCenter()
        {
            Vector3 GeographicalCenter = GetGeographicalCenter();
            float MeanDistanceToCenter = 0f;

            foreach (GameObject ReinitializedGameObject_ in AllReinitializedGameObjects) {
                MeanDistanceToCenter +=
                  Vector3.Distance(ReinitializedGameObject_.transform.position, GeographicalCenter);
            }
            MeanDistanceToCenter /= AllReinitializedGameObjects.Count;
            return MeanDistanceToCenter;
        }

        private float Last_EpisodeReward_ShapeOfGroup = 0f;

        public float
        GetStepReward_ShapeOfGroup()
        {
            // ShapeOfGroupToCompare
            float EpisodeReward_ShapeOfGroup = GetEpisodeReward_ShapeOfGroup();
            float StepReward_ShapeOfGroup    = EpisodeReward_ShapeOfGroup - Last_EpisodeReward_ShapeOfGroup;

            Last_EpisodeReward_ShapeOfGroup = EpisodeReward_ShapeOfGroup;

            return StepReward_ShapeOfGroup * globalManager.RewardShapeOfGroupCoefficient;
        }

        public float
        GetEpisodeReward_ShapeOfGroup()
        {
            return 0.99f;
        }
    }
}
