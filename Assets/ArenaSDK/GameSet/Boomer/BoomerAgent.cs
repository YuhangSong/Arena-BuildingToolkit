using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;
using Arena;

namespace Arena
{
    public class BoomerAgent : BasicAgent
    {
        public float MoveSpeed = 5.0f;

        public GameObject BulletEmitterForward;
        public new GameObject Bullet;
        public new PercentageBar BulletBar;

        public new float NumBulletPerLoad = 0.04f;
        public new float FullNumBullet    = 1.0f;

        private float NumBullet = 1f;
        private bool Reloading  = false;

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
                    if ((this.NumBullet > 0) && !this.Reloading) {
                        GameObject Bullet_Emitter = BulletEmitterForward;
                        Bullet_Emitter = BulletEmitterForward;
                        GameObject Temp_Bullet_Handeler;
                        Temp_Bullet_Handeler = Instantiate(Bullet, Bullet_Emitter.transform.position,
                            Bullet_Emitter.transform.rotation) as GameObject;
                        this.NumBullet -= 1.0f;
                        BulletBar.UpdatePercentage(NumBullet / FullNumBullet);
                        if (this.NumBullet < 1.0f) {
                            this.Reloading = true;
                        }
                    }
                    break;
                default:
                    break;
            }

            Player.GetComponent<Rigidbody>().velocity = Player.transform.TransformVector(PalyerVelocity);

            if (this.Reloading) {
                this.NumBullet += NumBulletPerLoad;
                BulletBar.UpdatePercentage(NumBullet / FullNumBullet);
                if (this.NumBullet >= FullNumBullet) {
                    this.Reloading = false;
                }
            }
        } // DiscreteStep
    }
}
