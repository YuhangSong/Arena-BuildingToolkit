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

        private List<GameObject> ReinitializedGameObjects = new List<GameObject>();

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
        /// Random force range (Min).
        /// </summary>
        public Vector3 RandomForceMin;

        /// <summary>
        /// Random force range (Max).
        /// </summary>
        public Vector3 RandomForceMax;

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
            if (ReinitializedGameObject != null) {
                ReinitializedGameObjects.Add(ReinitializedGameObject);
            }

            for (int i = 0; i < NumDuplications; i++) {
                GameObject Temp_;
                Temp_ = GameObject.Instantiate(ReinitializedGameObject, ReinitializedGameObject.transform.position,
                    ReinitializedGameObject.transform.rotation) as GameObject;
                ReinitializedGameObjects.Add(Temp_);
            }

            foreach (GameObject ReinitializedGameObject_ in ReinitializedGameObjects) {
                OriginalPosition.Add(ReinitializedGameObject_.transform.position);
                OriginalEulerAngles.Add(ReinitializedGameObject_.transform.eulerAngles);
            }
        }

        /// <summary>
        /// Every Reinitializor should implement this method.
        /// </summary>
        override public void
        Reinitialize()
        {
            for (int i = 0; i < ReinitializedGameObjects.Count; i++) {
                ReinitializedGameObjects[i].SetActive(true);

                // Reinitialize the ReinitializedGameObjects[i] to a position and EulerAngles that has some randomness
                ReinitializedGameObjects[i].transform.position = new Vector3(
                    OriginalPosition[i].x + Utils.RandomSign_Float()
                    * Random.Range(RandomPositionMin.x, RandomPositionMax.x),
                    OriginalPosition[i].y + Utils.RandomSign_Float()
                    * Random.Range(RandomPositionMin.y, RandomPositionMax.y),
                    OriginalPosition[i].z + Utils.RandomSign_Float()
                    * Random.Range(RandomPositionMin.z, RandomPositionMax.z)
                );

                ReinitializedGameObjects[i].transform.eulerAngles = new Vector3(
                    OriginalEulerAngles[i].x + Utils.RandomSign_Float() * Random.Range(RandomEulerAnglesMin.x,
                    RandomEulerAnglesMax.x),
                    OriginalEulerAngles[i].y + Utils.RandomSign_Float() * Random.Range(RandomEulerAnglesMin.y,
                    RandomEulerAnglesMax.y),
                    OriginalEulerAngles[i].z + Utils.RandomSign_Float() * Random.Range(RandomEulerAnglesMin.z,
                    RandomEulerAnglesMax.z)
                );

                if (ReinitializedGameObjects[i].GetComponent<Rigidbody>() != null) {
                    ReinitializedGameObjects[i].GetComponent<Rigidbody>().velocity        = Vector3.zero;
                    ReinitializedGameObjects[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    ReinitializedGameObjects[i].GetComponent<Rigidbody>().AddForce(
                        new Vector3(
                            Utils.RandomSign_Float() * Random.Range(RandomForceMin.x, RandomForceMax.x),
                            Utils.RandomSign_Float() * Random.Range(RandomForceMin.y, RandomForceMax.y),
                            Utils.RandomSign_Float() * Random.Range(RandomForceMin.z, RandomForceMax.z)
                        )
                    );
                }
            }
        } // Reinitialize
    }
}
