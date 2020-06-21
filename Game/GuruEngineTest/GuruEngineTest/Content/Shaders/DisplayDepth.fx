#if OPENGL
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

float LinearizeDepth(float2 uv)
{
	float n = 1.0; // camera z near
	float f = 10000.0; // camera z far
	float z = tex2D(DepthMapSampler, uv).x;
	return (2.0 * n) / (f + n - z * (f - n));
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float d = LinearizeDepth(input.TextureCoordinates);
	return float4(d, d, d, 1);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};