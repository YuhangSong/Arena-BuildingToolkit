Shader "Flooded_Grounds/PBR_TopBlend" {
    Properties {
        _MainTex ("Base Albedo (RGB)", 2D) = "white" {}
        _Spc("Base Metalness(R) Smoothness(A)", 2D) = "black" {}
        _BumpMap ("Base Normal", 2D) = "bump" {} 
        _AO("Base AO", 2D)= "white" {}
        _layer1Tex ("Layer1 Albedo (RGB) Smoothness (A)", 2D) = "white" {}
        _layer1Metal ("Layer1 Metalness", Range(0,1)) = 0
        _layer1Norm("Layer 1 Normal", 2D) = "bump" {}
        _layer1Breakup ("Layer1 Breakup (R)", 2D) = "white" {}
        _layer1BreakupAmnt ("Layer1 Breakup Amount", Range(0,1)) = 0.5
        _layer1Tiling("Layer1 Tiling", float) = 10
        _Power ("Layer1 Blend Amount", float ) = 1
        _Shift("Layer1 Blend Height", float) = 1           
        _DetailBump ("Detail Normal", 2D) = "bump" {}  
        _DetailInt ("DetailNormal Intensity", Range(0,1)) = 0.4
        _DetailTiling("DetailNormal Tiling", float) = 2  
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
        #pragma surface surf Standard 
        #pragma target 3.0
       

        sampler2D _MainTex, _Spc, _BumpMap, _AO, _DetailBump, _layer1Tex, _layer1Norm, _layer1Breakup;
        half3 _layer1Color, _SelfIlum;
        half _Power, _DetailInt, _DetailTiling, _layer1Tiling, _layer1Metal, _layer1BreakupAmnt, _Shift, _VMask;



        struct Input {
            half2 uv_MainTex; 
            half2 uv_BumpMap;          
            half3 worldNormal;  INTERNAL_DATA         
        
        };


        void surf (Input IN, inout SurfaceOutputStandard o) {       


            half3 layer1direction = half3(0,1,0);


            //Texture Inputs
            half3 main = tex2D (_MainTex, IN.uv_MainTex);            
            half3 norm = UnpackNormal(tex2D (_BumpMap, IN.uv_BumpMap));
            half4 spec = tex2D(_Spc, IN.uv_MainTex);
            half3 ao = tex2D(_AO, IN.uv_BumpMap);
            half4 layer1 = tex2D (_layer1Tex, IN.uv_MainTex * _layer1Tiling);
            half3 layer1norm = UnpackNormal(tex2D(_layer1Norm, IN.uv_MainTex * _layer1Tiling));
            
            half layer1Breakup = tex2D (_layer1Breakup, IN.uv_MainTex * _layer1Tiling).r;
            
            half3 detnorm = UnpackNormal(tex2D (_DetailBump, IN.uv_MainTex * _DetailTiling));

            half3 modNormal = norm + half3(layer1norm.r * 0.6, layer1norm.g * 0.6, 0);


            //Prepare Blend Masks
            half blend = dot(WorldNormalVector(IN, modNormal), layer1direction);
            
   
                   
            half blend2 = (blend * _Power + _Shift) * lerp(1, layer1Breakup, _layer1BreakupAmnt);
            
            blend2 = saturate(pow(blend2, 3));

        
          
            //Combine Normals          
            half3 blendedNormal = lerp(norm, layer1norm, blend2);
            blendedNormal = blendedNormal + (detnorm * half3(_DetailInt,_DetailInt,0));
    
            //Combine Diffuse layers
            half3 blendedColor = lerp(main, layer1, blend2);

            o.Albedo = blendedColor;
            o.Occlusion = ao.rgb;
            o.Smoothness = lerp(spec.a, layer1.a, blend2);
            o.Metallic = lerp(spec.r, _layer1Metal, blend2);
            o.Normal = blendedNormal.rgb;
           

        }
        ENDCG
    } 
    FallBack "Bumped Specular"
}