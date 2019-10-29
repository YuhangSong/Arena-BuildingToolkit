using UnityEngine;

namespace Arena
{
    public class BlowBlowAgent : BasicAgent {
        public int NumBulletPerRelease = 3;
        public GameObject Bullet_Emitter;
        public GameObject Bullet_EmitterLeftEdge;
        public GameObject Bullet_EmitterRightEdge;

        override protected void
        DiscreteStep(int Action_)
        {
            base.DiscreteStep(Action_);

            switch (Action_) {
                case Forward:
                    for (int i = 0; i < NumBulletPerRelease; i++) {
                        GameObject Temp_Bullet_Handeler;
                        Temp_Bullet_Handeler = Instantiate(Bullet, Bullet_Emitter.transform.position,
                            Bullet_Emitter.transform.rotation) as GameObject;
                        var vs =
                          Vector3.Normalize(Vector3.Slerp((Bullet_EmitterLeftEdge.transform.position
                            - Bullet_Emitter.transform.position),
                            (Bullet_EmitterRightEdge.transform.position - Bullet_Emitter.transform.position),
                            Random.value));
                        Temp_Bullet_Handeler.GetComponent<Rigidbody>().velocity = vs * 10f;
                        Destroy(Temp_Bullet_Handeler, 3.0f);
                    }
                    Player.GetComponentInChildren<Rigidbody>().AddForce(Player.transform.TransformVector(
                          Vector3.forward * 40f));
                    Player.GetComponentInChildren<Rigidbody>().angularVelocity = Vector3.zero;
                    break;
                default:
                    break;
            }
        } // Step
    }
}
