using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerBoomController : MonoBehaviour
{
    // public reference
    public GameObject[] fire_emitter;
    public GameObject fire;

    // public config
    public float scale_speed;
    public float scale_magnititude;
    public float explosion_time;

    // private status
    private Vector3 scale;
    private MaterialPropertyBlock material;
    private float time_start;

    private void Start()
    {
        // fires does not collise with boom
        GameObject[] fires;
        fires = GameObject.FindGameObjectsWithTag("Fire");
        foreach (GameObject fire in fires)
        {
            Physics.IgnoreCollision(fire.GetComponent<Collider>(), this.GetComponent<Collider>());
        }

        this.scale = this.transform.localScale;
        material = new MaterialPropertyBlock();
        time_start = Time.time;
    }

    void OnCollisionEnter(Collision other)
    {
        //// can not place a boom in collision with walls
        //if (other.gameObject.CompareTag("Wall"))
        //{
        //    Destroy(this.gameObject);
        //}
    }

    void emmit_fire(Vector3 direction)
    {
        // emmits fire ar a direction
        GameObject Temp_Bullet_Handeler;
        Temp_Bullet_Handeler = Instantiate(fire, fire_emitter[0].transform.position+ direction, fire_emitter[0].transform.rotation) as GameObject;
        // fires does not collise with fire
        foreach (GameObject fire in GameObject.FindGameObjectsWithTag("Fire"))
        {
            Physics.IgnoreCollision(Temp_Bullet_Handeler.GetComponent<Collider>(), fire.GetComponent<Collider>());
        }
        Temp_Bullet_Handeler.GetComponent<Rigidbody>().velocity = transform.TransformDirection(direction*10.0f);
    }

    void Update()
    {
        if ((Time.time - time_start) > explosion_time)
        {
            // emitts fire
            emmit_fire(Vector3.forward);
            emmit_fire(Vector3.back);
            emmit_fire(Vector3.left);
            emmit_fire(Vector3.right);
            //destroy self
            Destroy(this.gameObject);
        }
        else
        {
            // flashing
            this.transform.localScale = this.scale * (1.0f + scale_magnititude * Mathf.Sin(Time.time * scale_speed));
            material.SetColor("_Color", (Mathf.Sin(Time.time * scale_speed) + 1.0f) * 0.5f * Color.red + (Mathf.Cos(Time.time * scale_speed) + 1.0f) * 0.5f * Color.black);
            GetComponent<Renderer>().SetPropertyBlock(material);
        }
    }
}
