using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MyDictionary;

namespace Arena
{
    /// <summary>
    /// Attach this script to a GameObject A, when A colide GameObject C with tag of TrigTags,
    /// Event will be trigged for the bottom level ArenaNode B that is the parent of C.
    /// </summary>
    public class EventGate : Gate
    {
        /// <summary>
        /// Tag of C that will trig the gate.
        /// </summary>
        public List<string> TrigTags = new List<string>();

        public enum SubjectTypes {
            Other,
            This
        }

        public SubjectTypes SubjectType = SubjectTypes.Other;

        public bool IsMatchNodeCoordinate = false;

        /// <summary>
        /// </summary>
        public bool IsKill = true;

        [SerializeField]
        public MyDictionary.StringStringDictionary IncrementAttributes;

        public bool IsCarried = false;

        /// <summary>
        /// The relative position of this GameObject to the GameObject to follow.
        /// </summary>
        private Vector3 RelativePosition;

        /// <summary>
        /// The GameObject to follow.
        /// </summary>
        private GameObject ToFollow;

        protected override void
        TrigEvent(GameObject other)
        {
            if (TrigTags.Contains(other.tag)) {
                ArenaNode OtherNode = Utils.GetBottomLevelArenaNodeInGameObject(other);
                ArenaNode ThisNode  = Utils.GetBottomLevelArenaNodeInGameObject(gameObject);

                ArenaNode SubjectNode;
                if (SubjectType == SubjectTypes.This) {
                    SubjectNode = ThisNode;
                } else {
                    SubjectNode = OtherNode;
                }

                if (SubjectNode == null) {
                    return;
                }

                if (IsMatchNodeCoordinate) {
                    if ((ThisNode == null) || (OtherNode == null)) {
                        return;
                    } else {
                        List<int> ThisNodeCoordinate  = ThisNode.GetCoordinate();
                        List<int> OtherNodeCoordinate = OtherNode.GetCoordinate();
                        if (!Utils.IsListEqual(ThisNodeCoordinate, OtherNodeCoordinate,
                          Mathf.Min(ThisNodeCoordinate.Count, OtherNodeCoordinate.Count)))
                        {
                            return;
                        }
                    }
                }


                if (IsKill) {
                    SubjectNode.Kill();
                }

                foreach (string Attribute_ in IncrementAttributes.Keys) {
                    if (float.Parse(IncrementAttributes[Attribute_]) != 0f) {
                        float Scale_ = 1f;
                        if (CompareTag("Eatable")) {
                            Scale_ = transform.localScale.y;
                        }
                        SubjectNode.IncrementAttribute(Attribute_,
                          float.Parse(IncrementAttributes[Attribute_]) * Scale_);
                    }
                }

                if (IsCarried) {
                    ToFollow         = other;
                    RelativePosition = transform.position - ToFollow.transform.position;
                }
            }
        } // TrigEvent

        void
        FixedUpdate()
        {
            if (ToFollow != null) {
                transform.position = ToFollow.transform.position + RelativePosition;
            }
        }

        public void
        Reset()
        {
            ToFollow = null;
        }
    }
}
