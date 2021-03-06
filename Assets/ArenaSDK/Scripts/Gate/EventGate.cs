using System.Collections.Generic;
using UnityEngine;

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

        /// <summary>
        /// </summary>
        [Tooltip("If only take effect when node coordinates are matched")]
        public bool IsMatchNodeCoordinate = false;

        /// <summary>
        /// </summary>
        [Tooltip("If only take effect when node coordinates are not matched")]
        public bool IsDisMatchNodeCoordinate = false;

        /// <summary>
        /// </summary>
        public bool IsKill = true;

        [SerializeField]
        public MyDictionary.StringStringDictionary IncrementAttributes;

        /// <summary>
        /// The scale of incremented value is based on transform.localScale.y
        /// </summary>
        public bool IncrementAttributesValueBasedOnScale = false;

        public bool IsCarried = false;

        /// <summary>
        /// The relative position of this GameObject to the GameObject to follow.
        /// </summary>
        private Vector3 RelativePosition;

        /// <summary>
        /// The GameObject to follow.
        /// </summary>
        private GameObject ToFollow;

        private ArenaNode OtherNode;
        private ArenaNode ThisNode;

        protected virtual void
        BeforeTrigKill(){ }

        protected override void
        TrigEvent(GameObject other)
        {
            if (TrigTags.Contains(other.tag)) {
                OtherNode = Utils.GetBottomLevelArenaNodeInGameObject(other);
                ThisNode  = Utils.GetBottomLevelArenaNodeInGameObject(gameObject);

                ArenaNode SubjectNode;
                if (SubjectType == SubjectTypes.This) {
                    SubjectNode = ThisNode;
                } else {
                    SubjectNode = OtherNode;
                }

                if (SubjectNode == null) {
                    return;
                }


                if (IsMatchNodeCoordinate || IsDisMatchNodeCoordinate) {
                    bool IsNodeCoordinateMatched = false;

                    if ((ThisNode == null) || (OtherNode == null)) {
                        Debug.LogError("Event gate has to be located under ArenaNode to take effect.");
                    } else {
                        List<int> ThisNodeCoordinate  = ThisNode.GetCoordinate_ParentToChild();
                        List<int> OtherNodeCoordinate = OtherNode.GetCoordinate_ParentToChild();
                        if (Utils.IsListEqual(ThisNodeCoordinate, OtherNodeCoordinate,
                          Mathf.Min(ThisNodeCoordinate.Count, OtherNodeCoordinate.Count)))
                        {
                            IsNodeCoordinateMatched = true;
                        } else {
                            IsNodeCoordinateMatched = false;
                        }
                    }

                    if (IsMatchNodeCoordinate) {
                        if (!IsNodeCoordinateMatched) {
                            return;
                        }
                    }

                    if (IsDisMatchNodeCoordinate) {
                        if (IsNodeCoordinateMatched) {
                            return;
                        }
                    }
                }

                if (IsKill) {
                    BeforeTrigKill();
                    SubjectNode.Kill();
                }

                foreach (string Attribute_ in IncrementAttributes.Keys) {
                    if (float.Parse(IncrementAttributes[Attribute_]) != 0f) {
                        float Scale_ = 1f;
                        if (IncrementAttributesValueBasedOnScale) {
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
