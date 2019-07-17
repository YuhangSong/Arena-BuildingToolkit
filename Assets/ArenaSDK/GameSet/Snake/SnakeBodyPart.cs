using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBodyPart : MonoBehaviour
{
    public GameObject SnakeHead;
    private float distance;
    private int myPlace;
    void Start()
    {
        if (SnakeHead == null)
        {
            SnakeHead = GameObject.Find("SnakeHead");
        }
         distance = SnakeHead.GetComponent<SnakeHead>().Distance;
         for (int i = 0; i < SnakeHead.GetComponent<SnakeHead>().BodyPartList.Count; i++)
         {
             if (gameObject == SnakeHead.GetComponent<SnakeHead>().BodyPartList[i])
             {
                 myPlace = i;
             }
         }        
    }
    private Vector3 moveVector;

    void FixedUpdate()
    {
        if (myPlace == 0)
        {
            //Vector3 moveVector = new Vector3(0, 0, 0);
            transform.position = Vector3.SmoothDamp(transform.position, SnakeHead.transform.position, ref moveVector, distance/5);
            transform.LookAt(SnakeHead.transform.position);
        }
        else
        {
            //Vector3 moveVector = new Vector3(0, 0, 0);
            GameObject previousPart = SnakeHead.GetComponent<SnakeHead>().BodyPartList[myPlace - 1];
            transform.position = Vector3.SmoothDamp(transform.position, previousPart.transform.position, ref moveVector, distance / 5);
            transform.LookAt(previousPart.transform.position);
        }
    }
}
