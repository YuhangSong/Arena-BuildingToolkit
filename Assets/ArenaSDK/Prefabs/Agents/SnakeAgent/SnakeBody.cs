using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arena
{
    public class SnakeBody : ArenaMovingObject
    {
        /// <summary>
        /// </summary>
        private SnakeAgent SnakeAgentParent;

        /// <summary>
        /// </summary>
        private int MyPlace;

        /// <summary>
        /// </summary>
        private float SmoothTime;

        /// <summary>
        /// </summary>
        public void
        Initialize(SnakeAgent SnakeAgentParent_, int MyPlace_, float SmoothTime_)
        {
            base.Initialize();
            SnakeAgentParent = SnakeAgentParent_;
            MyPlace    = MyPlace_;
            SmoothTime = SmoothTime_;
        }

        /// <summary>
        /// </summary>
        private Vector3 CurrentVelocity = Vector3.zero;

        /// <summary>
        /// </summary>
        public override void
        UpdateMovement()
        {
            base.UpdateMovement();

            GameObject previousPart;
            if (MyPlace < 1) {
                previousPart = SnakeAgentParent.GetHead();
            } else {
                previousPart = SnakeAgentParent.GetBodies()[MyPlace - 1];
            }

            transform.position = Vector3.SmoothDamp(
                transform.position,
                previousPart.transform.position - previousPart.transform.forward.normalized * SnakeAgentParent.DistanceBetweenBodies,
                ref CurrentVelocity,
                SmoothTime, Mathf.Infinity,
                DeltTimeUpdateMovement);
            transform.LookAt(previousPart.transform.position);
        }
    }
}
