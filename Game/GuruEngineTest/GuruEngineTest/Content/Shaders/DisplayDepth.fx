﻿#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D DepthMap;

sampler2D DepthMapSampler = sampler_state
{
	Texture = <DepthMap>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

//float LinearizeDepth(float2 uv)
//{
//	float n = 1.0; // camera z near
//	float f = 100000.0; // camera z far
//	float z = tex2D(DepthMapSampler, uv).x;
//	return (2.0 * n) / (f + n - z * (f - n));
//}


float4 MainPS(VertexShaderOutput input) : COLOR
{
	//float d = LinearizeDepth(input.TextureCoordinates);
	//return float4(d, d, d, 1);

	float z = tex2D(DepthMapSampler, input.TextureCoordinates).x;
	float r = 0;
	float g = 0;
	float b = 0;

	if (z < 1000)
	{
		r = z / 1000.0;
	}
	else if (z < 10000)
	{
		g = z / 10000.0;
	}
	else {
		b = z / 100000.0;
	}
	return float4(r, g, b, 1);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};