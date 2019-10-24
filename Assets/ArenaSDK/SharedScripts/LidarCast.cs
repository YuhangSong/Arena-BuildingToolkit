using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class LidarCast : MonoBehaviour
{

    [Range(1, 128)]
    public int h = 16;           // number of collumns
    [Tooltip("Field-of-View in degrees")]
    [Range(1f, 359f)]
    public float H_FOV = 150;     // Horizontal field of view 
    [Tooltip("Range as a distance from Lidars position")]
    [Range(0.1f, 50f)]
    public float Range = 5;      // range of the lidar in scene units
    [Range(0.0f, 2f)]
    [Tooltip("Starting offset used to avoid selfcollision with the agent")]
    public float Offset = 0.5f;
    public Color Raycolor = new Color(0.7f, 0.81f, 0.40f, 1f);

    List<Vector3> RaysList = new List<Vector3>();

    RaycastHit Hit;
    [HideInInspector]
    public float[] LidarValue;          // Array used as vector observations 

    void Update()
    {
        if (!EditorApplication.isPlaying)
        {
            PopulateRays(h, H_FOV);
            DrawRays();
        }
        else
        {
            for (int i = 0; i < RaysList.Count; i++)
            {
                LidarValue[i] = CastRay(RaysList[i]);
            }
        }
    }
    void DrawRays() //used for preview in editor
    {
        foreach (Vector3 ray in RaysList)
        {
            Vector3 origin = transform.position + ray * Offset;
            Debug.DrawRay(origin, ray * (Range - Offset), Raycolor);
        }
    }
    void PopulateRays(int hcount, float fov)
    {

        RaysList.Clear();
        Vector3 currentRay;
        Vector3 fwd = gameObject.transform.forward;
        float stepAngle = fov / hcount;

        for (int i = 0; i < hcount; i++)
        {
            for (int j = 0; j < hcount; j++)
            {
                currentRay = Quaternion.Euler(0, (-fov / 2) + stepAngle / 2 + stepAngle * (i), 0) * fwd;

                currentRay = currentRay.normalized;
                RaysList.Add(currentRay);
            }
        }

    }

    private float CastRay(Vector3 ray) // returns the distance to the closest hit of raycast
    {
        Vector3 origin = transform.position + ray * Range / Offset;
        if (Physics.Raycast(origin, ray, Range - Offset))            //  add a layer mask value if you need to ignore certain type of objects
        {
            float distance = Vector3.Distance(Hit.transform.position, transform.position);
            return distance;
        }
        else return 0f;
    }
}
