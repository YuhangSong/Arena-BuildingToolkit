using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

namespace Arena {
    /// <summary>
    /// Kill an team (specified by TeamIDToMatch) if something collid with the object attached
    /// with this script.
    /// This is normally used for a target the whole team is trying to acheive (avoid).
    /// </summary>
    public class KillTeamGate : Gate
    {
        /// <summary>
        /// Tag of the object that will trig team's death.
        /// </summary>
        public List<string> TrigTags = new List<string>();

        /// <summary>
        /// Make it only takes effect for specific TeamID.
        ///   -2: take effect for team that is the parent of this object.
        ///   -1: no effect.
        ///   non-negative number: take effect for the specific number of TeamID.
        ///   other values: invalid.
        /// </summary>
        public int TeamIDToMatch = -2;

        /// <summary>
        /// When TeamIDToMatch = -2, take effect for team that is the parent of this object (self) or the collider (other)
        /// </summary>
        public ComparingObjectTypes TeamIDComparingObjectType = ComparingObjectTypes.self;

        protected override void
        TrigEvent(GameObject other)
        {
            // take effect when other is of TrigTags
            foreach (string Tag_ in TrigTags) {
                if (other.CompareTag(Tag_)) {
                    break;
                }
            }

            if (TeamIDToMatch == -2) {
                ArenaTeam ArenaTeam_;
                if (TeamIDComparingObjectType == ComparingObjectTypes.self) {
                    ArenaTeam_ = GetComponentInParent<ArenaTeam>();
                } else if (TeamIDComparingObjectType == ComparingObjectTypes.other) {
                    ArenaTeam_ = other.GetComponentInParent<ArenaTeam>();
                } else {
                    ArenaTeam_ = null;
                }

                if (ArenaTeam_ == null) {
                    Debug.LogError(
                        "Setting TeamIDToMatch to -2 requires either the object it self or the collider to be the child of a ArenaTeam (depend on the ComparingObjectType)");
                    return;
                }

                globalManager.KillTeam(
                    ArenaTeam_.getTeamID()
                );

                return;
            } else if (TeamIDToMatch == -1) {
                return;
            } else if (TeamIDToMatch >= 0) {
                globalManager.KillTeam(TeamIDToMatch);
                return;
            } else {
                Debug.LogError("Not a valid TeamIDToMatch");
                return;
            }
        } // TrigEvent
    }
}
