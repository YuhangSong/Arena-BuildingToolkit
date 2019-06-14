using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using Arena;

namespace Arena
{
    public class BlowBlowAgent : BasicAgent {
        public GameObject Bullet_Emitter;
        public GameObject Bullet_Emitter1;
        public GameObject Bullet_Emitter2;
        public new GameObject Bullet;

        override protected void
        DiscreteStep(int Action_)
        {
            base.DiscreteStep(Action_);

            switch (Action_) {
                case Forward:
                    for (int i = 0; i < 2; i++) {
                        GameObject Temp_Bullet_Handeler;
                        Temp_Bullet_Handeler = Instantiate(Bullet, Bullet_Emitter.transform.position,
                            Bullet_Emitter.transform.rotation) as GameObject;
                        var vs =
                          Vector3.Normalize(Vector3.Slerp((Bullet_Emitter1.transform.position
                            - Bullet_Emitter.transform.position),
                            (Bullet_Emitter2.transform.position - Bullet_Emitter.transform.position), Random.value));
                        Temp_Bullet_Handeler.GetComponent<Rigidbody>().velocity = vs * 10f;
                        Destroy(Temp_Bullet_Handeler, 3.0f);
                    }
                    Player.GetComponentInChildren<Rigidbody>().AddForce(Player.transform.TransformVector(
                          Vector3.right * 40f));
                    Player.GetComponentInChildren<Rigidbody>().angularVelocity = Vector3.zero;
                    break;
                default:
                    break;
            }
        } // Step
    }
}
