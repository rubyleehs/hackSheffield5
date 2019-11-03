Shader "Hidden/Ripple Effect"
{
    Properties
    {
        _MainTex("Base", 2D) = "white" {}
        _GradTex("Gradient", 2D) = "white" {}
        _Params1("Parameters 1", Vector) = (1, 1, 0.8, 0)
        _Params2("Parameters 2", Vector) = (1, 1, 1, 0)
        _Ripple("Ripple", Vector) = (0.49, 0.5, 0, 0)
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float2 _MainTex_TexelSize;

    sampler2D _GradTex;
    
    float4 _Params1;    // [ aspect, 1, 0, 0 ]
    float4 _Params2;    // [ 1, 1/aspect, refraction, 0 ]
    
    static const int _RippleCount = 5;
    float4 _Ripples[5];

    float wave(float2 position, float2 origin, float time, float _oneOverScale)
    {
        float normalizedDistance = length(position - origin) * _oneOverScale;
        if (normalizedDistance > 1) return 0;

        float t = time * 2 - normalizedDistance; // I think something wrong here but meh

        return (tex2D(_GradTex, float2(t,0)).a - 0.5f) * 2 * (1 - normalizedDistance) * (6 - _oneOverScale);
    }

    float allwave (float2 position) {

        float result = 0;

        for (int i = 0; i < _RippleCount; i++) {
        
            float4 ripple = _Ripples[i];
            if (ripple.w <= 0) return result;
            result += wave(position,ripple.xy,ripple.z,ripple.w);

        }

        return result;
    }

    half4 frag(v2f_img i) : SV_Target
    {
        const float2 dx = float2(0.01f, 0);
        const float2 dy = float2(0, 0.01f);

        float2 p = i.uv * _Params1.xy;

        float w = allwave(p);

        /*
        half4 dc = tex2D(_MainTex, i.uv);
        return dc * (1 - abs(w));
        */

        float2 dw = float2(allwave(p + dx) - w, allwave(p + dy) - w);

        float2 duv = dw * _Params2.xy * 0.2f * _Params2.z;
        half4 c = tex2D(_MainTex, i.uv + duv);

        return c;

        /*
        float fr = pow(length(dw) * 3 * _Params2.w, 3);
        return lerp(c, _Reflection, fr);
        */
    }

    ENDCG

    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            Fog { Mode off }
            CGPROGRAM
            #pragma fragmentoption ARB_precision_hint_fastest 
            #pragma target 3.0
            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
    } 
}
