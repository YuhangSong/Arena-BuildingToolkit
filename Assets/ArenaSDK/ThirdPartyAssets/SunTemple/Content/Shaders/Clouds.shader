Shader "Sun_Temple/Clouds" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		

		_DistortionTexture("Distortion Texture", 2D) = "black" {}
		_DistortionIntensity("Distortion Intensity", range(0, 1)) = 0.5
		_ScrollSpeed("Scroll Speed", float)	= 0.5

	}


	SubShader { 
		Tags { "RenderType"="Opaque" "Queue"="Overlay" "IgnoreProjector"="True" }
		LOD 200
		Cull Off		
		ZWrite Off
		Blend OneMinusDstColor One
		Fog {Mode Off}
		
		CGPROGRAM
		#pragma surface surf SimpleUnlit nofog
		#pragma target 3.0

		sampler2D _MainTex, _DistortionTexture;
		half _ScrollSpeed, _DistortionIntensity;
		half4 _Color;


		half4 LightingSimpleUnlit (SurfaceOutput s, half3 lightDir, half atten) {
              half NdotL = dot (s.Normal, lightDir);
              half4 c;
              c.rgb = s.Albedo;
              c.a = s.Alpha;
              return c;
          }

		struct Input {
			half2 uv_MainTex;	
		};	


		void surf (Input IN, inout SurfaceOutput o) {


			half scrollX = _ScrollSpeed * _Time;
			half2 uv_scrolled = IN.uv_MainTex + half2(scrollX, 0);

			half distortion = tex2D(_DistortionTexture, uv_scrolled);

		
			half uv_distorted_x = (distortion * _DistortionIntensity * 0.1) - 0.05;	

			half2 uv_distorted_xy = IN.uv_MainTex + half2(uv_distorted_x, 0);



			half3 col = tex2D(_MainTex, uv_distorted_xy);
			half3 finalAlbedo =  col * _Color;			
			o.Albedo = saturate(finalAlbedo);	
		
		}
		ENDCG

	}

FallBack "Diffuse"
}

