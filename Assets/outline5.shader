// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "BarycentricWireframeUv1"
{
	Properties
	{
		_LineColor ("Line Color", Color) = (1,1,1,1)
		_GridColor ("Grid Color", Color) = (0,0,0,0)
		_LineWidth ("Line Width", float) = 0.1
	}
	SubShader
	{
        Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			uniform float4 _LineColor;
			uniform float4 _GridColor;
			uniform float _LineWidth;

			// vertex input: position, uv1, uv2
			struct appdata
			{
				float4 vertex : POSITION;
				float4 texcoord1 : TEXCOORD1;
				float4 color : COLOR;
			};

			struct v2f
			{
				float4 pos : POSITION;
				float4 texcoord1 : TEXCOORD1;
				float4 color : COLOR;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex);
				o.texcoord1 = v.texcoord1;
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 answer;
				if (i.texcoord1.x < _LineWidth || i.texcoord1.y < _LineWidth)
				{
					answer = _LineColor;
				}
				else if ((i.texcoord1.x - i.texcoord1.y) < _LineWidth && (i.texcoord1.y - i.texcoord1.x) < _LineWidth)
				{
					answer = _LineColor;
				}
				else
				{
					answer = _GridColor;
				}
				return answer;
			}
			ENDCG
        }
	} 
	Fallback "Vertex Colored", 1
}