using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;

namespace Arena
{
    [RequireComponent(typeof(Camera))]
    public class RenderGraph : ArenaBase
    {
        public GameObject Panel;

        public Color GraphColor = new Color(0.9f, 0.2f, 0.1f, 0.2f);

        public Color PanelColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

        public List<float> vectorObservation = new List<float>();

        private List<float> VectorObs = new List<float>(); // List used to display graph

        private float ValueRange;
        private Vector2 NormedScale = new Vector2(1, 1);
        private Vector2 rePosition  = new Vector2(0, 0);
        private Material LineMat;

        private Canvas PanelCanvas = null;

        public override void
        Initialize()
        {
            base.Initialize();

            // check config
            if (Panel == null) {
                Debug.LogError("Panel must be assigned to a VisVecObsPanel prefab");
            } else {
                PanelCanvas = Panel.GetComponent<RectTransform>().GetComponentInParent<Canvas>();
            }

            // initiate config
            if (LineMat == null) {
                LineMat       = new Material(Shader.Find("Unlit/Color"));
                LineMat.color = GraphColor;
            }
        }

        private void
        Update()
        {
            if (globalManager.IsVisVecObs) {
                Panel.GetComponent<Image>().enabled = true;
                Resize2Panel();
                VectorObs.Clear();
                for (int i = globalManager.StartBit; i < globalManager.EndBit; i++) {
                    VectorObs.Add(vectorObservation[i]);
                }
                ValueRange = Math.Abs(VectorObs.Max() - VectorObs.Min());
            } else {
                Panel.GetComponent<Image>().enabled = false;
            }
        }

        private void
        OnPostRender()
        {
            if (globalManager.IsVisVecObs) {
                RenderLines(VectorObs);
            }
        }

        private void
        RenderLines(List<float> vecobs)
        {
            for (int i = 1; i < vecobs.Count(); i++) {
                LineMat.color = GraphColor;
                GL.PushMatrix(); // Save the current state
                LineMat.SetPass(0);
                GL.LoadOrtho();
                GL.Color(GraphColor);
                GL.Begin(GL.LINES);

                Vector2 xyA = ReShape(NormedScale, rePosition, vecobs[i], i, ValueRange);
                Vector2 xyB = ReShape(NormedScale, rePosition, vecobs[i - 1], i - 1, ValueRange);

                Vector3 A = new Vector3(xyA.x, xyA.y, 0);
                Vector3 B = new Vector3(xyB.x, xyB.y, 0);
                ;

                GL.Vertex(A);
                GL.Vertex(B);

                GL.End();
                GL.PopMatrix(); // Pop changes.
            }
        }

        private Vector2
        ReShape(Vector2 scale, Vector2 posZero, float value, float order, float rang_)
        {
            Vector2 newpos = new Vector2(order / (Math.Abs(
                    globalManager.StartBit - globalManager.EndBit)), value / rang_);

            newpos *= scale * new Vector2(1, globalManager.VerticalZoom);
            newpos += posZero + new Vector2(0, globalManager.VerticalOffset);
            return newpos;
        }

        void
        Resize2Panel()
        {
            float NormedXScale = Panel.GetComponent<RectTransform>().rect.width
              / PanelCanvas.GetComponent<RectTransform>().rect.width;
            float NormedYScale = Panel.GetComponent<RectTransform>().rect.height
              / PanelCanvas.GetComponent<RectTransform>().rect.height;

            NormedScale = new Vector2(NormedXScale, NormedYScale);

            float NormedXPos = (
                Panel.GetComponent<RectTransform>().anchoredPosition.x
                - Panel.GetComponent<RectTransform>().rect.width / 2
              ) / PanelCanvas.GetComponent<RectTransform>().rect.width;
            float NormedYPos = (
                Panel.GetComponent<RectTransform>().anchoredPosition.y
              ) / PanelCanvas.GetComponent<RectTransform>().rect.height;
            rePosition = new Vector2(NormedXPos, NormedYPos);
        }

        // public Vector2 PanelPosition = new Vector2(0, -45);
        // [Tooltip("Relative to screen in %")]
        //
        // public Vector2 PanelSize = new Vector2(80, 40);

        // GameObject
        // GetChildWithName(GameObject obj, string name)
        // {
        //     Transform trans      = obj.transform;
        //     Transform childTrans = trans.Find(name);
        //
        //     if (childTrans != null) {
        //         return childTrans.gameObject;
        //     } else {
        //         return null;
        //     }
        // }

        // [ExecuteInEditMode] // for changing the Panel size in editor. Could be turned off if it's causing performance issues in editor.
        // private void
        // OnValidate()
        // {
        //     // if (Panel) {
        //     //     PanelParameters(Panel);
        //     // }
        //     PanelSize.x = Mathf.Clamp(PanelSize.x, 10, 100);
        //     PanelSize.y = Mathf.Clamp(PanelSize.y, 10, 100);
        //     globalManager.StartBit = Mathf.Clamp(globalManager.StartBit, 0, globalManager.EndBit - 1); // keeps the startvector lower than endvector and vice versa
        //     globalManager.EndBit   = Mathf.Clamp(globalManager.EndBit, globalManager.StartBit + 1, GlobalManager.MaxVecObsSize);
        // }

        // private void
        // PanelParameters(GameObject PanelObj)
        // {
        //     PanelObj.GetComponent<RectTransform>().sizeDelta        = PanelSize * new Vector2(8, 6);
        //     PanelObj.GetComponent<RectTransform>().anchoredPosition = (Vector2.one * 100 - (PanelSize - PanelPosition))
        //       * new Vector2(4, 3); // PanelPosition + new Vector2(50,50);
        //     PanelObj.GetComponent<Image>().color = PanelColor;
        // }

        // public void
        // AssignPanel()
        // {
        //     if (!Panel) {
        //         if (GetChildWithName(gameObject, "VisVecObsPanel")) { // find if Panel exist as child object GetChildWithName, but is not assigned - needs fixing! - returns null always
        //             Panel = GetChildWithName(gameObject, "VisVecObsPanel");
        //             GameObject canvasObj = GetChildWithName(gameObject, "Canvas");
        //             Panel.transform.parent = canvasObj.transform;
        //         } else { // vvvv--- create a new Panel
        //             GameObject newPanel  = new GameObject("VisVecObsPanel");
        //             GameObject canvasObj = transform.Find("Canvas").gameObject;
        //             newPanel.transform.parent = canvasObj.transform;
        //             newPanel.AddComponent<RectTransform>();
        //             newPanel.AddComponent<CanvasRenderer>();
        //             newPanel.AddComponent<Image>();
        //
        //             var sprit = Resources.Load<Sprite>("frame");
        //             newPanel.GetComponent<Image>().sprite = sprit;
        //             PanelParameters(newPanel);
        //             newPanel.GetComponent<RectTransform>().localPosition =
        //               new Vector3(newPanel.GetComponent<RectTransform>().localPosition.x,
        //                 newPanel.GetComponent<RectTransform>().localPosition.y, 0);
        //             newPanel.GetComponent<RectTransform>().localScale    = new Vector3(1, 1, 1);
        //             newPanel.GetComponent<RectTransform>().localRotation = Quaternion.identity;
        //             newPanel.GetComponent<RectTransform>().anchorMin     = new Vector2(0, 0);
        //             newPanel.GetComponent<RectTransform>().anchorMax     = new Vector2(0, 0);
        //             newPanel.GetComponent<RectTransform>().pivot         = new Vector2(0, 0);
        //             newPanel.GetComponent<Image>().type = Image.Type.Sliced;
        //             Panel = newPanel;
        //         }
        //     }
        // }
    }
}
