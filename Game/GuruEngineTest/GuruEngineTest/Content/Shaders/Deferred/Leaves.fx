#include "../Forward/ShaderVariables.inc"

#define MAXBONES 20

texture Texture;

float4x4 Bones[MAXBONES];

float LeafScale = 1.0f;

float3 BillboardRight = float3(1, 0, 0);	// The billboard's right direction in view space
float3 BillboardUp = float3(0, 1, 0);		// The billboard's up direction in view space


sampler TextureSampler = sampler_state
{
	Texture = (Texture);
	MinFilter = Anisotropic;
	MagFilter = Anisotropic;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
	float2 Offset : TEXCOORD1;
	float4 Color : COLOR0;
	int2 BoneIndex : TEXCOORD2;
	float3 Normal : NORMAL;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
	float4 Color : COLOR0;
	float2 Depth : TEXCOORD1;
	float3 Normal : TEXCOORD2;
};

struct PixelShaderOutput
{
	float4 Color		: COLOR0;
	float4 Normal		: COLOR1;
	float  Depth		: COLOR2;
	float4 Material		: COLOR3;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 localPosition = mul(input.Position, Bones[input.BoneIndex.x]);
	float4 viewPosition = mul(localPosition, WorldView);

	viewPosition.xyz += (input.Offset.x * BillboardRight + input.Offset.y * BillboardUp) * LeafScale;

	output.Position = mul(viewPosition, Projection);
	output.Depth.x = output.Position.z;
	output.Depth.y = output.Position.w;

	output.TextureCoordinate = input.TextureCoordinate;

	float3 normal = mul(input.Normal, Bones[input.BoneIndex.x]);
	normal = mul(normal, WorldView);
	output.Normal = normal;
	output.Color = AmbientColour;

	output.Color += SunColour * (0.75 * saturate(dot(normal, -mul(SunDirection, View))) + 0.25);
	output.Color = output.Color * input.Color;
	output.Color.a = 1.0;

	return output;
}

PixelShaderOutput  PixelShaderFunctionApply(VertexShaderOutput input)
{
	PixelShaderOutput output = (PixelShaderOutput)0;
	output.Color = float4(input.Color * tex2D(TextureSampler, input.TextureCoordinate).rgb, tex2Dbias(TextureSampler, float4(input.TextureCoordinate.xy, 1, -1)).a);
	if (output.Color.a < 230.0f / 255.0f)
		discard;
	output.Normal.rgb = 0.5f * (normalize(input.Normal) + 1.0f);
	output.Normal.a = 0;
	output.Material.rgba = 0;
	output.Depth = input.Depth.x / input.Depth.y;
	return output;
}

PixelShaderOutput  PixelShaderFunction(VertexShaderOutput input)
{
	PixelShaderOutput output = (PixelShaderOutput)0;
	output.Color = float4(input.Color * tex2D(TextureSampler, input.TextureCoordinate).rgb, tex2Dbias(TextureSampler, float4(input.TextureCoordinate.xy, 1, -1)).a);
	output.Color.a = 0;
	output.Normal.rgb = 0.5f * (normalize(input.Normal) + 1.0f);
	output.Normal.a = 0;
	output.Material.rgba = 0;
	output.Color.a = 0;
	return output;
}


technique Standard
{
	pass Opaque
	{

		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunctionApply();
		//AlphaBlendEnable = false;
		//AlphaTestEnable = true;
		ZEnable = true;
		ZWriteEnable = true;
		CullMode = None;
	}
	pass BlendedEdges
	{

		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
		//AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		//AlphaTestEnable = true;
		ZEnable = true;
		ZWriteEnable = false;

		CullMode = None;
	}
}
