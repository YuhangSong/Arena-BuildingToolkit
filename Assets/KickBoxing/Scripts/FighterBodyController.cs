using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arena;

public class FighterBodyController : MonoBehaviour
{
    public float hurting;
    public float hurted;
    public GameObject warning_ball;

    private const float scale_speed = 5.0f;

    private bool hurting_warning = false;
    private float hurting_warning_start_time;

    private Vector3 warning_ball_scale;
    private const float scale_magnititude = 9.0f;

    private void
    trig_hurting_warning()
    {
        this.hurting_warning = true;
        this.hurting_warning_start_time = Time.time;
    }

    private void
    Update()
    {
        if (this.hurting_warning) {
            warning_ball.transform.localScale = this.warning_ball_scale
              * (1.0f + scale_magnititude * Mathf.Sin(Time.time * scale_speed));
            if ((Time.time - this.hurting_warning_start_time) > 1.0f) {
                this.hurting_warning = false;
                warning_ball.transform.localScale = this.warning_ball_scale;
            }
        }
    }

    private void
    Start()
    {
        this.warning_ball_scale = warning_ball.transform.localScale;
    }

    private void
    OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Body")) {
            FighterBodyController other_body =
              other.gameObject.GetComponent(typeof(FighterBodyController)) as FighterBodyController;
            // this.GetComponentInParent<KickBoxingAgent>().hurt(other_body.hurting * this.hurted);
            this.trig_hurting_warning();
        }
    }
}
