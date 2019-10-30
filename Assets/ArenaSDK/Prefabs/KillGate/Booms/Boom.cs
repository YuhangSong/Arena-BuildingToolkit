using UnityEngine;

namespace Arena
{
    public class Boom : ArenaBase
    {
        // public reference
        public GameObject Fire;

        // public config
        public int NumFireWhenExplode = 8;
        public float ScaleSpeed       = 10f;
        public float ScaleMagnititude = 0.08f;
        public float ExplosionTime    = 1f;

        // private status
        private Vector3 Scale;
        private MaterialPropertyBlock Material;
        private float TimeStart;

        private void
        Start()
        {
            Initialize();
        }

        private float StepSize;

        public override void
        Initialize()
        {
            base.Initialize();

            Scale     = transform.localScale;
            Material  = new MaterialPropertyBlock();
            TimeStart = Time.time;
            StepSize  = 360f / NumFireWhenExplode;

            Invoke("Explode", ExplosionTime);
        }

        void
        EmmitBullet(Quaternion direction)
        {
            // create Bullet object
            GameObject Temp_Bullet_Handeler;

            Temp_Bullet_Handeler = Instantiate(Fire,
                transform.position,
                direction) as GameObject;
            Temp_Bullet_Handeler.SetActive(true);

            // Bullet does not collise with Boom or Bullet
            Utils.IgnoreCollision(Temp_Bullet_Handeler, "Boom");
            Utils.IgnoreCollision(Temp_Bullet_Handeler, "Bullet");

            // give the fire initial speed
            Vector3 velocity = Temp_Bullet_Handeler.transform.TransformDirection(Vector3.forward * 10.0f);
            Temp_Bullet_Handeler.GetComponent<Rigidbody>().velocity = velocity;
        }

        private void
        Explode()
        {
            for (int i = 0; i < NumFireWhenExplode; i++) {
                EmmitBullet(
                    Quaternion.Euler(
                        0,
                        StepSize * i,
                        0
                    )
                );
            }

            // destroy self
            // Destroy(gameObject, 10f);
        }

        void
        Update()
        {
            if ((Time.time - TimeStart) < ExplosionTime) {
                // flashing
                transform.localScale = Scale
                  * (1.0f + ScaleMagnititude * Mathf.Sin(Time.time * ScaleSpeed));
                Material.SetColor("_Color",
                  (Mathf.Sin(Time.time * ScaleSpeed) + 1.0f) * 0.5f * Color.red
                  + (Mathf.Cos(Time.time * ScaleSpeed) + 1.0f) * 0.5f * Color.black);
                GetComponent<Renderer>().SetPropertyBlock(Material);
            }
        }
    }
}
