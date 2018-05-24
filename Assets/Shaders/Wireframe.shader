// Upgrade NOTE: replaced 'glstate.matrix.mvp' with 'UNITY_MATRIX_MVP'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Wireframe" {
	Properties {
		_LineColor ("Line Color", Color) = (1,1,1,1)
		_GridColor ("Grid Color", Color) = (0,0,0,0)
		_LineWidth ("Line Width", float) = 0.1
	}
	SubShader {
        Pass {
 
			CGPROGRAM
			 
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			 
			uniform float4 _LineColor;
			uniform float4 _GridColor;
			uniform float _LineWidth;
			 
			// vertex input: position, uv1, uv2
			struct appdata {
			    float4 vertex : POSITION;
			    float4 texcoord1 : TEXCOORD1;
			    float4 color : COLOR;
			};
			 
			struct v2f {
			    float4 pos : POSITION;
			    float4 texcoord1 : TEXCOORD1;
			    float4 color : COLOR;
			};
			 
			v2f vert (appdata v) {
			    v2f o;
			    o.pos = UnityObjectToClipPos( v.vertex);
			    o.texcoord1 = v.texcoord1;
			    o.color = v.color;
			    return o;
			}

			bool isEdge (float4 i ) {
				float diff = i.y - i.x;
				if (i.x < _LineWidth && i.x > 0) return true;
				if (i.y < _LineWidth && i.y > 0) return true;
				if (diff < _LineWidth && diff > 0) return true;
				if (i.x > (1 - _LineWidth) && i.x < 1) return true;
				if (i.y > (1 - _LineWidth) && i.y < 1) return true;
				if (diff > (1 - _LineWidth) && diff < 1) return true;
				return false;
			}
			float4 frag(v2f i ) : COLOR
			{
				if (isEdge(i.texcoord1))
				{
					return _LineColor;
				}

				return _GridColor;
			}
			 
			ENDCG
			 
        }
	} 
	Fallback "Vertex Colored", 1
}