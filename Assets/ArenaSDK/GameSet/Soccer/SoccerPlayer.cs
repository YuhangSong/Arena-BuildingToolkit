using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerPlayer : MonoBehaviour
{
    protected void
    LateUpdate()
    {
        // to force the agent to have no rotation on x and z axises
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }
}
