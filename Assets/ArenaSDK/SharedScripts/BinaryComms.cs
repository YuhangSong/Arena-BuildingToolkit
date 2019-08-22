using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class BinaryComms : MonoBehaviour
{
    public bool UseCommunication;
    public int BitCount;
    private GameObject BitObject;
    public bool[] BitValues;

    private GameObject[] BitArray;
    [SerializeField]
    Material M_zero;
    [SerializeField]
    Material M_one;
    

    void Start()
    {
        SetBitObject();
        if (UseCommunication)
        {
            BitValues   = new bool[BitCount];
            BitArray    = new GameObject[BitCount];
            SpawnBits();
            for (int i = 0; i < BitCount; i++) // random initial values
            {                
                if (Random.value >= 0.5)
                {   BitValues[i] = true;    }
                else
                {   BitValues[i] = false;   }
            }
            SetBitValues();
        }
    }

    void SetBitObject()
    {
        BitObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        BitObject.transform.localScale = new Vector3(0.3f, 0.08f, 0.3f);
    }

    void SpawnBits()
    {
        for (int i = 0; i < BitCount; i++)
        {
            Vector3 agentPos = transform.position;
            Vector3 bitPos = new Vector3(agentPos.x, (agentPos.y + 0.5f +(0.162f*i)), agentPos.z);
            GameObject bit = Instantiate(BitObject, bitPos, Quaternion.identity, this.transform);
            BitArray[i] = bit;
        }

    }
    void SetBitValues()         //method to set black/white materials according to the changes in BitValues array
    {
        if (UseCommunication)
        {
            for (int i = 0; i < BitValues.Length; i++)
            {
                if (BitValues[i])
                {                    
                    BitArray[i].GetComponent<Renderer>().material = M_one;  
                }
                else
                { BitArray[i].GetComponent<Renderer>().material = M_zero; }
            }
        }
    }
}
