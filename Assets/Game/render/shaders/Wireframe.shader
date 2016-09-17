Shader "Custom/Wireframe" {
	Properties {
		_Color("Line Color", Color) = (1,1,1,1)
		_Thickness("Thickness", Float) = 1
		_Resolution("Resolution For Thickness", Float) = 1080
		_FaceColor("Face Color", Color) = (0,0,0,0)
	}
	SubShader {

		Pass{
			Tags{ "RenderType" = "Opaque" "Queue" = "Geometry" }

			Blend SrcAlpha OneMinusSrcAlpha
			LOD 200

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
			};

			struct v2f {
				float4 position: POSITION;
			};

			float4 _FaceColor = { 0,0,0,0 };

			v2f vert(appdata IN)
			{
				v2f OUT;

				OUT.position = mul(UNITY_MATRIX_MVP, IN.vertex);

				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target{
				return _FaceColor;
			}

			ENDCG
		}

		Pass{
			Tags{ "RenderType" = "Opaque" "Queue" = "Geometry" }

			Blend SrcAlpha OneMinusSrcAlpha
			LOD 200

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			#include "UnityCG.cginc"

			//vector to geo
			struct v2g
			{
				float4	pos		: POSITION;		// vertex position
			};

			//geo to fragment
			struct g2f
			{
				float4	pos		: POSITION;		// fragment position
				float3  dist	: TEXCOORD1;	// distance to each edge of the triangle
			};

			float _Thickness = 1;		// line thickness
			float4 _Color = { 1,1,1,1 };	// line color
			float _ThicknessAtResolution = 1080;

			//vert
			//Build object
			// Vertex Shader
			v2g vert(appdata_base v)
			{
				v2g output;
				output.pos = mul(UNITY_MATRIX_MVP, v.vertex);

				return output;
			}

			// Geometry Shader
			//build frame
			[maxvertexcount(3)]
			void geom(triangle v2g p[3], inout TriangleStream<g2f> triStream)
			{
				//points in screen space
				float2 p0 = _ScreenParams.xy * p[0].pos.xy / p[0].pos.w;
				float2 p1 = _ScreenParams.xy * p[1].pos.xy / p[1].pos.w;
				float2 p2 = _ScreenParams.xy * p[2].pos.xy / p[2].pos.w;

				//edge vectors
				float2 v0 = p2 - p1;
				float2 v1 = p2 - p0;
				float2 v2 = p1 - p0;

				//area of the triangle
				float area = abs(v1.x*v2.y - v1.y * v2.x);

				//values based on distance to the edges
				float dist0 = area / length(v0);
				float dist1 = area / length(v1);
				float dist2 = area / length(v2);

				g2f pIn;

				//add the first point
				pIn.pos = p[0].pos;
				pIn.dist = float3(dist0, 0, 0);
				triStream.Append(pIn);

				//add the second point
				pIn.pos = p[1].pos;
				pIn.dist = float3(0, dist1, 0);
				triStream.Append(pIn);

				//add the third point
				pIn.pos = p[2].pos;
				pIn.dist = float3(0, 0, dist2);
				triStream.Append(pIn);
			}

			// Fragment Shader
			//render lines
			float4 frag(g2f input) : COLOR
			{
				//find the smallest distance
				float val = min(input.dist.x, min(input.dist.y, input.dist.z));
				
				float modifiedThickness = _Thickness * (_ScreenParams.y / _ThicknessAtResolution);

				//calculate power to 2 to thin the line
				val = exp2(-1 / (_Thickness) * val * val);

				//blend between the lines and the negative space to give illusion of anti aliasing
				float4 targetColor = _Color;
				float4 transCol = _Color;
				transCol.a = 0;
				float4 col = val * targetColor + (1 - val) * transCol;

				if (col.a < 0.5f) discard;
				else col.a = 1.0f;

				return col;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
