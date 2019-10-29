using UnityEngine;

namespace Arena
{
    public class TemporallyAlive : ArenaBase
    {
        // for how long the fire lives, set to non-positive number to make it live forever
        public float AliveDuration = 10f;

        void
        Start()
        {
            Initialize();
        }

        public override void
        Initialize()
        {
            base.Initialize();

            // destroy self after AliveDuration
            if (AliveDuration > 0.0f) {
                Destroy(gameObject, AliveDuration);
            }
        }
    }
}
