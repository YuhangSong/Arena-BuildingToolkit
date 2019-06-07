Shader "Sun_Temple/VertexBlend" {
    Properties {

        _Color("BASE Tint (RGB), Tint Fade (A)", Color) = (0.5, 0.5, 0.5, 0)
        _MainTex ("BASE Albedo (RGB) Tint Mask (A)", 2D) = "white" {}       
        [Normal]_BumpMap ("BASE Normal (RGB)", 2D) = "bump" {} 
        _Roughness("BASE Roughness", range(0,1)) = 1
        //_OcclusionMap("BASE AO", 2D)= "white" {}

        [NoScaleOffset] _layer1Tex ("LAYER_B Albedo (RGB)", 2D) = "white" {}       
        [Normal][NoScaleOffset] _layer1Norm("LAYER_B Normal (RGB)", 2D) = "bump" {}
        _layer1Tiling("LAYER_B Tiling", float) = 1
        _layer1Rough("LAYER_B Roughness", range(0, 1)) = 1

        [NoScaleOffset] _BlendMask("BLEND_Mask (R)", 2D) = "white" {}
        _BlendTile("BLEND_Tiling", float) = 1
        _Choke ("BLEND_Choke", Range(0, 60) ) = 15
        _Crisp ("BLEND_Crispyness", range(1, 20)) = 5
       
    
        [NoScaleOffset] _DetailAlbedo ("DETAIL_Albedo (R)", 2D) = "grey" {}
        [Normal][NoScaleOffset] _DetailNormal ("DETAIL_Normal (RGB)", 2D) = "bump" {}  
        _DetailNormalStrength ("DETAIL_Normal Strength", Range(0,1)) = 0.4    
        _DetailTiling("DETAIL_Tiling", float) = 2              
    }


    
    
    SubShader {
        Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
        LOD 500
        CGPROGRAM            
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0
       

        sampler2D _MainTex, _BumpMap, _DetailAlbedo, _DetailNormal, _layer1Tex, _layer1Norm, _BlendMask;
        half4 _Color, _layer1Color, _SelfIlum;
        half _Choke, _Crisp, _BlendInvert, _BlendTile, _DetailNormalStrength, _DetailTiling, _layer1Tiling, _Roughness, _layer1Rough;


        struct Input {
            half2 uv_MainTex; 
            half2 uv_BumpMap;       
         
            half4 color : COLOR;           
        };


        void surf (Input IN, inout SurfaceOutputStandard o) {       
            
            // Base layer textures
            half4 main = tex2D (_MainTex, IN.uv_MainTex);            
            main = lerp(main, _Color, main.a * _Color.a);



            half3 normal = UnpackNormal(tex2D (_BumpMap, IN.uv_BumpMap));              

            // Layer1 Textures
            half3 layer1Albedo = tex2D (_layer1Tex, IN.uv_MainTex * _layer1Tiling);
            half3 layer1Normal = UnpackNormal(tex2D(_layer1Norm, IN.uv_MainTex * _layer1Tiling)); 

            half blendMask = tex2D(_BlendMask, IN.uv_MainTex * _BlendTile).r;
            blendMask = clamp(blendMask, .2, .9);

            // Detail Normal Texture
            half detailAlbedo = tex2D(_DetailAlbedo, IN.uv_MainTex * _DetailTiling).r;
            half3 detailNormal = UnpackNormal(tex2D (_DetailNormal, IN.uv_MainTex * _DetailTiling));

            // Blend Mask
            half blend = (IN.color.r * blendMask) * _Choke;
           
            blend = pow(blend, _Crisp);
            blend = saturate(blend);


            // Blended textures
            half3 blendedAlbedo = lerp(layer1Albedo, main, blend);
            blendedAlbedo = blendedAlbedo * LerpWhiteTo(detailAlbedo * unity_ColorSpaceDouble.rgb, 1);    
     
            half3 blendedNormal = lerp(layer1Normal, normal, blend);
            blendedNormal = blendedNormal + (detailNormal * half3(_DetailNormalStrength, _DetailNormalStrength, 0));
            half blendedSmoothness = lerp(_layer1Rough, _Roughness, blend);


       
            o.Albedo = blendedAlbedo;
    
            o.Smoothness = saturate(1 - blendedSmoothness);
            o.Metallic = 0;
            o.Normal = blendedNormal.rgb;
        }

        ENDCG  

    }

    FallBack "Standard"
}