using System.Collections.Generic;
using UnityEngine;

namespace Arena
{
    public class TargetBox : EventGate
    {
        public ArenaNode CollaborativeParentNode;
        public MaterialReinitializor MaterialReinitializor_;
        public int MatchMaterialIndex;

        protected override void
        BeforeTrigKill()
        {
            base.BeforeTrigKill();
            if (MatchMaterialIndex == MaterialReinitializor_.GetCurrentMaterialIndex()) {
                CollaborativeParentNode.AddReward(1f * CollaborativeParentNode.RewardSchemeScale);
            } else  {
                CollaborativeParentNode.AddReward(-1f * CollaborativeParentNode.RewardSchemeScale);
            }
        } // TrigEvent
    }
}
