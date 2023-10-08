// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Overlay" {
    Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
        _OverlayTex ("Overlay (RGB)", 2D) = "white" {}
    }
   
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+1000"}
        LOD 200
		Blend SrcAlpha OneMinusSrcAlpha 
       
        GrabPass {}
       
        Pass {
            CGPROGRAM
           
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"
           
            sampler2D _GrabTexture;
            sampler2D _OverlayTex;
			fixed4 _Color;
           
            struct appdata_t {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
            };
           
            struct v2f {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 projPos : TEXCOORD1;
            };
           
            float4 _OverlayTex_ST;
           
            v2f vert( appdata_t v ){
                v2f o;
                o.vertex = UnityObjectToClipPos( v.vertex );
                o.uv = TRANSFORM_TEX( v.texcoord, _OverlayTex );
                o.projPos = ComputeScreenPos( o.vertex );
                return o;
            }
           
            half4 frag( v2f i ) : COLOR {
                i.projPos /= i.projPos.w;
#if SHADER_API_METAL || SHADER_API_D3D11
				half4 base = tex2D(_GrabTexture, float2(i.projPos.x, 1-i.projPos.y));
#else
				half4 base = tex2D(_GrabTexture, float2(i.projPos.x, i.projPos.y));
#endif
                half4 overlay = tex2D( _OverlayTex, i.uv );
 
                half4 output = lerp(   1 - 2 * (1 - base) * (1 - overlay),    2 * base * overlay,    step( base, 0.5 ));
				output.a = _Color.a;
				return output;
            }
           
            ENDCG
        }
    }
}