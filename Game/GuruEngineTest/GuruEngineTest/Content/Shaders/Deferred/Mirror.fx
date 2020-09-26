#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
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
	float4 Position		: POSITION0;
	float3 Reflection 	: TEXCOORD0;
	float3 Normal		: TEXCOORD1;
	float2 Depth		: TEXCOORD2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, WorldViewProjection);

	float4 VertexPosition = mul(input.Position, World);
	float3 ViewDirection = ViewVector - VertexPosition;

	float3 Normal = normalize(mul(input.Normal, WorldInverseTranspose));
	output.Reflection = reflect(-normalize(ViewDirection), normalize(Normal));
	output.Normal = Normal;
	output.Depth.x = output.Position.z;
	output.Depth.y = output.Position.w;
	return output;
}


struct PixelShaderOutput
{
	float4 Color		: COLOR0;
	float4 Normal		: COLOR1;
	float  Depth		: COLOR2;
	float4 Material		: COLOR3;
};

PixelShaderOutput MainPS(VertexShaderOutput input)
{
	PixelShaderOutput output;

	output.Color = texCUBE(envMapSampler, normalize(input.Reflection));
	output.Color.a = 0.2f;
	output.Normal.rgb = 0.5f * (normalize(input.Normal) + 1.0f);		// transform normal domain
	output.Normal.a = 1.0f;
	output.Depth = input.Depth.x / input.Depth.y;
	output.Material.x = 1;
	output.Material.y = 2;
	output.Material.z = 1;
	output.Material.w = 1;
	return output;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};