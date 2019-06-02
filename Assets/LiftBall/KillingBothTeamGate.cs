using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

namespace Arena {
    /// <summary>
    /// Kill an team (specified by KillingTeamID) if something collid with the object attached
    /// with this script.
    /// This is normally used for a target the whole team is trying to acheive (avoid).
    /// </summary>
    public class KillingBothTeamGate : ArenaBase
    {
        /// <summary>
        /// The TeamID of which the team will be killed.
        /// </summary>
        public int KillingTeamID0 = 0;
        public int KillingTeamID1 = 1;

        /// <summary>
        /// Tag of the object that will trig team's death.
        /// </summary>
        public string TrigTag = "Ball";

        /// <summary>
        /// Collision tha trig the event.
        /// </summary>
        void
        OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag(TrigTag)) {
                globalManager.KillTeam(KillingTeamID0);
                globalManager.KillTeam(KillingTeamID1);
            }
        }
    }
}
