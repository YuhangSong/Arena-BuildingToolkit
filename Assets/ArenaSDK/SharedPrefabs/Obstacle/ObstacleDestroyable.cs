using UnityEngine;

namespace Arena
{
    public class ObstacleDestroyable : MonoBehaviour
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
            if (other.gameObject.CompareTag(this.DestoryerByTag)) {
                if (EnableExploreEffect) {
                    GameObject ExplosionEffectPuff =
                      Instantiate(ExplosionEffect, other.transform.position, other.transform.rotation) as GameObject;
                    Destroy(ExplosionEffectPuff, 2);
                }

                if (this.LifeTotal > 0) {
                    this.NumLife -= 1;
                    if (this.NumLife == 0) {
                        this.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
