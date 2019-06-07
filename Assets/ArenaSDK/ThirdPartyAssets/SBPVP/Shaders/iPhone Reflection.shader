Shader "iPhone/Reflection/SphereMapped" {

Properties {
    _Color ("Main Color", Color) = (1,1,1)
    _SpecColor ("Spec Color", Color) = (1,1,1)
    _Shininess ("Shininess", Range (0.03, 1)) = 0.7
    _MainTex ("Base (RGB) RefStrength (A)", 2D) = "white" {}
    _Sphere ("Reflection SphereMap", 2D) = "white" {TexGen SphereMap} 
}

Category {
    Lighting On
    Material {
        Diffuse[_Color]
        Ambient(1,1,1)
        Shininess[_Shininess]
        Specular[_SpecColor]
    }

    // iPhone 3GS and later
    SubShader {Pass {
        SetTexture[_Sphere]
        SetTexture[_MainTex] {Combine previous * texture alpha}
        SetTexture[_MainTex] {Combine previous + texture}
        SetTexture[_] {Combine previous * primary DOUBLE}
    } }

    // pre-3GS devices, including the September 2009 8GB iPod touch
    SubShader
    { 
        Pass
        {
            SetTexture[_Sphere] {Combine texture * primary DOUBLE}
        }
        Pass
        {
            Blend One SrcAlpha
            SetTexture[_MainTex] {Combine texture * primary DOUBLE, texture}
        }
    }
}
Fallback "VertexLit"
}

