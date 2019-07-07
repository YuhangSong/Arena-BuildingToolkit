using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

namespace Arena
{
    /// <summary>
    /// Attach this script to a GameObject A, when A colide GameObject C with tag of TrigTags,
    /// ArenaTeam B2 (if B2 is the parent of C) will be killed.
    /// </summary>
    public class KillGate : Gate
    {
        /// <summary>
        /// Tag of C that will trig the gate.
        /// </summary>
        public List<string> TrigTags = new List<string>();

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
                Utils.GetBottomLevelArenaNodeInGameObject(other).Kill();
            }
        } // TrigEvent
    }
}
