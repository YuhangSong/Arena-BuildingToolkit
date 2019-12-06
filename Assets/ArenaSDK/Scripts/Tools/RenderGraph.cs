using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;
[RequireComponent(typeof(Camera))]
public class RenderGraph : MonoBehaviour
{    
    public bool HideFrame = false;
    private Material mat;
    //private float[] points;
    public float[] Observations = new float[120]; // *****! this is a placeholder for the Arena observations. *****!
    [Range(0, 120)]
    public int startVector;
    [Range(0, 120)]
    public int endVector = 120;
    [Range(5, 500)]
    public float Vertical_zoom = 100;
    [Range(-1, 1)]
    public float Vertical_Offset = 0;

    [ExecuteInEditMode] //for changing the panel size in editor. Could be turned off if it's causing performance issues in editor. 
    private void OnValidate() 
    {
        if (panel)
        {
            PanelParameters(panel);
        }
        PanelSize.x = Mathf.Clamp(PanelSize.x, 10, 100);
        PanelSize.y = Mathf.Clamp(PanelSize.y, 10, 100);
        startVector = Mathf.Clamp(startVector, 0, endVector-1); //keeps the startvector lower than endvector and vice versa
        endVector = Mathf.Clamp(endVector, startVector+1, 120);
    }

    private void PanelParameters(GameObject panelObj)
    {
        panelObj.GetComponent<RectTransform>().sizeDelta = PanelSize * new Vector2(8, 6);
        panelObj.GetComponent<RectTransform>().anchoredPosition = (Vector2.one * 100 - (PanelSize - PanelPosition)) * new Vector2(4, 3);//PanelPosition + new Vector2(50,50);
        panelObj.GetComponent<Image>().color = PanelColor;
    }

    List<float> VectorObs = new List<float>(); //List used to display graph

    float[] sorted;

    public GameObject panel;
    [Tooltip("Relative to screen in %")]
    
    public Vector2 PanelPosition = new Vector2(0,-45);
    [Tooltip("Relative to screen in %")]
    
    public Vector2 PanelSize = new Vector2(80, 40);

    public Color GraphColor = new Color(0.9f,0.2f,0.1f,0.2f);
    public Color PanelColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    float range;
    private Vector2 scaleFactor = new Vector2(1,1);
    private Vector2 rePosition= new Vector2(0, 0.5f);

    private void Start()
    {
        if (mat == null) {
            mat = new Material(Shader.Find("Unlit/Color"));
            mat.color = GraphColor;
        }
        if (panel == null)
        {
            AssignPanel();
        }
        //resize2Panel();
    }
    public void AssignPanel() {
        if (!panel)
        {                  
            
            if (GetChildWithName(gameObject, "Panel_VectorObs")) //find if panel exist as child object GetChildWithName, but is not assigned - needs fixing! - returns null always
            {                
                panel = GetChildWithName(gameObject, "Panel_VectorObs");
                GameObject canvasObj = GetChildWithName(gameObject, "Canvas");
                panel.transform.parent = canvasObj.transform;
            }
            else {                                              // vvvv--- create a new panel              
                GameObject newpanel = new GameObject("Panel_VectorObs");
                GameObject canvasObj = transform.Find("Canvas").gameObject;
                newpanel.transform.parent = canvasObj.transform;
                newpanel.AddComponent<RectTransform>();
                newpanel.AddComponent<CanvasRenderer>();
                newpanel.AddComponent<Image>();
                
                var sprit = Resources.Load<Sprite>("frame");
                newpanel.GetComponent<Image>().sprite = sprit; 
                PanelParameters(newpanel);
                newpanel.GetComponent<RectTransform>().localPosition = 
                new Vector3(newpanel.GetComponent<RectTransform>().localPosition.x, newpanel.GetComponent<RectTransform>().localPosition.y, 0);
                newpanel.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                newpanel.GetComponent<RectTransform>().localRotation = Quaternion.identity;
                newpanel.GetComponent<RectTransform>().anchorMin = new Vector2(0,0);
                newpanel.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
                newpanel.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
                newpanel.GetComponent<Image>().type = Image.Type.Sliced;
                panel = newpanel;
            }
        }        
    }

    private void Update()
    {
        resize2Panel();
        if (HideFrame && panel.GetComponent<Image>().enabled == true)
        { panel.GetComponent<Image>().enabled = false; }
        else if (!HideFrame && panel.GetComponent<Image>().enabled == false) { panel.GetComponent<Image>().enabled = true; }
        
        VectorObs.Clear();
        AddRandomValues(Observations);
        for (int i = startVector; i < endVector; i++)
        {
            VectorObs.Add(Observations[i]);
        }
        range = 1.2f*Math.Abs(VectorObs.Max()- VectorObs.Min());        
    }
    void OnPostRender()
    {
        RenderLines(VectorObs);
    }

    void RenderLines(List<float> vecobs)    {       

        for (int i = 1; i < vecobs.Count(); i++)
        {
            mat.color = GraphColor;
            GL.PushMatrix(); // Save the current state            
            mat.SetPass(0);
            GL.LoadOrtho();
            GL.Color(GraphColor);
            GL.Begin(GL.LINES);

            Vector2 xyA = ReShape(scaleFactor, rePosition, vecobs[i], i, range);
            Vector2 xyB = ReShape(scaleFactor, rePosition, vecobs[i-1], i-1, range);

            Vector3 A = new Vector3(xyA.x, xyA.y, 0);
            Vector3 B = new Vector3(xyB.x, xyB.y, 0); ;
                        
            GL.Vertex(A);
            GL.Vertex(B);

            GL.End();
            GL.PopMatrix(); // Pop changes.
        }
    }
    private Vector2 ReShape(Vector2 scale, Vector2 posZero, float value, float order, float rang_) {
        Vector2 newpos = new Vector2(order/(Math.Abs(startVector - endVector)), value / rang_);
        newpos *= scale * new Vector2(1, Vertical_zoom/100f);
        newpos += posZero + new Vector2(0,Vertical_Offset);
        return newpos;
    }
    private float[] AddRandomValues(float[] obs) {  // ************! this is just for prototyping, needs to be removed later
        for (int i = 0; i < obs.Length; i++)
        {
            obs[i] += UnityEngine.Random.Range(-0.1f, 0.1f);
        }
        return obs;
    }
    void resize2Panel() {
        float xscale = (panel.GetComponent<RectTransform>().rect.width / 800)*0.95f;            //<---margin for panel
        float yscale = panel.GetComponent<RectTransform>().rect.height / 600;
        scaleFactor = new Vector2(xscale, yscale);

        float posX = (panel.GetComponent<RectTransform>().anchoredPosition.x / 800f) + 0.025f;  //<---margin for panel
        float posY = (panel.GetComponent<RectTransform>().anchoredPosition.y + (panel.GetComponent<RectTransform>().rect.height / 2)) / 600f;
        rePosition = new Vector2(posX, posY);
    }
    GameObject GetChildWithName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
    }

}
