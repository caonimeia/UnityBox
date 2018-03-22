// File Name:                 DiffusePixelLevel.shader
// Author:                    huijie wu
// Create Time:               2018-03-21 17:56:33
// Description:               漫反射 兰伯特模型 逐像素光照


Shader "MFShader/DiffusePixelLevel"
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

				o.pos = UnityObjectToClipPos(v.vertex);			//转换到裁剪空间坐标

				o.worldNormal = UnityObjectToWorldDir(v.normal);	//将顶点法线从模型空间变换为世界空间， 并进行归一化

				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;	//环境光照

				fixed3 vertWorldNormal = normalize(i.worldNormal);

				fixed3 lightWorldNormal = normalize(_WorldSpaceLightPos0.xyz);			//归一化光源方向

				fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(vertWorldNormal, lightWorldNormal)); // 光源颜色 * 漫反射颜色 * 顶点法线与光源方向的余弦值

				fixed3 color = ambient + diffuse;

				return fixed4(color, 1);
			}
			
			ENDCG
		}
	}

	FallBack "Diffuse"
}