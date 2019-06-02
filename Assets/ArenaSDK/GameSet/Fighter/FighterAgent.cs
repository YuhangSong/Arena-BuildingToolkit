using UnityEngine;

namespace Arena
{
    public class FighterAgent : BasicAgent
    {
        // public float MoveForce = 20f;

        public float BulletFarwardForce = 500f;
        public float NumBulletPerLoad   = 0.5f;
        public float FullNumBullet      = 30f;

        public GameObject BulletEmitter;
        public GameObject Bullet;
        public PercentageBar BulletBar;

        private float NumBullet;
        private bool Reloading;

        // PlayerReinitializor = new TransformReinitializor(
        //     Player,
        //     Vector3.zero, new Vector3(3f, 0f, 6f),
        //     Vector3.zero, new Vector3(0f, 0f, 0f),
        //     Vector3.zero, Vector3.zero);

        public override void
        AgentReset()
        {
            base.AgentReset();
            this.NumBullet = Random.Range(0, FullNumBullet);
            BulletBar.UpdatePercentage(GetBulletPercentage());
        }

        override protected void
        DiscreteStep(int Action_)
        {
            base.DiscreteStep(Action_);

            switch (Action_) {
                case Hit:
                    if ((this.NumBullet > 0) && !this.Reloading) {
                        GameObject Temp_Bullet_Handeler;
                        Temp_Bullet_Handeler = Instantiate(Bullet, BulletEmitter.transform.position,
                            BulletEmitter.transform.rotation) as GameObject;
                        Temp_Bullet_Handeler.transform.SetParent(transform, true);
                        Temp_Bullet_Handeler.GetComponent<Rigidbody>().AddForce(
                            BulletEmitter.transform.forward * BulletFarwardForce);
                        Destroy(Temp_Bullet_Handeler, 3.0f);
                        this.NumBullet -= 1.0f;
                        BulletBar.UpdatePercentage(GetBulletPercentage());
                        if (this.NumBullet < 1.0f) {
                            this.Reloading = true;
                        }
                    }
                    break;
                default:
                    break;
            }

            if (this.Reloading) {
                this.NumBullet += NumBulletPerLoad;
                BulletBar.UpdatePercentage(GetBulletPercentage());
                if (this.NumBullet >= FullNumBullet) {
                    this.Reloading = false;
                }
            }
        } // Step

        private float
        GetBulletPercentage()
        {
            return (float) this.NumBullet / (float) FullNumBullet;
        }
    }
}
