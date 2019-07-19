using System.Collections.Generic;
using UnityEngine;

namespace Arena
{
    public class SnakeAgent : ArenaAgent
    {
        [Header("Snake Settings")][Space(10)]

        /// <summary>
        /// </summary>
        public float MaxNumBodies = 20f;

        /// <summary>
        /// </summary>
        public float MovementSpeed = 5f;

        /// <summary>
        /// </summary>
        public float TurnSpeed = 150f;

        /// <summary>
        /// </summary>
        public float DistanceBetweenBodies = 0.5f;

        /// <summary>
        /// </summary>
        public GameObject Head;

        /// <summary>
        /// </summary>
        public GameObject BodyPrefab;

        /// <summary>
        /// </summary>
        private List<GameObject> Bodies = new List<GameObject>();

        /// <summary>
        /// </summary>
        public GameObject
        GetHead()
        {
            return Head;
        }

        /// <summary>
        /// </summary>
        public List<GameObject>
        GetBodies()
        {
            return Bodies;
        }

        /// <summary>
        /// </summary>
        public int
        GetNumBodies()
        {
            return Bodies.Count;
        }

        /// <summary>
        /// </summary>
        override public void
        IncrementAttribute(string Key_, float IncrementValue_)
        {
            base.IncrementAttribute(Key_, IncrementValue_);
            if (Key_ == "Nutrition") {
                UpdateBodiesFromNutrition();
            }
        }

        /// <summary>
        /// </summary>
        private void
        UpdateBodiesFromNutrition()
        {
            int NumBodiesExpected_ = (int) (Attributes["Nutrition"] / (1f / MaxNumBodies));

            while (GetNumBodies() < NumBodiesExpected_) {
                AddBody();
            }
        }

        /// <summary>
        /// </summary>
        override protected void
        DiscreteStep(int Action_)
        {
            base.DiscreteStep(Action_);

            switch (Action_) {
                case TurnRight:
                    Head.transform.eulerAngles = new Vector3(Head.transform.eulerAngles.x,
                        Head.transform.eulerAngles.y + TurnSpeed * DeltTimeAgentStep,
                        Head.transform.eulerAngles.z);
                    break;
                case TurnLeft:
                    Head.transform.eulerAngles = new Vector3(Head.transform.eulerAngles.x,
                        Head.transform.eulerAngles.y - TurnSpeed * DeltTimeAgentStep,
                        Head.transform.eulerAngles.z);
                    break;
                default:
                    break;
            }

            // keep moving forward
            Head.transform.position += Head.transform.forward * MovementSpeed * DeltTimeAgentStep;

            // update the movement of the bodies
            foreach (GameObject Body_ in Bodies) {
                Body_.GetComponent<SnakeBody>().UpdateMovement();
            }
        } // DiscreteStep

        /// <summary>
        /// </summary>
        public override void
        AgentReset()
        {
            base.AgentReset();

            // clear Bodies
            for (int i = 0; i < Bodies.Count; i++) {
                Destroy(Bodies[i]);
            }
            Bodies.Clear();

            // for initial bodies
            UpdateBodiesFromNutrition();
        }

        /// <summary>
        /// </summary>
        public void
        AddBody()
        {
            Transform LastBodyTransform;

            if (GetNumBodies() < 1) {
                LastBodyTransform = Head.transform;
            } else {
                LastBodyTransform = Bodies[GetNumBodies() - 1].transform;
            }

            GameObject Body_ = Instantiate(
                BodyPrefab,
                LastBodyTransform.position,
                LastBodyTransform.rotation);
            Body_.GetComponent<SnakeBody>().Initialize(this, GetNumBodies(), DistanceBetweenBodies / 5f);

            // do make sure any component of the agent is the transform child of the agent, so that Arena can handle the social tree correctly
            Body_.transform.SetParent(transform);

            // add to bodies list
            Bodies.Add(Body_);
        }
    }
}
