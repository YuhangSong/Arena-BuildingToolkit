using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilliardsGate : MonoBehaviour
{

    void OnCollisionEnter(Collision other)
    {
        // string tagger = other.gameObject.tag;
        // string turn = GameObject.FindWithTag("GlobalManager").GetComponent<BilliardsGlobalManager>().get_turn();
        //
        // if (tagger.Contains("Ball")){
        //
        //     // ball disapear
        //     other.gameObject.SetActive(false);
        //
        //     if (tagger=="WhiteBall")
        //     {
        //         // whiteball in gate, lost
        //         GameObject.FindWithTag(turn).GetComponent<BilliardsAgent>().trig_loss();
        //     }
        //     else if (tagger == "BallK" || tagger == "BallS")
        //     {
        //         if (GameObject.FindWithTag("AgentA").GetComponent<BilliardsAgent>().getMyBall() == "None" ||
        //         GameObject.FindWithTag("AgentB").GetComponent<BilliardsAgent>().getMyBall() == "None")
        //         {
        //             // ball has not been set, set myBall
        //             GameObject.FindWithTag(turn).GetComponent<BilliardsAgent>().setMyBall(other.gameObject.tag);
        //             GameObject.FindWithTag(turn).GetComponent<BilliardsAgent>().Competitor.setMyBall(
        //                 GameObject.FindWithTag("GlobalManager").GetComponent<BilliardsGlobalManager>().getAnotherBall(
        //                     other.gameObject.tag
        //                     )
        //                 );
        //         }
        //
        //         // hit one the correct ball, give score
        //         if (GameObject.FindWithTag("AgentB").GetComponent<BilliardsAgent>().getMyBall() == tagger)
        //         {
        //             GameObject.FindWithTag("AgentB").GetComponent<BilliardsAgent>().addMyScore();
        //         }
        //
        //         if (GameObject.FindWithTag("AgentA").GetComponent<BilliardsAgent>().getMyBall() == tagger)
        //         {
        //             GameObject.FindWithTag("AgentA").GetComponent<BilliardsAgent>().addMyScore();
        //         }
        //     }
        //     else if (tagger == "BlackBall")
        //     {
        //         if (GameObject.FindWithTag(turn).GetComponent<BilliardsAgent>().getMyScore() <
        //             GameObject.FindWithTag("GlobalManager").GetComponent<BilliardsGlobalManager>().getBallTotal())
        //         {
        //             // still got ball, but hit the balck ball, win
        //             GameObject.FindWithTag(turn).GetComponent<BilliardsAgent>().trig_loss();
        //         }
        //         else
        //         {
        //             // no balls and hit the black ball, win
        //             GameObject.FindWithTag(turn).GetComponent<BilliardsAgent>().trig_win();
        //         }
        //     }
        // }
    }
}
