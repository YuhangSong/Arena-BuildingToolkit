using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arena
{
    public class SnakeHead : Gate
    {
        protected override void
        TrigEvent(GameObject other)
        {
            // if collid with body, according to https://www.youtube.com/watch?v=35lpSHgvibU
            if (other.CompareTag("Body")) {
                ArenaNode OtherNode = Utils.GetBottomLevelArenaNodeInGameObject(other);
                ArenaNode ThisNode  = Utils.GetBottomLevelArenaNodeInGameObject(gameObject);
                List<int> ThisNodeCoordinate  = ThisNode.GetCoordinate();
                List<int> OtherNodeCoordinate = OtherNode.GetCoordinate();
                if (!Utils.IsListEqual(ThisNodeCoordinate, OtherNodeCoordinate,
                  Mathf.Min(ThisNodeCoordinate.Count, OtherNodeCoordinate.Count)))
                {
                    ThisNode.Kill();
                }
            }
        } // TrigEvent
    }
}
