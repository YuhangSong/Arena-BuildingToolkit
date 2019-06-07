Shader "Sun_Temple/Decal" {
    Properties {
    	 _Color ("Color Tint (RGB), Fade (A)", Color) = (0.5, 0.5, 0.5, 0)
        _MainTex ("Albedo (RGB), Alpha (A)", 2D) = "white" {} 
        _Cutoff("Alpha Cutoff", range(0, 1)) = 0.7

        [NoScaleOffset] _DetailAlbedo ("DETAIL_Albedo", 2D) = "grey" {}
        _DetailTiling("DETAIL_Tiling", float) = 2  
    }

 
    
    SubShader {
        Tags {"Queue" = "AlphaTest" "RenderType" = "TransparentCutout"  "ForceNoShadowCasting" = "True"}
        LOD 400
        Cull Back           
        Offset -1, -1 



        CGPROGRAM
        #pragma surface surf Lambert fullforwardshadows nolightmap
        #pragma target 3.0    

        sampler2D _MainTex, _DetailAlbedo;
        half4 _Color;
        half _Cutoff, _DetailTiling ;

        struct Input {
            half2 uv_MainTex;
            half2 uv_Dissolve;    
        };


        void surf (Input IN, inout SurfaceOutput o) {
            half4 albedo = tex2D (_MainTex, IN.uv_MainTex);

            half detailAlbedo = tex2D(_DetailAlbedo, IN.uv_MainTex * _DetailTiling).r * unity_ColorSpaceDouble.rgb;  
            albedo.rgb = albedo.rgb * _Color.rgb * LerpWhiteTo(detailAlbedo, 1);
            
            half alphaMask = albedo.a * detailAlbedo * _Color.a;
                          
       
        	clip(alphaMask - _Cutoff);
        	o.Albedo = lerp(_Color.rgb, albedo.rgb, alphaMask);  
        	o.Alpha = alphaMask;
                 
           
        }
        ENDCG

    } 
  Fallback "Bumped Specular"
}