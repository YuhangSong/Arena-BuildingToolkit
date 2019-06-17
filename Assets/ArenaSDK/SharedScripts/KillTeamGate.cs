using UnityEngine;

namespace Arena {
    /// <summary>
    /// Kill an team (specified by TeamIDToMatch) if something collid with the object attached
    /// with this script.
    /// This is normally used for a target the whole team is trying to acheive (avoid).
    /// </summary>
    public class KillTeamGate : ArenaBase
    {
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
        public ComparingObjectTypes ComparingObjectType = ComparingObjectTypes.self;

        /// <summary>
        /// Tag of the object that will trig team's death.
        /// </summary>
        public string TrigTag = "Ball";

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
                if (TeamIDToMatch == -2) {
                    ArenaTeam ArenaTeam_;
                    if (ComparingObjectType == ComparingObjectTypes.self) {
                        ArenaTeam_ = GetComponentInParent<ArenaTeam>();
                    } else if (ComparingObjectType == ComparingObjectTypes.other) {
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
            }
        } // TrigEvent
    }
}
