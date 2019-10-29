using UnityEngine;

namespace Arena
{
    public class Boom : ArenaBase
    {
        // public reference
        public GameObject Fire;

        // public config
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

        public override void
        Initialize()
        {
            base.Initialize();

            Scale     = transform.localScale;
            Material  = new MaterialPropertyBlock();
            TimeStart = Time.time;

            Invoke("Explode", ExplosionTime);
        }

        void
        EmmitBullet(Vector3 direction)
        {
            // emmit Bullet towards direction

            // create Bullet object
            GameObject Temp_Bullet_Handeler;

            Temp_Bullet_Handeler = Instantiate(Fire,
                transform.position + direction * (transform.lossyScale.x + Fire.transform.lossyScale.x) / 2f,
                transform.rotation) as GameObject;
            Temp_Bullet_Handeler.SetActive(true);

            // Bullet does not collise with Boom or Bullet
            Utils.IgnoreCollision(Temp_Bullet_Handeler, "Boom");
            Utils.IgnoreCollision(Temp_Bullet_Handeler, "Bullet");

            // give Bullet initial speed
            Temp_Bullet_Handeler.GetComponent<Rigidbody>().velocity =
              (transform.TransformDirection(Vector3.forward
              * 100.0f));
        }

        private void
        Explode()
        {
            EmmitBullet(Vector3.forward);
            EmmitBullet(Vector3.back);
            EmmitBullet(Vector3.left);
            EmmitBullet(Vector3.right);

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
