using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using Arena;

namespace Arena
{
    public class CTFAgent : BasicAgent
    {
        public GameObject Flag;
        private TransformReinitializor FlagReinitializor;

        public override void
        InitializeAgent()
        {
            base.InitializeAgent();
            this.GetComponentInChildren<CarryFlag>().disboardflag();
            Vector3 positionvector = new Vector3(0.0f, 0.0f, 4.0f);
            FlagReinitializor = new TransformReinitializor(
                Flag,
                Vector3.zero, positionvector,
                Vector3.zero, Vector3.zero,
                Vector3.zero, Vector3.zero);
        }

        public override void
        AgentReset()
        {
            base.AgentReset();
            FlagReinitializor.Reinitialize();
        }
    }
}
