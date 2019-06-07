Shader "Sun_Temple/Mountains" {
    Properties {

        _TerrainNormal ("Terrain Normal map (overall)", 2D) = "bump" {}

        _MainTex ("Layer_A Albedo (RGB)", 2D) = "white" {}       
        _BumpMap ("LAYER_A Normal", 2D) = "bump" {}
        _baseTiling("LAYER_A Tiling", float ) = 1


        _layer1Tex ("LAYER_B Albedo (RGB) Smoothness (A)", 2D) = "white" {}       
        _layer1Norm("LAYER_B Normal", 2D) = "bump" {}
        _layer1Tiling("LAYER_B Tiling", float) = 1

        _BlendMask("BLEND_Mask", 2D) = "white" {}
    }

    CGINCLUDE
        #define _GLOSSYENV 1
        #define UNITY_SETUP_BRDF_INPUT MetallicSetup
    ENDCG
    
    SubShader {
        Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
        LOD 500
 
        CGPROGRAM
        #include "UnityPBSLighting.cginc"
        #pragma surface surf Standard fullforwardshadows noforwardadd nolightmap
        #pragma target 3.0
       

        sampler2D _MainTex, _BumpMap, _layer1Tex, _layer1Norm, _BlendMask, _TerrainNormal;
        half _BlendMaskInvert, _layer1Tiling, _baseTiling;



        struct Input {
            half2 uv_MainTex; 
            half2 uv_BumpMap;         
        };


        void surf (Input IN, inout SurfaceOutputStandard o) {       
            
            // Base layer textures
            half3 layerA_albedo = tex2D (_MainTex, IN.uv_MainTex * _baseTiling );
            half3 layerA_normal = UnpackNormal(tex2D (_BumpMap, IN.uv_BumpMap * _baseTiling)); 



            half3 terrainNormal = UnpackNormal(tex2D (_TerrainNormal, IN.uv_BumpMap));           
 

            // Layer1 Textures
            half3 layerB_albedo = tex2D (_layer1Tex, IN.uv_MainTex * _layer1Tiling);
            half3 layerB_normal = UnpackNormal(tex2D(_layer1Norm, IN.uv_MainTex * _layer1Tiling));
            
            

            // Blend Mask
            half blendMask = tex2D(_BlendMask, IN.uv_MainTex).r;      


            // Blended textures
            half3 blendedAlbedo = lerp(layerB_albedo, layerA_albedo, blendMask);     
            half3 blendedNormal = lerp(layerB_normal, layerA_normal, blendMask);
            half3 finalNormal = terrainNormal + half3(blendedNormal.r, blendedNormal.g, 0);
       



            o.Albedo = blendedAlbedo; 
            o.Normal = finalNormal.rgb;    
            o.Metallic = 0;
            o.Smoothness = 0;       

        }
        ENDCG
    } 
    FallBack "Bumped Specular"
}