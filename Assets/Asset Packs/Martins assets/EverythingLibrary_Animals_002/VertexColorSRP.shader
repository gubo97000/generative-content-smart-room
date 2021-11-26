/*If you're trying to use this pack in Unity, 
you need to make a shader for vector colors, using this code.
Then on the .blend file in Unity, you click extract materials and put them wherever you want.
Apply the vector colors shader you made to all the materials.
Once you put the .blend file in the scene, it should already have the materials applied appropriately.
*/

Shader "Custom/VertexColorSRP" {
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		#pragma target 3.0

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
		ENDCG
	}
		FallBack "Diffuse"
}