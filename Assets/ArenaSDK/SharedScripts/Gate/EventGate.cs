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
            bool IsTrig = false;

            // take effect when other is of TrigTags
            foreach (string Tag_ in TrigTags) {
                if (other.CompareTag(Tag_)) {
                    IsTrig = true;
                    break;
                }
            }

            if (IsTrig) {
                if (IsKill) {
                    Utils.GetBottomLevelArenaNodeInGameObject(other).Kill();
                }
                if (IncrementHealth != 0f) {
                    Utils.GetBottomLevelArenaNodeInGameObject(other).IncrementHealth(IncrementHealth);
                }
                if (IncrementEnergy != 0f) {
                    Utils.GetBottomLevelArenaNodeInGameObject(other).IncrementEnergy(IncrementEnergy);
                }
            }
        } // TrigEvent
    }
}
