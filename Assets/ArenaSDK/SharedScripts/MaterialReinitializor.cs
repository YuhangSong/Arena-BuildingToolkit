using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arena
{
    public class MaterialReinitializor : MonoBehaviour
    {
        public List<Material> Materials = new List<Material>();

        private int CurrentMaterialIndex = -1;
        public int
        GetCurrentMaterialIndex()
        {
            return CurrentMaterialIndex;
        }

        public void
        Reinitialize()
        {
            if (Materials.Count > 0) {
                CurrentMaterialIndex = Random.Range(0, Materials.Count);
                Utils.ApplyMaterial(Materials[CurrentMaterialIndex], this.gameObject);
            } else {
                Debug.LogWarning("Only " + Materials.Count + " Materials are assigned.");
            }
        }
    }
}
