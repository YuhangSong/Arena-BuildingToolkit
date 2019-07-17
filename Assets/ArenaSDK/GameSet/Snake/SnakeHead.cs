using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : MonoBehaviour
{
    public List<GameObject> BodyPartList;
    public float MovementSpeed = 1;
    public float TurnSpeed = 1;
    public int StartBodySegments = 3;

    private int BodySegments;
    public GameObject BodyPrefab;
    private Transform startPos;
    public float Distance;

    private float currentRotation;
    public
    void Start()
    {
        startPos = this.transform;
        Reset();
    }
        
    void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            currentRotation -= TurnSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            currentRotation += TurnSpeed * Time.deltaTime;
        }
    }
    public void FixedUpdate()
    {
        Rotate();
        MoveFWD();
    }

    void Rotate()
    {
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, currentRotation, transform.rotation.z));
    }
    void MoveFWD()
    {
        transform.position += transform.forward * MovementSpeed * Time.deltaTime;
    }
    public void Reset()
    {
        for (int i = 0; i < BodyPartList.Count; i++)
        {
            Destroy(BodyPartList[i]);
        }

        BodySegments = StartBodySegments;
        BodyPartList.Clear();


        for (int i = 0; i < BodySegments; i++)
        {
            GameObject bp = Instantiate(BodyPrefab, new Vector3(startPos.position.x, startPos.position.y, startPos.position.z - (i+1)*Distance), Quaternion.identity);
            BodyPartList.Add(bp);
        }
        this.transform.position = startPos.position;
        this.transform.rotation = startPos.rotation;
    }

    public void AddSegment()
    {
        int newPartN = BodyPartList.Count;
        Transform lastSegment = BodyPartList[newPartN-1].transform;
        Vector3 newPos = lastSegment.position - lastSegment.forward.normalized * Distance; 
        GameObject bp = Instantiate(BodyPrefab, newPos, Quaternion.identity);
        BodyPartList.Add(bp);
    }

    //Used for testing only
    //private void OnGUI()
    //{
    //    if (GUI.Button(new Rect(10, 10, 90, 50), "Add segment"))
    //        AddSegment();
    //}
    private void OnCollisionEnter(Collision collision)
    {
        // just some quasi-code placeholder for collisions
        //if (food)           { AddSegment(); }
        //if (other agent)    { Kill();       }
        //if (obstacle)       { Die();        }
    }
}
