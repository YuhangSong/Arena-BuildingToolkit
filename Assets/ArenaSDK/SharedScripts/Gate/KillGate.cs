using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

namespace Arena
{
    /// <summary>
    /// Attach this script to a GameObject A, when A colide GameObject C with tag of TrigTags,
    /// ArenaAgent B1 (if KillType==KillTypes.ArenaAgent) or ArenaTeam B2 (if KillType==KillTypes.ArenaAgent) will be killed.
    /// If TeamIDToMatch == -1,
    ///   B2 will be killed if B2 is the father of C;
    /// If TeamIDToMatch > -1,
    ///   B2 will be killed if the TeamID of B2 is TeamIDToMatch;
    /// If AgentIDToMatch == -1,
    ///   B1 will be killed if B1 is the father of C;
    /// If AgentIDToMatch > -1,
    ///   B1 will be killed if the AgentID of B1 is AgentIDToMatch;
    /// Example:
    ///   A: bullet, with the tag of "Bullet";
    ///   B1: ArenaAgent B1;
    ///   C: body of agent B, which is a child of B;
    ///   Other settings: as default;
    /// </summary>
    public class KillGate : Gate
    {
        public enum KillTypes {
            ArenaAgent,
            ArenaTeam
        }

        /// <summary>
        /// Kill ArenaAgent (if KillTypes.ArenaAgent) or ArenaTeam (if KillTypes.ArenaTeam).
        /// </summary>
        public KillTypes KillType = KillTypes.ArenaAgent;

        /// <summary>
        /// Tag of C that will trig the gate.
        /// </summary>
        public List<string> TrigTags = new List<string>();

        /// <summary>
        /// Make it only takes effect for specific TeamID.
        ///   -1: take effect for the ArenaTeam that is the father of C;
        ///   non-negative number: take effect for the specific ArenaTeam of TeamIDToMatch;
        ///   other values: invalid;
        /// </summary>
        public int TeamIDToMatch = -1;

        /// <summary>
        /// Make it only takes effect for specific AgentID.
        ///   -1: take effect for the ArenaAgent that is the father of C;
        ///   non-negative number: take effect for the specific ArenaAgent of AgentIDToMatch;
        ///   other values: invalid;
        /// </summary>
        public int AgentIDToMatch = -1;

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
                if (KillType == KillTypes.ArenaTeam) {
                    ArenaTeam OtherArenaTeam_ = other.GetComponentInParent<ArenaTeam>();

                    if (TeamIDToMatch == -1) {
                        //
                    } else if (TeamIDToMatch >= 0) {
                        if (OtherArenaTeam_.getTeamID() != TeamIDToMatch) {
                            return;
                        }
                    } else {
                        Debug.LogError("Invalid TeamIDToMatch");
                    }

                    globalManager.KillTeam(OtherArenaTeam_.getTeamID());
                } else if (KillType == KillTypes.ArenaAgent) {
                    ArenaAgent OtherArenaAgent_ = other.GetComponentInParent<ArenaAgent>();

                    if (TeamIDToMatch == -1) {
                        //
                    } else if (TeamIDToMatch >= 0) {
                        if (OtherArenaAgent_.getTeamID() != TeamIDToMatch) {
                            return;
                        }
                    } else {
                        Debug.LogError("Invalid TeamIDToMatch");
                    }

                    if (AgentIDToMatch == -1) {
                        //
                    } else if (AgentIDToMatch >= 0) {
                        if (OtherArenaAgent_.getAgentID() != AgentIDToMatch) {
                            return;
                        }
                    } else {
                        Debug.LogError("Invalid AgentIDToMatch");
                    }

                    globalManager.KillAgent(OtherArenaAgent_.getTeamID(), OtherArenaAgent_.getAgentID());
                }
            }
        } // TrigEvent
    }
}
