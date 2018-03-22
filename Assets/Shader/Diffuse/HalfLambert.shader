// File Name:                 HalfLambert.shader
// Author:                    huijie wu
// Create Time:               2018-03-21 19:14:37
// Description:               漫反射 简单的半兰伯特光照模型 逐像素光照


Shader "MFShader/HalfLambert"
{
	Properties
	{
		_Diffuse ("Diffuse", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Pass
		{
			Tags {"LightMode" = "ForwardBase"}

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag 
			#pragma multi_compile_fog			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			fixed4 _Diffuse;

			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0; 
			};

			v2f vert(a2v v ) {
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);				// 转换到裁剪空间坐标

				o.worldNormal = UnityObjectToWorldDir(v.normal);	// 将顶点法线从模型空间变换为世界空间

				return o;
			}

			fixed3 halfLambert(float3 worldNormal) {
				fixed3 vertWorldNormal = normalize(worldNormal);	// 归一化

				fixed3 lightWorldNormal = normalize(_WorldSpaceLightPos0.xyz);

				fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * (dot(vertWorldNormal, lightWorldNormal) * 0.5 + 0.5);	//半兰伯特公式

				return diffuse;
			}

			fixed4 frag(v2f i) : SV_Target{
				return fixed4(UNITY_LIGHTMODEL_AMBIENT.xyz + halfLambert(i.worldNormal), 1);
			}
			
			ENDCG
		}
	}

	FallBack "Diffuse"
}