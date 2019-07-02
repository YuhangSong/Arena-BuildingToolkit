using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

namespace Arena
{
    /// <summary>
    /// Attach this script to a GameObject (A), so that A can kill the agent (B) when colide GameObject C, where C is a child of B.
    /// Example:
    ///   A: bullet
    ///   B: ArenaAgent B;
    ///   C: body of agent B
    /// </summary>
    public class KillAgentGate : Gate
    {
        /// <summary>
        /// Tag of the C that will trig agent death
        /// Examples:
        ///   Player,
        ///   Body
        /// </summary>
        public List<string> TrigTags = new List<string>();

        /// <summary>
        /// Make it only takes effect for specific TeamID.
        ///   -2: take effect for any agent within the team that is the parent of C or A (depending on TeamIDComparingObjectType).
        ///   -1: take effect for any agent within any team.
        ///   non-negative number: take effect for the specific number of TeamID.
        ///   other values: invalid.
        /// </summary>
        public int TeamIDToMatch = -1;

        /// <summary>
        /// When TeamIDToMatch = -2, take effect for team that is the parent of A (self) or C (other)
        /// </summary>
        public ComparingObjectTypes TeamIDComparingObjectType = ComparingObjectTypes.self;

        /// <summary>
        /// Make it only takes effect for specific AgentID.
        ///   -2: take effect for the agent that is the parent of this object.
        ///   -1: take effect for any agent.
        ///   non-negative number: take effect for the specific number of AgentID.
        ///   other values: invalid.
        /// </summary>
        public int AgentIDToMatch = -1;

        /// <summary>
        /// When AgentIDToMatch = -2, take effect for team that is the parent of this object (self) or the collider (other)
        /// </summary>
        public ComparingObjectTypes AgentIDComparingObjectType = ComparingObjectTypes.self;

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
                ArenaAgent OtherArenaAgent_ = other.GetComponentInParent<ArenaAgent>();

                if (TeamIDToMatch == -2) {
                    ArenaTeam ArenaTeam_;
                    if (TeamIDComparingObjectType == ComparingObjectTypes.self) {
                        ArenaTeam_ = GetComponentInParent<ArenaTeam>();
                    } else if (TeamIDComparingObjectType == ComparingObjectTypes.other) {
                        ArenaTeam_ = other.GetComponentInParent<ArenaTeam>();
                    } else {
                        ArenaTeam_ = null;
                        Debug.LogError("Invalid TeamIDComparingObjectType");
                    }

                    if (OtherArenaAgent_.getTeamID() != ArenaTeam_.getTeamID()) {
                        return;
                    }
                } else if (TeamIDToMatch == -1) {
                    //
                } else if (TeamIDToMatch >= 0) {
                    if (OtherArenaAgent_.getTeamID() != TeamIDToMatch) {
                        return;
                    }
                } else {
                    Debug.LogError("Invalid TeamIDToMatch");
                }

                if (AgentIDToMatch == -2) {
                    ArenaAgent ArenaAgent_;
                    if (AgentIDComparingObjectType == ComparingObjectTypes.self) {
                        ArenaAgent_ = GetComponentInParent<ArenaAgent>();
                    } else if (AgentIDComparingObjectType == ComparingObjectTypes.other) {
                        ArenaAgent_ = other.GetComponentInParent<ArenaAgent>();
                    } else {
                        ArenaAgent_ = null;
                        Debug.LogError("Invalid AgentIDComparingObjectType");
                    }

                    if (OtherArenaAgent_.getAgentID() != ArenaAgent_.getAgentID()) {
                        return;
                    }
                } else if (AgentIDToMatch == -1) {
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
        } // TrigEvent
    }
}
