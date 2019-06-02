using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollaballController : MonoBehaviour
{
    public int num_target;
    private int count;
    // Start is called before the first frame update
    void Start()
    {
        count = 0;
    }

    public int get_count()
    {
        return count;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag ( "Pick Up"))
        {
            other.gameObject.SetActive (false);
            count = count + 1;
        }

        if (count>(num_target/2))
        {
            this.gameObject.GetComponentInParent<RollaballAgent>().trig_win();
            count = 0;
        }

        if (count == num_target / 2)
        {
            if (this.gameObject.GetComponentInParent<RollaballAgent>().Competitor.GetComponentInChildren<RollaballController>().get_count() == num_target / 2)
            {
                this.gameObject.GetComponentInParent<RollaballAgent>().trig_tie();
                count = 0;
            }
        }
    }
}
