Shader "Custom/VertexColor" {
	SubShader{
		Tags { "RenderType" = "Opaque" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" }
		LOD 200

		HLSLPROGRAM
		#pragma surface surf Lambert vertex:vert
		#pragma target 3.0

		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		struct Input {
			float4 vertColor;
		};

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.vertColor = v.color;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = IN.vertColor.rgb;
		}
		ENDHLSL
	}
		FallBack "Diffuse"
}