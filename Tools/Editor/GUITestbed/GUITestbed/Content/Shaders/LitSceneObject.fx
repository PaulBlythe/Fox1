#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;

texture LightTexture;
sampler2D LightSampler = sampler_state
{
	Texture = (LightTexture);
	MinFilter = POINT;
	MagFilter = POINT;
};

texture ModelTexture;
sampler2D textureSampler = sampler_state
{
	Texture = (ModelTexture);
};


struct VertexShaderInput
{
	float4 Position				: POSITION0;
	float3 Normal				: NORMAL0;
	float2 TextureCoordinate	: TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Pos		: POSITION;
	float2 Tex      : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output;

	output.Tex = input.TextureCoordinate;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Pos = mul(viewPosition, Projection);

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 textureColor = tex2D(textureSampler, input.Tex);

	float light = 0.2f;
	light = saturate(light + tex2D(LightSampler, input.Tex).x);

	float4 result;
	result.xyz = light * textureColor.xyz;
	result.a = textureColor.a;

	return result;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};