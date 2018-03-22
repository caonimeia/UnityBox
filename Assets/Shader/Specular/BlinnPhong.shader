// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// File Name:                 BlinnPhong.shader
// Author:                    huijie wu
// Create Time:               2018-03-22 16:08:24
// Description:               BlinnPhong光照模型 高光反射 逐像素光照


Shader "MFShader/BlinnPhong"
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

			fixed3 BlinnPhong(float3 worldPos) {
				// 计算视角方向 由顶点的世界坐标指向摄像机的世界坐标
				// fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos.xyz);
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));

				// fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(worldPos));

				// 视角方向 + 光照方向 再进行归一化
				fixed3 halfDir = normalize(viewDir + worldLightDir);

				return _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(viewDir, halfDir)), _Gloss);
			}

			v2f vert(a2v i) {
				v2f o;

				o.pos = UnityObjectToClipPos(i.vertex);

				// o.worldNormal = mul(i.normal, (float3x3)unity_WorldToObject);

				// o.worldNormal = mul((float3x3)unity_ObjectToWorld, i.normal);

				o.worldNormal = UnityObjectToWorldNormal(i.normal);

				o.worldPos = mul(unity_ObjectToWorld, i.vertex).xyz;
				// o.worldPos = UnityObjectToWorldDir(i.vertex);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

				fixed3 diffuse = Lambert(i.worldNormal);

				fixed3 specular = BlinnPhong(i.worldPos);

				return fixed4(ambient + diffuse + specular, 1.0);
			}
			
			ENDCG
		}
	}

	FallBack "Specular"
}