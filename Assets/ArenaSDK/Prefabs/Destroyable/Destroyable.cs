using UnityEngine;

namespace Arena
{
    public class Destroyable : MonoBehaviour
    {
        public string DestoryerByTag = "Bullet";

        public int LifeTotal = 1;

        public bool EnableExploreEffect;
        public GameObject ExplosionEffect;

        private int NumLife;

        void
        Start()
        {
            this.NumLife = this.LifeTotal;
        }

        void
        OnEnable()
        {
            this.NumLife = this.LifeTotal;
        }

        void
        OnCollisionEnter(Collision other)
        {
            if (this.LifeTotal > 0) {
                if (other.gameObject.CompareTag(this.DestoryerByTag)) {
                    Hitted();
                }
            }
        }

        public void
        Hitted()
        {
            if (EnableExploreEffect) {
                GameObject ExplosionEffectPuff =
                  Instantiate(ExplosionEffect, transform.position,
                    transform.rotation) as GameObject;
                Destroy(ExplosionEffectPuff, 2);
            }

            this.NumLife -= 1;
            if (this.NumLife == 0) {
                this.gameObject.SetActive(false);
            }
        }
    }
}
