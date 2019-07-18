using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arena
{
    public class SnakeBodyPart : MonoBehaviour
    {
        private SnakeAgent SnakeAgent_;
        private GameObject SnakeHead;
        private float Distance;
        private int MyPlace = -1;
        private Vector3 MoveVector;

        void
        FixedUpdate()
        {
            if (SnakeAgent_ == null) {
                SnakeAgent_ = GetComponentInParent<SnakeAgent>();
                if (SnakeAgent_ == null) {
                    return;
                }
                SnakeHead = SnakeAgent_.Head;
                Distance  = SnakeAgent_.Distance;
                for (int i = 0; i < SnakeAgent_.Bodies.Count; i++) {
                    if (gameObject == SnakeAgent_.Bodies[i]) {
                        MyPlace = i;
                    }
                }
            } else {
                if (MyPlace == 0) {
                    // Vector3 MoveVector = new Vector3(0, 0, 0);
                    transform.position = Vector3.SmoothDamp(transform.position, SnakeHead.transform.position,
                        ref MoveVector,
                        Distance / 5);
                    transform.LookAt(SnakeHead.transform.position);
                } else {
                    // Vector3 MoveVector = new Vector3(0, 0, 0);
                    GameObject previousPart = SnakeAgent_.Bodies[MyPlace - 1];
                    transform.position = Vector3.SmoothDamp(transform.position, previousPart.transform.position,
                        ref MoveVector,
                        Distance / 5);
                    transform.LookAt(previousPart.transform.position);
                }
            }
        }
    }
}
