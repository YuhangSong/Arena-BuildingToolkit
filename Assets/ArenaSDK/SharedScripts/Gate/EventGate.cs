using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

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

        public bool IsMatchNodeCoordinate = false;

        /// <summary>
        /// </summary>
        public bool IsKill = true;

        /// <summary>
        /// </summary>
        public float IncrementHealth = 0f;

        /// <summary>
        /// </summary>
        public float IncrementEnergy = 0f;

        protected override void
        TrigEvent(GameObject other)
        {
            if (TrigTags.Contains(other.tag)) {
                ArenaNode OtherNode = Utils.GetBottomLevelArenaNodeInGameObject(other);
                if (OtherNode == null) {
                    return;
                }

                if (IsMatchNodeCoordinate) {
                    ArenaNode ThisNode = Utils.GetBottomLevelArenaNodeInGameObject(gameObject);
                    if (ThisNode == null) {
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
                    OtherNode.Kill();
                }
                if (IncrementHealth != 0f) {
                    OtherNode.IncrementHealth(IncrementHealth);
                }
                if (IncrementEnergy != 0f) {
                    OtherNode.IncrementEnergy(IncrementEnergy);
                }
            }
        } // TrigEvent
    }
}
