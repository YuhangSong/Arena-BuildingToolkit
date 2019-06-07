Shader "Sun_Temple/Decal_Puddle" {
    Properties {
    	 _Color ("Color", Color) = (0.5, 0.5, 0.5, 0)
         _Mask("Mask (R)", 2D) = "black" {}
         _MaskFade("Mask Fade", range(0, 1)) = 0
        _BumpMap("Normal (RGB)", 2D) = "bump"{}


        _Roughness("Roughness", range(0, 1)) = 0

        _ScrollSpeed("ScrollSpeed", range(0, 4)) = 2

    }

 
    
    SubShader {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
        LOD 200
        Cull Back
        Offset -1, -1 


        CGPROGRAM
        #pragma surface surf Standard alpha:fade noforwardadd nolightmap
        #pragma target 3.0    

        sampler2D _Mask, _BumpMap;
        half4 _Color;
        half _MaskFade, _Roughness, _ScrollSpeed ;

        struct Input {
        	half2 uv_MainTex;
            half2 uv_BumpMap;           
        };


        void surf (Input IN, inout SurfaceOutputStandard o) {

        	half scrollX = _ScrollSpeed * _Time;
			half scrollY = _ScrollSpeed * _Time;

		
			half2 uv1 = IN.uv_BumpMap + half2(scrollX, scrollY);
			half2 uv2 = IN.uv_BumpMap - half2(scrollX, scrollY);


            half3 albedo = _Color.rgb;
            half3 normal_a = UnpackNormal(tex2D (_BumpMap, uv1));
            half3 normal_b = UnpackNormal(tex2D(_BumpMap, uv2));

            half3 normalCombined = normal_a + normal_b;

            half alpha = tex2D(_Mask, IN.uv_MainTex).r;                        
                                      
        
           o.Albedo = albedo.rgb;  
           o.Metallic = _Color.a;
           o.Normal = normalCombined;
           o.Smoothness = saturate(1 - _Roughness);
           o.Alpha = lerp(alpha, 0, _MaskFade);

        }
        ENDCG

    } 
  Fallback "Standard"
}