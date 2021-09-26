 Shader "GradientDissolve" {
 Properties {
     _MainTex ("Base (RGB)", 2D) = "white" {}
     _Color1 ("Bottom Color", Color) = (1,1,1,1)
     _Color2 ("Top Color", Color) = (0,0,0,1)
     _Scale ("Scale", Float) = 0.24
     _Offset ("Offset", Float) = 0
     _Glossiness ("Smoothness", Range(0,1)) = 1.0
     _Metallic ("Metallic", Range(0,1)) = 0.0
 _SliceGuide ("Slice Guide (RGB)", 2D) = "white" {}
      _SliceAmount ("Slice Amount", Range(0.0, 1.0)) = 0.5
 
 }
 
 
 
 SubShader {
     Tags { "RenderType"="Opaque" "DisableBatching"="True"}
     LOD 200
 
     CGPROGRAM
     #pragma surface surf Standard fullforwardshadows vertex:vert
     #pragma target 3.0
 
     sampler2D _MainTex;
     half _Glossiness;
     half _Metallic;
     fixed4 _Color1;
     fixed4 _Color2;
     fixed  _Scale;
     fixed  _Offset;
 
 
     struct Input {
         float2 uv_MainTex;
         float3 localPos;
 		float2 uv_SliceGuide;
          float _SliceAmount;
     };
 
     void vert (inout appdata_full v, out Input o){
         UNITY_INITIALIZE_OUTPUT(Input,o);
         o.localPos = v.vertex.xyz;
     } 
 		
      sampler2D _SliceGuide;
      float _SliceAmount;
 
     void surf (Input IN, inout SurfaceOutputStandard o) {
         clip(tex2D (_SliceGuide, IN.uv_SliceGuide).rgb - _SliceAmount);
         half4 c = tex2D(_MainTex, IN.uv_MainTex);
         c.rgb *= lerp(_Color1,_Color2, (IN.localPos.y * _Scale)+_Offset);
         o.Albedo = c.rgb;
         o.Alpha = c.a;
         o.Metallic = _Metallic;
         o.Smoothness = _Glossiness;
 		
     }
	
     ENDCG
 }
 

 
 Fallback "Diffuse"
 }