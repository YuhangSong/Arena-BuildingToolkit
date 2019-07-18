using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arena
{
    public class SnakeBody : MonoBehaviour
    {
        private SnakeAgent SnakeAgentParent;
        private int MyPlace = -1;
        private float SmoothTime;

        public void
        Initialize(SnakeAgent SnakeAgentParent_, int MyPlace_, float SmoothTime_)
        {
            SnakeAgentParent = SnakeAgentParent_;
            MyPlace    = MyPlace_;
            SmoothTime = SmoothTime_;
        }

        private Vector3 CurrentVelocity = Vector3.zero;

        private float LastTimeAgentStep   = -1f;
        protected float DeltTimeAgentStep = 0f;

        public void
        UpdateBody()
        {
            if (LastTimeAgentStep == -1f) {
                DeltTimeAgentStep = 0f;
            } else {
                DeltTimeAgentStep = Time.time - LastTimeAgentStep;
            }
            LastTimeAgentStep = Time.time;

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
                DeltTimeAgentStep);
            transform.LookAt(previousPart.transform.position);
        }
    }
}
