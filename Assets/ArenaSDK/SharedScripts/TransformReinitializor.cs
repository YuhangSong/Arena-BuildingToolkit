using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arena
{
    /// <summary>
    /// Use this method to reinitialize a object's transform, and add forces.
    /// </summary>
    public class TransformReinitializor : Reinitializor
    {
        /// <summary>
        /// The orignal position of the object.
        /// </summary>
        private Vector3 OriginalPosition;

        /// <summary>
        /// Random position range (Min).
        /// </summary>
        private Vector3 RandomPositionMin;

        /// <summary>
        /// Random position range (Max).
        /// </summary>
        private Vector3 RandomPositionMax;

        /// <summary>
        /// Original eulerAngles of the object.
        /// </summary>
        private Vector3 OriginalEulerAngles;

        /// <summary>
        /// Random eular angle range (Min).
        /// </summary>
        private Vector3 RandomEulerAnglesMin;

        /// <summary>
        /// Random eular angle range (Max).
        /// </summary>
        private Vector3 RandomEulerAnglesMax;

        /// <summary>
        /// Random force range (Min).
        /// </summary>
        private Vector3 RandomForceMin;

        /// <summary>
        /// Random force range (Max).
        /// </summary>
        private Vector3 RandomForceMax;

        /// <summary>
        /// Reference to the GameObject.
        /// </summary>
        private GameObject gameObject;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameObject_">Reference to the GameObject.</param>
        /// <param name="RandomPositionMin_">Random position range (Min).</param>
        /// <param name="RandomPositionMax_">Random position range (Max).</param>
        /// <param name="RandomEulerAnglesMin_">Random eular angle range (Min).</param>
        /// <param name="RandomEulerAnglesMax_">Random eular angle range (Max).</param>
        /// <param name="RandomForceMin_">Random force range (Min).</param>
        /// <param name="RandomForceMax_">Random force range (Max).</param>
        public TransformReinitializor(
            GameObject gameObject_,
            Vector3 RandomPositionMin_, Vector3 RandomPositionMax_,
            Vector3 RandomEulerAnglesMin_, Vector3 RandomEulerAnglesMax_,
            Vector3 RandomForceMin_, Vector3 RandomForceMax_)
        {
            gameObject = gameObject_;

            RandomPositionMin = RandomPositionMin_;
            RandomPositionMax = RandomPositionMax_;

            RandomEulerAnglesMin = RandomEulerAnglesMin_;
            RandomEulerAnglesMax = RandomEulerAnglesMax_;

            RandomForceMin = RandomForceMin_;
            RandomForceMax = RandomForceMax_;

            OriginalPosition    = gameObject.transform.position;
            OriginalEulerAngles = gameObject.transform.eulerAngles;
        }

        /// <summary>
        /// Every Reinitializor should implement this method.
        /// </summary>
        override public void
        Reinitialize()
        {
            // Reinitialize the gameObject to a position and EulerAngles that has some randomness
            gameObject.transform.position = new Vector3(
                OriginalPosition.x + Utils.RandomSign_Float() * Random.Range(RandomPositionMin.x, RandomPositionMax.x),
                OriginalPosition.y + Utils.RandomSign_Float() * Random.Range(RandomPositionMin.y, RandomPositionMax.y),
                OriginalPosition.z + Utils.RandomSign_Float() * Random.Range(RandomPositionMin.z, RandomPositionMax.z)
            );

            gameObject.transform.eulerAngles = new Vector3(
                OriginalEulerAngles.x + Utils.RandomSign_Float() * Random.Range(RandomEulerAnglesMin.x,
                RandomEulerAnglesMax.x),
                OriginalEulerAngles.y + Utils.RandomSign_Float() * Random.Range(RandomEulerAnglesMin.y,
                RandomEulerAnglesMax.y),
                OriginalEulerAngles.z + Utils.RandomSign_Float() * Random.Range(RandomEulerAnglesMin.z,
                RandomEulerAnglesMax.z)
            );

            if (gameObject.GetComponent<Rigidbody>() != null) {
                gameObject.GetComponent<Rigidbody>().velocity        = Vector3.zero;
                gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                gameObject.GetComponent<Rigidbody>().AddForce(
                    new Vector3(
                        Utils.RandomSign_Float() * Random.Range(RandomForceMin.x, RandomForceMax.x),
                        Utils.RandomSign_Float() * Random.Range(RandomForceMin.y, RandomForceMax.y),
                        Utils.RandomSign_Float() * Random.Range(RandomForceMin.z, RandomForceMax.z)
                    )
                );
            }
        }
    }
}
