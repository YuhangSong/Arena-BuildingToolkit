using UnityEngine;

namespace Arena
{
    public class BoomerAgent : BasicAgent
    {
        public float MoveSpeed = 5.0f;

        public GameObject BulletEmitterForward;

        public override void
        InitializeAgent()
        {
            base.InitializeAgent();
            UIPercentageBars["AM"].Enable();
        }

        public override void
        AgentReset()
        {
            base.AgentReset();

            NumBullet = FullNumBullet;
            UIPercentageBars["AM"].UpdatePercentage(NumBullet / FullNumBullet);
        }

        override protected void
        DiscreteStep(int Action_)
        {
            base.DiscreteStep(Action_);

            Vector3 PalyerVelocity = Player.transform.InverseTransformVector(Player.GetComponent<Rigidbody>().velocity);
            PalyerVelocity = Vector3.zero;

            switch (Action_) {
                case Right:
                    Player.transform.eulerAngles = new Vector3(
                        Player.transform.eulerAngles.x,
                        transform.eulerAngles.y + 90.0f,
                        Player.transform.eulerAngles.z);
                    PalyerVelocity.z = MoveSpeed;
                    break;
                case Left:
                    Player.transform.eulerAngles = new Vector3(
                        Player.transform.eulerAngles.x,
                        transform.eulerAngles.y - 90.0f,
                        Player.transform.eulerAngles.z);
                    PalyerVelocity.z = MoveSpeed;
                    break;
                case Forward:
                    Player.transform.eulerAngles = new Vector3(
                        Player.transform.eulerAngles.x,
                        transform.eulerAngles.y + 0.0f,
                        Player.transform.eulerAngles.z);
                    PalyerVelocity.z = MoveSpeed;
                    break;
                case Backward:
                    Player.transform.eulerAngles = new Vector3(
                        Player.transform.eulerAngles.x,
                        transform.eulerAngles.y + 180.0f,
                        Player.transform.eulerAngles.z);
                    PalyerVelocity.z = MoveSpeed;
                    break;
                case Attack:
                    if ((NumBullet > 0) && !Reloading) {
                        GameObject Temp_Bullet_Handeler;
                        Temp_Bullet_Handeler = Instantiate(Bullet, BulletEmitterForward.transform.position,
                            BulletEmitterForward.transform.rotation) as GameObject;
                        Temp_Bullet_Handeler.SetActive(true);
                        NumBullet -= 1.0f;
                        UIPercentageBars["AM"].UpdatePercentage(NumBullet / FullNumBullet);
                        if (NumBullet < 1.0f) {
                            Reloading = true;
                        }
                    }
                    break;
                default:
                    break;
            }

            Player.GetComponent<Rigidbody>().velocity = Player.transform.TransformVector(PalyerVelocity);

            if (Reloading) {
                NumBullet += NumBulletPerLoad;
                UIPercentageBars["AM"].UpdatePercentage(NumBullet / FullNumBullet);
                if (NumBullet >= FullNumBullet) {
                    Reloading = false;
                }
            }
        } // DiscreteStep
    }
}
