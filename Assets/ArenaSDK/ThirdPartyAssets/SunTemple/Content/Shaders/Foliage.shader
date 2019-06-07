// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Sun_Temple/Foliage" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Layer_A Albedo (RGB)", 2D) = "black" {} 
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        _SelfIllum("Self Illumination", range(0, 1)) = 0
        _NormalModification("Normal Modification", range(0, 1)) = 1

        _WaveFreq("Wave Frequency", float) = 20
        _WaveHeight("Wave Height", float) = 0.1  
         _WaveScale("Wave Scale", float) = 1
    }

 
    
    SubShader {        
        Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
        LOD 200
      
     

        Cull Back
        CGPROGRAM
        #pragma surface surf Standard alphatest:_Cutoff vertex:vert fullforwardshadows addshadow novertexlights nolightmap
        #pragma target 3.0
        #include "VertexWind.cginc"  
     

       sampler2D _MainTex;
       fixed4 _Color;   
       //half _Cutoff;
       half _SelfIllum;
       half _WaveFreq, _WaveHeight, _WaveScale;   
       half _NormalModification;

        struct Input {
            half2 uv_MainTex; 
                
        };


        void vert (inout appdata_full v) {         
             v.vertex.xyz = v.vertex.xyz + wind_simplified(v.vertex.xyz, v.color, _WaveFreq, _WaveHeight, _WaveScale); 

            // half3 modified_dir = mul(unity_WorldToObject, half4(0, 2, 0, 0));

            half3 modifiedNormal = half3(0, 2, 0);

            v.normal = lerp(v.normal, v.normal + modifiedNormal, _NormalModification);

                            
                    
        }


        void surf (Input IN, inout SurfaceOutputStandard o) {              
           
            half4 albedo = tex2D (_MainTex, IN.uv_MainTex); 
            o.Albedo = albedo.rgb * _Color; 
           o.Emission = albedo.rgb * _SelfIllum; 
          
                
            o.Alpha = albedo.a;   
        }
        ENDCG

      
     

    } 
   Fallback Off
}