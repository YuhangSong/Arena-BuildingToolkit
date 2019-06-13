using UnityEngine;

namespace Arena
{
    public class SoccerAgent : BasicAgent
    {
        protected const int KickLeft  = CustomizeActionStartAt;
        protected const int KickRight = CustomizeActionStartAt + 1;

        override protected void
        DiscreteStep(int Action_)
        {
            base.DiscreteStep(Action_);

            switch (Action_) {
                case KickLeft:
                    break;
                case KickRight:
                    break;
                default:
                    break;
            }
        } // DiscreteStep
    }
}
