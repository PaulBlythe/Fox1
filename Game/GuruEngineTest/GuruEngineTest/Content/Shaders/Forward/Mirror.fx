#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x3 WorldInverseTranspose;
float4x4 WorldViewProjection;
float4x4 World;
float3	 ViewVector = float3(1, 0, 0);

texture environmentMap;

samplerCUBE envMapSampler = sampler_state
{
	Texture = <environmentMap>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
	AddressU = Mirror;
	AddressV = Mirror;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Normal	: NORMAL0;
	float2 Tex		: TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position		: SV_POSITION;
	float3 Reflection 	: TEXCOORD0;

};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, WorldViewProjection);

	float4 VertexPosition = mul(input.Position, World);
	float3 ViewDirection = ViewVector - VertexPosition;

	float3 Normal = normalize(mul(input.Normal, WorldInverseTranspose));
	output.Reflection = reflect(-normalize(ViewDirection), normalize(Normal));
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return texCUBE(envMapSampler, normalize(input.Reflection));
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};