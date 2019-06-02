using UnityEngine;

public class BulletController : MonoBehaviour
{
    void
    OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Bullet")) {
            Destroy(this.gameObject);
        }
    }
}
