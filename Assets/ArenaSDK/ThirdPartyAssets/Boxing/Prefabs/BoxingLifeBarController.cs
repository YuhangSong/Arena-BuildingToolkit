using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxingLifeBarController : MonoBehaviour
{
    private Vector3 barscale;
    // Start is called before the first frame update
    void Start()
    {
        this.barscale = this.transform.localScale;
    }

    // Update is called once per frame
    public void UpdatePercentage(float percentage)
    {
        Vector3 barscale_temp = this.barscale;
        barscale_temp.y = this.barscale.y * percentage;
        transform.localScale = barscale_temp;
    }
}
