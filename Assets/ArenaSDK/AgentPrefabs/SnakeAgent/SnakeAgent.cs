using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arena
{
    public class SnakeAgent : ArenaAgent
    {
        [Header("Snake Settings")][Space(10)]

        public float MaxNumBodies   = 20f;
        public float MovementSpeed  = 5f;
        public float TurnSpeed      = 150f;
        public int InitialNumBodies = 3;
        public List<GameObject> Bodies;

        public GameObject Head;
        public GameObject BodyPrefab;
        public float Distance = 0.5f;

        private float CurrentRotation;

        override public void
        IncrementAttribute(string Key_, float IncrementValue_)
        {
            base.IncrementAttribute(Key_, IncrementValue_);
            if (Key_ == "Nutrition") {
                int NumSegmentExpected_ = (int) (Attributes["Nutrition"] / (1f / MaxNumBodies));
                while (GetNumSegment() < NumSegmentExpected_) {
                    AddSegment();
                }
            }
        }

        override protected void
        DiscreteStep(int Action_)
        {
            base.DiscreteStep(Action_);

            switch (Action_) {
                case TurnRight:
                    CurrentRotation += TurnSpeed * Time.deltaTime;
                    break;
                case TurnLeft:
                    CurrentRotation -= TurnSpeed * Time.deltaTime;
                    break;
                default:
                    break;
            }
        } // DiscreteStep

        public void
        FixedUpdate()
        {
            Rotate();
            MoveFWD();
        }

        void
        Rotate()
        {
            Head.transform.rotation =
              Quaternion.Euler(new Vector3(Head.transform.rotation.x, CurrentRotation, Head.transform.rotation.z));
        }

        void
        MoveFWD()
        {
            Head.transform.position += Head.transform.forward * MovementSpeed * Time.deltaTime;
        }

        public override void
        AgentReset()
        {
            base.AgentReset();

            for (int i = 0; i < Bodies.Count; i++) {
                Destroy(Bodies[i]);
            }

            Bodies.Clear();

            for (int i = 0; i < InitialNumBodies; i++) {
                GameObject bp =
                  Instantiate(BodyPrefab,
                    new Vector3(Head.transform.position.x, Head.transform.position.y,
                    Head.transform.position.z - (i + 1) * Distance), Quaternion.identity);
                bp.transform.SetParent(transform);
                Bodies.Add(bp);
            }
        }

        public void
        AddSegment()
        {
            int newPartN = Bodies.Count;
            Transform lastSegment = Bodies[newPartN - 1].transform;
            Vector3 newPos        = lastSegment.position - lastSegment.forward.normalized * Distance;
            GameObject bp         = Instantiate(BodyPrefab, newPos, Quaternion.identity);

            bp.transform.SetParent(transform);
            Bodies.Add(bp);
        }

        public int
        GetNumSegment()
        {
            return Bodies.Count;
        }
    }
}
