using UnityEngine;

namespace Arena {
    /// <summary>
    /// Kill an team (specified by KillingTeamID) if something collid with the object attached
    /// with this script.
    /// This is normally used for a target the whole team is trying to acheive (avoid).
    /// </summary>
    public class KillTeamGate : ArenaBase
    {
        /// <summary>
        /// The TeamID of which the team will be killed.
        /// -1 means to kill the team that is the parent of the collider.
        /// </summary>
        public int KillingTeamID = -1;

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
                if (KillingTeamID >= 0) {
                    globalManager.KillTeam(KillingTeamID);
                } else {
                    print(globalManager);

                    globalManager.KillTeam(
                        other.GetComponentInParent<ArenaTeam>().getTeamID()
                    );
                }
            }
        }
    }
}
