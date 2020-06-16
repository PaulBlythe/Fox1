#include "ShaderVariables.inc"

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
	float4 Pos					: POSITION;
	float2 Tex					: TEXCOORD0;
	float3 Light				: TEXCOORD1;
	float3 Norm					: TEXCOORD2;
};

float3 LightDirection = float3(1,0,0);
float sun_height = 0.2f;

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output;

	output.Light = normalize(mul(LightDirection,World));
	output.Tex = input.TextureCoordinate;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Pos = mul(viewPosition, Projection);

	output.Norm = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 textureColor = tex2D(textureSampler, input.Tex);

	float3 Normal = normalize(input.Norm);
	float3 LightDir = normalize(input.Light);

	float3 Id = saturate(0.2f + dot(Normal, LightDir));

	float4 result;

	result.xyz = saturate(Id * textureColor.xyz);

	if (sun_height > 0.04f)
		result.a = 1;
	else
		result.a = length(result.xyz);
	

	return result;
}

technique Moon
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};