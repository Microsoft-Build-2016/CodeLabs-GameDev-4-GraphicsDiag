Shader "Skybox XFade Shader" {
	Properties{
		_Cube("Environment Map", Cube) = "white" {}
		_CubeSpace("Space Map", Cube) = "white" {}
		_XFade("X Fade", Range(0,1.0)) = 0.4

	}

	SubShader{
	Tags{ "Queue" = "Background" }

	Pass{
		ZWrite Off
		Cull Off

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma enable_d3d11_debug_symbols

		samplerCUBE _Cube;
		samplerCUBE _CubeSpace;
		float _XFade;

		struct vertexInput {
			float4 vertex : POSITION;
			float3 texcoord : TEXCOORD0;
		};

		struct vertexOutput {
			float4 vertex : SV_POSITION;
			float3 texcoord : TEXCOORD0;
		};

		vertexOutput vert(vertexInput input)
		{
			vertexOutput output;
			output.vertex = mul(UNITY_MATRIX_MVP, input.vertex);
			output.texcoord = input.texcoord;
			return output;
		}

		fixed4 frag(vertexOutput input) : COLOR
		{ 
			fixed4 earthColor = texCUBE(_Cube, input.texcoord);
			fixed4 spaceColor = texCUBE(_CubeSpace, input.texcoord);
			fixed4 final = lerp(earthColor, spaceColor, _XFade);
			return final;
		}
			ENDCG
		}
	}
}