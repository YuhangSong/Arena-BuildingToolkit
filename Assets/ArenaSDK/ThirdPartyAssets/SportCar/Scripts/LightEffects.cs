using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEffects : MonoBehaviour {

    public bool on = false;
    public bool on2 = false;
    public bool on3 = false;

    public bool BrakeL = false;
    public bool ReverseL = false;
    public bool FrontL = false;
    public bool TailL = false;
    public bool IndicatorL = false;
    public bool IndicatorR = false;
    public bool INDICATORS = false;

    public GameObject FL;
    public GameObject BL;
    public GameObject TL;
    public GameObject RL;
    public GameObject IL;
    public GameObject IR;

    public Material FLM;
    public Material TLM;
    public Material RLM;
    public Material ILM;
    public Material IRM;

    private float timer = 0.5f;
    private float timer2 = 0.5f;
    private float btimer = 0.5f;
    private float btimer2 = 0.5f;

    private void Start()
    {
        FL = GameObject.Find("FrontLights");
        BL = GameObject.Find("BrakeLights");
        TL = GameObject.Find("TailLights");
        RL = GameObject.Find("ReverseLights");
        IL = GameObject.Find("LeftIndicators");
        IR = GameObject.Find("RightIndicators");

        FL.SetActive(false);
        BL.SetActive(false);
        TL.SetActive(false);
        RL.SetActive(false);
        IL.SetActive(false);
        IR.SetActive(false);

        FLM.DisableKeyword("_EMISSION");
        TLM.DisableKeyword("_EMISSION");
        RLM.DisableKeyword("_EMISSION");
        ILM.DisableKeyword("_EMISSION");
        IRM.DisableKeyword("_EMISSION");
    }

    private void Update()
    {
        if (IndicatorL)
        {
            if (timer >= 0f)
            {
                timer -= Time.deltaTime;
                IL.SetActive(true);
                ILM.EnableKeyword("_EMISSION");
                timer2 = 0.5f;
            }
            if(timer <= 0f)
            {
                IL.SetActive(false);
                ILM.DisableKeyword("_EMISSION");
                timer2 -= Time.deltaTime;
                if (timer2 <= 0f) timer = 0.5f;
            }
        }
        else
        {
            IL.SetActive(false);
            ILM.DisableKeyword("_EMISSION");
        }

        if (IndicatorR)
        {
            if (btimer >= 0f)
            {
                btimer -= Time.deltaTime;
                IR.SetActive(true);
                IRM.EnableKeyword("_EMISSION");
                btimer2 = 0.5f;
            }
            if (btimer <= 0f)
            {
                IR.SetActive(false);
                IRM.DisableKeyword("_EMISSION");
                btimer2 -= Time.deltaTime;
                if (btimer2 <= 0f) btimer = 0.5f;
            }
        }
        else
        {
            IR.SetActive(false);
            IRM.DisableKeyword("_EMISSION");
        }
            
    }

    private void OnGUI()
    {
        ////Rects
        //Rect Rect1 = new Rect(108.75f, 50, 150, 50);
        //Rect Rect2 = new Rect(367.5f, 50, 150, 50);
        //Rect Rect3 = new Rect(626.25f, 50, 150, 50);
        //Rect Rect4 = new Rect(885f, 50, 150, 50);
        //Rect Rect5 = new Rect(1143.75f, 50, 150, 50);
        //Rect Rect6 = new Rect(1402.5f, 50, 150, 50);
        //Rect Rect7 = new Rect(1661.25f, 50, 150, 50);

        ////Screen scaling
        //Vector3 scale;
        //float originalWidht = 1920;
        //float originalHeight = 1080;

        //scale.x = Screen.width / originalWidht;
        //scale.y = Screen.height / originalHeight;
        //scale.z = 1;
        //Matrix4x4 svMat = GUI.matrix;
        //GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, scale);

        //GUI.backgroundColor = Color.clear;

        ////Buttons
        //if(GUI.Button(Rect1, "Front Lights"))
        //{
        //    if (FrontL == false)
        //    {
        //        FrontL = true;
        //        FL.SetActive(true);
        //        FLM.EnableKeyword("_EMISSION");
        //    }
        //    else {
        //        FrontL = false;
        //        FL.SetActive(false);
        //    }
        //}
        //if (GUI.Button(Rect2, "Tail Lights"))
        //{
        //    if (TailL == false)
        //    {
        //        TailL = true;
        //        TL.SetActive(true);
        //        TLM.EnableKeyword("_EMISSION");
        //    }
        //    else {
        //        TailL = false;
        //        TL.SetActive(false);
        //        TLM.DisableKeyword("_EMISSION");
        //    }
        //}
        //if (TailL) {
        //    if (GUI.Button(Rect3, "Brake Lights"))
        //    {
        //        if (BrakeL == false)
        //        {
        //            BrakeL = true;
        //            BL.SetActive(true);
        //        }
        //        else
        //        {
        //            BrakeL = false;
        //            BL.SetActive(false);
        //        }
        //    }
        //}
        //if (GUI.Button(Rect4, "Left Indicators"))
        //{
        //    timer = 0.5f;
        //    IndicatorL = false;
        //    IndicatorR = false;
        //    if (on2 == false)
        //    {
        //        on2 = true;
        //        on = false;
        //        on3 = false;
        //        if (IndicatorL == false)
        //        {
        //            IndicatorL = true;
        //            IndicatorR = false;
        //            /*IL.active = true;
        //            ILM.EnableKeyword("_EMISSION");*/
        //        }
        //    }
        //    else
        //    {
        //        on2 = false;
        //        IndicatorL = false;
        //        /*IL.active = false;
        //        ILM.DisableKeyword("_EMISSION");*/
        //    }
        //}
        //if (GUI.Button(Rect5, "Right Indicators"))
        //{
        //    btimer = 0.5f;
        //    IndicatorL = false;
        //    IndicatorR = false;
        //    if (on == false)
        //    {
        //        on = true;
        //        on2 = false;
        //        on3 = false;
        //        if (IndicatorR == false)
        //        {
        //            IndicatorR = true;
        //            IndicatorL = false;
        //            //IR.active = true;
        //            //IRM.EnableKeyword("_EMISSION");
        //        }
        //    }
        //    else
        //    {
        //        on = false;
        //        IndicatorR = false;
        //        //IR.active = false;
        //        //IRM.DisableKeyword("_EMISSION");
        //    }
        //}
        //if (GUI.Button(Rect6, "Reverse Lights"))
        //{
        //    if (ReverseL == false)
        //    {
        //        ReverseL = true;
        //        RL.SetActive(true);
        //        RLM.EnableKeyword("_EMISSION");
        //    }
        //    else
        //    {
        //        ReverseL = false;
        //        RL.SetActive(false);
        //        RLM.DisableKeyword("EMISSION");
        //    }
        //}
        //if (GUI.Button(Rect7, "INDICATORS"))
        //{
        //    btimer = 0.5f;
        //    timer = 0.5f;
        //    IndicatorL = false;
        //    IndicatorR = false;
        //    if (on3 == false)
        //    {
        //        on3 = true;
        //        on = false;
        //        on2 = false;
        //        if (IndicatorL == false && IndicatorR == false)
        //        {
        //            IndicatorL = true;
        //            IndicatorR = true;
        //        }
        //    }
        //    else
        //    {
        //        on3 = false;
        //        IndicatorL = false;
        //        IndicatorR = false;
        //    }
        //}
    }

}
