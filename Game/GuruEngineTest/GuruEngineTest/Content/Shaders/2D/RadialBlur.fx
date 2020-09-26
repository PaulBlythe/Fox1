#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

float invBlurSamples = 0.125f;
float psize = 2.0f / 1080.0f;
float BlurIntensity = 0.5f;

struct VertexShaderOutput
{
	float4 Position				: SV_POSITION;
	float4 Color				: COLOR0;
	float2 TextureCoordinates	: TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 vec = float2(0.5,0.5);
	vec -= input.TextureCoordinates;

	float4 color = float4(0,0,0,0);

	for (int i = 0; i < 8; i++)
	{
		color += tex2D(SpriteTextureSampler, input.TextureCoordinates + vec * i * invBlurSamples * BlurIntensity * 0.5f);
	}
	
	color *= invBlurSamples;

	return float4(color.rgb, 1);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};