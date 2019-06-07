Shader "Sun_Temple/Cloth" {
    Properties {
        _MainTex ("Layer_A Albedo (RGB)", 2D) = "white" {} 
        _SelfIllum("Self Illumination", range(0, 1)) = 0

        [NoScaleOffset] _DetailAlbedo ("DETAIL_Albedo", 2D) = "grey" {}
        _DetailTiling("DETAIL_Tiling", float) = 2  

        _WaveFreq("Wave Frequency", float) = 20
        _WaveHeight("Wave Height", float) = 0.1  
        _WaveScale("Wave Scale", float) = 1

        
    }

 
    
    SubShader {        
        Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
        LOD 400
        Cull Back

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert addshadow noforwardadd nolightmap
        #pragma target 3.0 

       sampler2D _MainTex, _DetailAlbedo; 
       half _DetailTiling;
       half _SelfIllum;
       half _WaveFreq, _WaveHeight, _WaveScale;   

        struct Input {
            half2 uv_MainTex;       
        };




        half3 windanim(half3 vertex_xyz, half2 color, half _WaveFreq, half _WaveHeight, half _WaveScale){
				half phase_slow = _Time * _WaveFreq;
	            half phase_med = _Time * 4 * _WaveFreq;
	           
	            half offset = (vertex_xyz.x + (vertex_xyz.z * _WaveScale)) * _WaveScale;
	            half offset2 = (vertex_xyz.x + (vertex_xyz.z * _WaveScale * 2)) * _WaveScale * 2;
	         
	            half sin1 = sin(phase_slow + offset);
	            half sin2 = sin(phase_med + offset2);          
	 
	            half sin_combined = (sin1 * 4) + sin2 ;
	           
	            half wind_x =  sin_combined * _WaveHeight * 0.1;
	            half3 wind_xyz = half3(wind_x, wind_x * 2, wind_x);


	            wind_xyz = wind_xyz * pow(color.r, 2);	     
				return wind_xyz;
		}



        void vert (inout appdata_full v) {                                                                         

            v.vertex.xyz = v.vertex.xyz + windanim(v.vertex.xyz, v.color, _WaveFreq, _WaveHeight, _WaveScale);   
                       
        }


        void surf (Input IN, inout SurfaceOutput o) {              
           
            half4 albedo = tex2D (_MainTex, IN.uv_MainTex );  
            half detailAlbedo = tex2D(_DetailAlbedo, IN.uv_MainTex * _DetailTiling).r * unity_ColorSpaceDouble.rgb; 


            albedo.rgb = albedo.rgb * LerpWhiteTo(detailAlbedo, 1);
            o.Albedo = albedo.rgb; 
            o.Emission = albedo.rgb * _SelfIllum; 

        }
        ENDCG



    } 
   Fallback Off
}