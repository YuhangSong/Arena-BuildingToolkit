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
    public class VecObsVisualizor : ArenaBase
    {
        [Tooltip("If visualize vector observation")]
        public bool IsVisVecObs = false;

        // this option has bug
        [Tooltip("If automatically scaling the curve")]
        public bool IsAutoScale = false;

        [Tooltip("Start bit of VisVecObs")]
        [Range(0, GlobalManager.MaxVecObsSize)]
        public int StartBit;

        [Tooltip("End bit of VisVecObs")]
        [Range(0, GlobalManager.MaxVecObsSize)]
        public int EndBit;

        [Tooltip("Vertical scale of VisVecObs")]
        [Range(0.1f, 10f)]
        public float VerticalZoom = 1;

        [Tooltip("Vertical Offset of VisVecObs")]
        [Range(-1, 1)]
        public float VerticalOffset = 0;

        public GameObject Panel;

        public Color GraphColor = new Color(0.9f, 0.2f, 0.1f, 0.2f);

        public Color PanelColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

        public List<float> vectorObservation = new List<float>();

        private List<float> VectorObs = new List<float>(); // List used to display graph

        private float ValueRange;
        private float ValueMid;
        private Vector2 NormedScale    = new Vector2(1, 1);
        private Vector2 NormedPosition = new Vector2(0, 0);
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
            // override config if there is a globalManager
            if (globalManager != null) {
                IsVisVecObs    = globalManager.IsVisVecObs;
                StartBit       = globalManager.StartBit;
                EndBit         = globalManager.EndBit;
                VerticalZoom   = globalManager.VerticalZoom;
                VerticalOffset = globalManager.VerticalOffset;
                IsAutoScale    = globalManager.IsAutoScale;
            }

            if (IsVisVecObs) {
                // get vectorObservation if there is a ArenaAgent as parent
                if (GetComponentInParent<ArenaAgent>() != null) {
                    vectorObservation = GetComponentInParent<ArenaAgent>().GetVectorObservations();
                } else {
                    for (int i = 0; i < GlobalManager.MaxVecObsSize; i++) {
                        vectorObservation.Add((float) (i % 13 - 6) / 6f);
                    }
                }
                Utils.Active(Panel.GetComponent<Image>().gameObject);
                Resize2Panel();
                VectorObs.Clear();
                for (int i = StartBit; i < EndBit; i++) {
                    VectorObs.Add(vectorObservation[i]);
                }
                if (IsAutoScale) {
                    ValueMid   = (VectorObs.Max() + VectorObs.Min()) / 2f;
                    ValueRange = Math.Abs(VectorObs.Max() - VectorObs.Min());
                } else {
                    ValueRange = 2f;
                    ValueMid   = 0f;
                }
            } else {
                Utils.Deactive(Panel.GetComponent<Image>().gameObject);
            }
        } // Update

        private void
        OnPostRender()
        {
            if (IsVisVecObs) {
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

                Vector2 xyA = ReShape(NormedScale, NormedPosition, vecobs[i], i, ValueMid, ValueRange);
                Vector2 xyB = ReShape(NormedScale, NormedPosition, vecobs[i - 1], i - 1, ValueMid, ValueRange);

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
        ReShape(Vector2 scale, Vector2 posZero, float value, float order, float mid_, float rang_)
        {
            Vector2 newpos = new Vector2(order / (Math.Abs(
                    StartBit - EndBit)), (value - mid_) / rang_);

            newpos *= scale * new Vector2(1, VerticalZoom);
            newpos += posZero + new Vector2(0, VerticalOffset);
            return newpos;
        }

        private void
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

            NormedPosition = new Vector2(NormedXPos, NormedYPos);
        }
    }
}
