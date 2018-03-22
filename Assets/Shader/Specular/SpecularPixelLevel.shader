// File Name:                 SpecularPixelLevel.shader
// Author:                    huijie wu
// Create Time:               2018-03-22 15:51:09
// Description:               Phong光照模型 高光反射 逐像素光照


Shader "MFShader/SpecularPixelLevel"
{
	Properties
	{
		_Diffuse ("Diffuse", Color) = (1, 1, 1, 1)
		_Specular ("Specular", Color) = (1, 1, 1, 1)
		_Gloss ("Gloss", Range(8.0, 256)) = 20
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

			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
			};

			fixed4 _Diffuse;
			fixed4 _Specular;
			float  _Gloss;

			fixed3 Lambert(float3 worldNormal) {
				fixed3 vertWorldNormal = normalize(worldNormal);

				fixed3 lightWorldNormal = normalize(_WorldSpaceLightPos0.xyz);

				fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(vertWorldNormal, lightWorldNormal));

				return diffuse;
			}

			fixed3 Phong(float3 worldPos, float3 worldNormal) {
				// 计算反射方向 reflect要求光源的方向是由反射点指向光源 所以要取反
				fixed3 reflectDir = normalize(reflect(normalize(-_WorldSpaceLightPos0.xyz), normalize(worldNormal)));

				// 计算视角方向 由顶点的世界坐标指向摄像机的世界坐标
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);

				return _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(reflectDir, viewDir)), _Gloss);
			}

			v2f vert(a2v i) {
				v2f o;

				o.pos = UnityObjectToClipPos(i.vertex);

				o.worldNormal = UnityObjectToWorldDir(i.normal);

				o.worldPos = UnityObjectToWorldDir(i.vertex);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

				fixed3 diffuse = Lambert(i.worldNormal);

				fixed3 specular = Phong(i.worldPos, i.worldNormal);

				return fixed4(ambient + diffuse + specular, 1.0);
			}
			
			ENDCG
		}
	}

	FallBack "Specular"
}