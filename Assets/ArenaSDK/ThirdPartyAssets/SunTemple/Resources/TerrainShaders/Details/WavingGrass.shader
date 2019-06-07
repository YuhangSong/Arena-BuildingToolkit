// Modified Waving Grass terrain shader. This one uses vertex color R instead of A for wind.

Shader "Hidden/TerrainEngine/Details/WavingDoublePass" {
Properties {
    _WavingTint ("Fade Color", Color) = (.7,.6,.5, 0)
    _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
    _WaveAndDistance ("Wave and distance", Vector) = (12, 3.6, 1, 1)
    _Cutoff ("Cutoff", float) = 0.5
}

SubShader {
    Tags {
        "Queue" = "Geometry+200"
        "IgnoreProjector"="True"
        "RenderType"="Grass"
        "DisableBatching"="False"
    }
    Cull Off
    LOD 200

    CGINCLUDE
        #define _GLOSSYENV 1
        #define UNITY_SETUP_BRDF_INPUT MetallicSetup
    ENDCG

    CGPROGRAM
    #pragma surface surf Standard vertex:WavingGrassVert_modified fullforwardshadows addshadow interpolateview
    #include "TerrainEngine.cginc"
    #include "UnityBuiltin3xTreeLibrary.cginc"


    sampler2D _MainTex;
    fixed _Cutoff;

    struct Input {
        float2 uv_MainTex;
        fixed4 color : COLOR;
    };


    void WavingGrassVert_modified (inout appdata_full v)
    {
        // MeshGrass v.color.a: 1 on top vertices, 0 on bottom vertices
        // _WaveAndDistance.z == 0 for MeshLit
        float waveAmount = v.color.r * _WaveAndDistance.z;

        v.color = TerrainWaveGrass (v.vertex, waveAmount, v.color);
    }



    void surf (Input IN, inout SurfaceOutputStandard o) {
        fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
        half3 col = lerp(saturate(IN.color * 10), 1, 0);
        o.Albedo = c.rgb;
        o.Alpha = c.a;
        o.Smoothness = 0;
        o.Metallic = 0;
        clip (o.Alpha - _Cutoff);
        o.Alpha *= IN.color.a;
    }
ENDCG



}


    Fallback Off
}
