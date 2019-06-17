using UnityEngine;

namespace Arena
{
    /// <summary>
    /// Kill an agent if something collid with the object attached
    /// with this script.
    /// </summary>
    public class KillAgentGate : ArenaBase
    {
        /// <summary>
        /// Tag of the object that will trig agent death.
        /// </summary>
        public string TrigTag = "Player";

        /// <summary>
        /// Make it only takes effect for specific TeamID.
        ///   -2: take effect for any agent within the team that is the parent of this object.
        ///   -1: take effect for any agent within any team.
        ///   non-negative number: take effect for the specific number of TeamID.
        ///   other values: invalid.
        /// </summary>
        public int TeamIDToMatch = -1;

        /// <summary>
        /// When TeamIDToMatch = -2, take effect for team that is the parent of this object (self) or the collider (other)
        /// </summary>
        public ComparingObjectTypes ComparingObjectType = ComparingObjectTypes.self;

        /// <summary>
        /// Make it only takes effect for specific AgentID.
        ///   -1: take effect for any agent.
        ///   non-negative number: take effect for the specific number of AgentID.
        ///   other values: invalid.
        /// </summary>
        public int AgentIDToMatch = -1;

        void
        Start()
        {
            Initialize();
        }

        void
        OnCollisionEnter(Collision other)
        {
            TrigEvent(other.gameObject);
        }

        void
        OnTriggerEnter(Collider other)
        {
            TrigEvent(other.gameObject);
        }

        private void
        TrigEvent(GameObject other)
        {
            if (other.CompareTag(TrigTag)) {
                ArenaAgent ArenaAgent_ = other.GetComponentInParent<ArenaAgent>();

                if (TeamIDToMatch == -2) {
                    ArenaTeam ArenaTeam_;
                    if (ComparingObjectType == ComparingObjectTypes.self) {
                        ArenaTeam_ = GetComponentInParent<ArenaTeam>();
                    } else if (ComparingObjectType == ComparingObjectTypes.other) {
                        ArenaTeam_ = other.GetComponentInParent<ArenaTeam>();
                    } else {
                        ArenaTeam_ = null;
                    }

                    if (ArenaAgent_.getTeamID() != ArenaTeam_.getTeamID()) {
                        return;
                    }
                } else if (TeamIDToMatch == -1) {
                    //
                } else if (TeamIDToMatch >= 0) {
                    if (ArenaAgent_.getTeamID() != TeamIDToMatch) {
                        return;
                    }
                }

                if (AgentIDToMatch == -1) {
                    //
                } else if (AgentIDToMatch >= 0) {
                    if (ArenaAgent_.getAgentID() != AgentIDToMatch) {
                        return;
                    }
                } else {
                    Debug.LogError("Invalid AgentIDToMatch");
                }

                globalManager.KillAgent(ArenaAgent_.getTeamID(), ArenaAgent_.getAgentID());
            }
        } // TrigEvent
    }
}
