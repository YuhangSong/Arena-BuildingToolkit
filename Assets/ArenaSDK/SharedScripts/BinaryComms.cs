using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arena
{
    public class BinaryComms : ArenaBase
    {
        public Material MaterialZero;
        public Material MaterialOne;

        private int MaxNumBits = -1;

        private GameObject BitObject;
        private GameObject[] BitGameObjectArray;

        private uint BitValues;

        public void
        DisplaySocialID(int SocialID)
        {
            BitValues |= (uint) SocialID;
            RefreshBits();
        }

        public int
        GetMaxNumBits()
        {
            if (MaxNumBits < 0) {
                MaxNumBits = (int) Mathf.Log(globalManager.GetMaxNumAgents() + 1, 2f);
            }
            return MaxNumBits;
        }

        public override void
        Initialize()
        {
            base.Initialize();

            // Bits	Range
            // 8	    0 to 2^8-1 (255)
            BitGameObjectArray = new GameObject[GetMaxNumBits()];

            SetBitObject();
            SpawnBits();

            BitValues = 0;
            RefreshBits();
        }

        private void
        SetBitObject()
        {
            BitObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            BitObject.transform.localScale = new Vector3(0.3f, 0.08f, 0.3f);
            BitObject.layer = 10;
        }

        private void
        SpawnBits()
        {
            for (int i = 0; i < GetMaxNumBits(); i++) {
                Vector3 basePos = transform.position;
                Vector3 bitPos  = new Vector3(basePos.x, (basePos.y + 0.0f + (0.162f * i)), basePos.z);
                GameObject bit  = Instantiate(BitObject, bitPos, Quaternion.identity, this.transform);
                BitGameObjectArray[i] = bit;
            }
        }

        /// <summary>
        /// Set black/white materials according to the changes in BitValues array
        /// </summary>
        private void
        RefreshBits()
        {
            for (int i = 0; i < GetMaxNumBits(); i++) {
                if (Utils.GetBit(BitValues, i)) {
                    BitGameObjectArray[i].GetComponent<Renderer>().material = MaterialOne;
                } else {
                    BitGameObjectArray[i].GetComponent<Renderer>().material = MaterialZero;
                }
            }
        }
    }
}
