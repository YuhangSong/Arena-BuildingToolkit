using UnityEngine;

namespace Arena
{
    public class SoccerAgent : BasicAgent
    {
        protected const int KickLeft  = CustomizeActionStartAt;
        protected const int KickRight = CustomizeActionStartAt + 1;

        // private float TurnForce = 3f;
        // private float MoveForce = 50f;

        // PlayerReinitializor = new TransformReinitializor(
        //     Player,
        //     Vector3.zero, new Vector3(0.8f, 0f, 1f),
        //     Vector3.zero, new Vector3(0f, 20f, 0f),
        //     Vector3.zero, Vector3.zero);

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
