#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#include "ShaderVariables.inc"

float LightMask = 1.0f;

texture Texture1;
sampler2D textureSampler = sampler_state
{
	Texture = (Texture1);
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
	float3 Light	: TEXCOORD1;
	float3 Norm		: TEXCOORD2;
	float3 View		: TEXCOORD3;
};


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	output.Light = normalize(-SunDirection.xyz);
	output.Tex = input.TextureCoordinate;

	float4 pos = input.Position;
	float4 worldPosition = mul(pos, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Pos = mul(viewPosition, Projection);

	output.View = normalize(ViewVector - worldPosition.xyz);
	output.Norm = normalize(mul(input.Normal, WorldInverseTranspose).xyz);

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 result;
	float4 textureColor = tex2D(textureSampler, input.Tex);

	float3 Normal = normalize(input.Norm);
	float3 LightDir = normalize(input.Light);
	float3 ViewDir = normalize(input.View);
	float3 H = normalize(LightDir + ViewDir);

	float3 Ia = LightMask * 0.2 * AmbientColour;
	float3 Id = LightMask * 0.8 * saturate(dot(Normal, LightDir));
	float3 Is = LightMask * 0.5 * pow(saturate(dot(Normal, H)), Shininess);

	result.xyz = saturate((saturate(Id + Ia) * textureColor.xyz) + (Is * SpecularColor.xyz));
	result.a = textureColor.a;
	return result;
}

technique Textured
{
	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
};