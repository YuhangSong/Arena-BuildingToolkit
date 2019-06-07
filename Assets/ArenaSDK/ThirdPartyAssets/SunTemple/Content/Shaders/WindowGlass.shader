Shader "Sun_Temple/WindowGlass" {
    Properties {
        _MainTex ("Albedo (RGB) Glass Mask(A)", 2D) = "white" {}  
        [NoScaleOffset]_RoughnessTexture ("Roughness (R)", 2D) = "white" {}       
        [Normal][NoScaleOffset]_BumpMap ("Normal", 2D) = "bump" {}
        [NoScaleOffset]_Emission("Emission(RGB)", 2d) = "black" {}
         
        _EmissionIntensity("Emission Intensity", range(0, 8)) = 0
        _EmissionVertexMask("Emission Vertex Mask", range(0, 1)) = 0
        _Reflection("Reflection (CUBE)", CUBE) = ""{}
        _SkyColor("Sky Color (RGB)", color) = (1, 1, 1, 1)

    
    }


    SubShader {
        Tags { "RenderType"="Opaque" "Queue" = "Geometry" "ForceNoShadowCasting" = "True" }
        LOD 500
 
        CGPROGRAM
        #pragma surface surf Standard 
        #pragma target 3.0

        sampler2D _MainTex, _BumpMap, _Emission, _RoughnessTexture;
        half _EmissionIntensity, _EmissionVertexMask;
        samplerCUBE _Reflection;
        half3 _SkyColor;
       

        struct Input {
        	half2 uv_MainTex;  
            half4 color : COLOR;
            half3 viewDir; 
            half3 worldRefl; INTERNAL_DATA                
        };


        void surf (Input IN, inout SurfaceOutputStandard o) {     

            half4 color = tex2D(_MainTex, IN.uv_MainTex);
            half3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
            half roughness = tex2D(_RoughnessTexture, IN.uv_MainTex).r;
            half emissionMask = lerp(0, 1, pow(IN.color.r, 4));
          	half3 emission = tex2D(_Emission, IN.uv_MainTex) * emissionMask * _EmissionIntensity * _SkyColor;
      

            half fresnel = 1.0 - saturate(dot (normalize(IN.viewDir), normal));

           
            //half3 worldReflWithNormal = WorldReflectionVector (IN, o.Normal);
            //half3 reflection = texCUBE(_Reflection, worldRefl).rgb;

            o.Normal = normal; 
            half3 reflection = texCUBE(_Reflection, IN.worldRefl).rgb;         
           
            
            reflection = reflection * (1 - roughness * 2) * pow(fresnel, 2);     

            o.Albedo = color.rgb;             
            o.Emission = emission.rgb + saturate(reflection);
            o.Metallic = 0;
           o.Smoothness = roughness;
           

        }
        ENDCG
    } 
    FallBack "Standard"
}