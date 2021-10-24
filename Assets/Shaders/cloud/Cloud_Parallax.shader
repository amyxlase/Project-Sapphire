// Koke_Cacao: https://www.jianshu.com/p/3fef69e2efb6, https://cuihongzhi1991.github.io/blog/2020/05/27/builtinttourp/
Shader "Custom/Cloud Parallax" 
{
	Properties {
		_Color("Color",Color) = (1,1,1,1)
		_MainTex("MainTex",2D) = "white"{}
		_SDFTex("SDFTex",2D) = "white"{}
		_Height("Displacement Amount",range(0,1)) = 0.15
		_HeightAmount("Turbulence Amount",range(0,2)) = 1
		_HeightTileSpeed("Turbulence Tile&Speed",Vector) = (1.0,1.0,0.05,0.0)
		_ViewRayOffest("ViewRayOffest", Range(0,0.42)) = 0.2
		[KeywordEnum(PM,RPM,POM,SDF)] _MODE("Mode", Float) = 0
		[Toggle(ENABLE_SDF)] _ENABLE_SDF("ENABLE_SDF",Float) = 0
		[Toggle(LAYER2)] _JUMPOUT("Layer2",Float) = 0
		[Toggle(DEBUG)] _Debug("Debug",Float) = 0
 		_Step("Step", Range(1,64)) = 32
	}

	// Koke_Cacao: changed
	HLSLINCLUDE

	// Koke_Cacao: changed
	ENDHLSL

	SubShader 
	{
		// Koke_Cacao: added
    Tags {"RenderPipeline" = "UniversalPipeline"}
		LOD 300

		Pass
		{
			// Koke_Cacao: you don't need this
		  // Name "FORWARD"

      // Tags {"LightMode"="ForwardBase"}
      Tags {"LightMode"="UniversalForward"}

			Blend SrcAlpha OneMinusSrcAlpha // Koke_Cacao: should be fine https://docs.unity3d.com/Manual/SL-Blend.html
			//ZWrite Off
			Cull Off // Koke_Cacao: front, back, or off, https://docs.unity3d.com/Manual/SL-Cull.html

			// Koke_Cacao: changed
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// Koke_Cacao: shader variants: https://www.youtube.com/watch?v=3i2V8Q7SsOM&ab_channel=MertKirimgeri
			// Koke_Cacao: and https://www.codenong.com/cs106872268/
			#pragma shader_feature _MODE_PM _MODE_RPM _MODE_POM _MODE_SDF
			#pragma shader_feature __ ENABLE_SDF
			#pragma shader_feature __ LAYER2
			#pragma shader_feature __ DEBUG

			// Koke_Cacao: changed
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
			// #include "UnityCG.cginc"
			// #include "AutoLight.cginc"
			// #include "Lighting.cginc" // TODO
			
			#pragma multi_compile_fwdbase
            		#pragma target 3.0

			// Koke_Cacao: putting stuff in
			CBUFFER_START(UnityPerMaterial)
				sampler2D _MainTex;
				sampler2D _SDFTex;
				float4 _MainTex_ST;
				float4 _MainTex_TexelSize;
			CBUFFER_END

			half _Height;
			float4 _HeightTileSpeed;
			half _HeightAmount;
			half4 _Color;

			half4 _LightingColor;
			half _ViewRayOffest;
			half _Step;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normalDir : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float4 posWorld : TEXCOORD3;
				float2 uv2 : TEXCOORD4;
				float4 color : TEXCOORD5;
				// Koke_Cacao: Changed
				float fogCoord : TEXCOORD6;
				// UNITY_FOG_COORDS(7)
			};

			// Koke_Cacao: Added from https://www.cnblogs.com/leeplogs/p/7339097.html
			struct appdata_full {
				float4 vertex : POSITION;//顶点坐标
				float4 tangent : TANGENT;//正切
				float3 normal : NORMAL;//法线
				float4 texcoord : TEXCOORD0;//第一层UV
				float4 texcoord1 : TEXCOORD1; //第二层UV
				half4 color : COLOR; // fixed4 to half4
				// fixed4 color : COLOR; //颜色
			};

			v2f vert (appdata_full v) {
				v2f o;
				// Koke_Cacao: Changed
				o.pos = TransformObjectToHClip(v.vertex);
				// o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex) + frac(_Time.y*_HeightTileSpeed.zw);
				o.uv2 = v.texcoord * _HeightTileSpeed.xy;
				o.posWorld = mul(unity_ObjectToWorld, v.vertex); // TODO: unity_ObjectToWorld not sure
				// Koke_Cacao: Changed according to https://github.com/Unity-Technologies/Graphics/blob/master/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl
				o.normalDir = TransformObjectToWorldNormal(v.normal);
				// o.normalDir = UnityObjectToWorldNormal(v.normal);

				// Koke_Cacao: replaced https://www.cnblogs.com/laoyueblogs/p/13782797.html
				// TANGENT_SPACE_ROTATION;
				float3 binormal = cross(v.normal, v.tangent.xyz ) * v.tangent.w;
	      float3x3 rotation = float3x3(v.tangent.xyz, binormal, v.normal);

				// Koke_Cacao: changed // TODO: not sure
				o.viewDir = mul(rotation, TransformWorldToObject(GetCameraPositionWS()));
				// o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));
				o.color = v.color;
				// Koke_Cacao: Changed
				o.fogCoord = ComputeFogFactor(o.pos); // TODO: not sure
				// UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			float mipmapLevel(float2 uv, float2 textureSize)
			{
				float2 dx = ddx(uv * textureSize);
				float2 dy = ddy(uv * textureSize);
				float d = max(dot(dx, dx), dot(dy, dy));
				return 0.5 * log2(d);
			}

			float4 frag(v2f i) : COLOR
			{
				float3 viewRay=normalize(i.viewDir*-1);
				viewRay.z = abs(viewRay.z)+ _ViewRayOffest;
				viewRay.xy *= _Height;

				float3 shadeP = float3(i.uv,0);
				float3 shadeP2 = float3(i.uv2,0);

				float4 T = tex2D(_MainTex, shadeP2.xy);
				#if LAYER2 //ʹ��˫������
					float h2 = T.a * _HeightAmount;
				#else
					float h2 = 1;
				#endif
				float count = 0;

				#if ENABLE_SDF //ʹ��Ԥ�決SDF
					float sdf = tex2D(_SDFTex, i.uv).r * h2;
					shadeP += normalize(viewRay) * sdf;
				#endif

				#if _MODE_PM
					float3 sioffset = viewRay / viewRay.z;
					float d = 1.0 - tex2D(_MainTex, shadeP.xy).a * h2;
					shadeP += sioffset * d;
				#elif _MODE_RPM
					int linearStep = _Step;
					int binaryStep = _Step;
					// linear search
					float3 lioffset = viewRay / (viewRay.z * (linearStep + 1));
					for (int k = 0; k < linearStep; k++)
					{
						float d = 1.0 - tex2Dlod(_MainTex, float4(shadeP.xy, 0, 0)).a * h2;
						shadeP += lioffset * step(shadeP.z, d);
						count++;
					}
					// binary search
					float3 biOffset = lioffset;
					for (int j = 0; j < binaryStep; j++)
					{
						biOffset = biOffset * 0.5;
						float d = 1.0 - tex2Dlod(_MainTex, float4(shadeP.xy, 0, 0)).a * h2;
						shadeP += biOffset * sign(d - shadeP.z);
						count++;
					}
				#elif _MODE_POM
					float linearStep = _Step;
					float3 lioffset = viewRay / (viewRay.z * linearStep);
					float d = 1.0 - tex2Dlod(_MainTex, float4(shadeP.xy, 0, 0)).a * h2;
					float prev_d = d;
					float3 prev_shadeP = shadeP;
					while (d > shadeP.z)
					{
						prev_shadeP = shadeP;
						shadeP += lioffset;
						prev_d = d;
						d = 1.0 - tex2Dlod(_MainTex, float4(shadeP.xy, 0, 0)).a * h2;
						count++;
					}
					float d1 = d - shadeP.z;
					float d2 = prev_d - prev_shadeP.z;
					float w = d1 / (d1 - d2);
					shadeP = lerp(shadeP, prev_shadeP, w);
				#elif _MODE_SDF
					float minLod = max(0,mipmapLevel(i.uv, _MainTex_TexelSize.zw));
					float maxLod = minLod + 4;
					float3 lioffset = viewRay / length(viewRay.xy) * _MainTex_TexelSize.x;
					float3 minOffest = lioffset * 0.5;
					lioffset *= pow(2,maxLod);
					int lod = maxLod;

					while (lod >= minLod)
					{
						float d = 1.0 - tex2Dlod(_MainTex, float4(shadeP.xy, 0, lod)).a * h2;
						count++;

						float3 prev_shadeP = shadeP;
						float prev_d = d;
						while (count < _Step && d - shadeP.z > 0)
						{
							prev_shadeP = shadeP;
							prev_d = d;
							shadeP += lioffset;
							d = 1.0 - tex2Dlod(_MainTex, float4(shadeP.xy, 0, lod)).a * h2;
							count++;
						}
						//if (prev_d - prev_shadeP.z < minOffest.z) break;
						shadeP = prev_shadeP;
						
						lod--;
						lioffset *= 0.5;
					}
				#endif
				#if DEBUG
				count /= 20;
				return float4(count,count,count,1);
				#endif

				half4 c = half4(tex2D(_MainTex,shadeP.xy).rgb * T.rgb,1) * _Color;
				return c;
			}
		ENDHLSL
		}
	}

	FallBack "Diffuse"
}
