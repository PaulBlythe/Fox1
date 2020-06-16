#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

texture WaterTexture;
sampler2D textureSampler = sampler_state
{
	Texture = (WaterTexture);
};


texture FoamTexture;
sampler2D foamSampler = sampler_state
{
	Texture = (FoamTexture);
};

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x3 WorldInverseTranspose;

struct VertexShaderInput
{
	float3 Position				: POSITION0;
	float3 Normal				: NORMAL0;
	float2 TextureCoordinate	: TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position			: POSITION0;
	float  fog_factor		: TEXCOORD0;
	float2 tex_coord		: TEXCOORD1;
	float2 Depth			: TEXCOORD2;
	float3 normal_vector	: TEXCOORD3;
};

struct PixelShaderOutput
{
	float4 Color		 : COLOR0;
	float4 Normal		 : COLOR1;
	float4 Depth		 : COLOR2;
	float4 Material		 : COLOR3;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	float4 worldPosition = mul(float4(input.Position,1), World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.fog_factor =  min(-viewPosition.z / 10000.0, 1.0);

	float3 normal1 = normalize(input.Normal);

	
	output.normal_vector = mul(WorldInverseTranspose, float4(normal1, 0.0)).xyz;
	output.tex_coord = input.TextureCoordinate;
	output.Depth.x = output.Position.z;
	output.Depth.y = output.Position.w;

	return output;
}

PixelShaderOutput MainPS(VertexShaderOutput input) 
{
	PixelShaderOutput output;

	float3 nn = normalize(input.normal_vector);
	float3 up = float3(0, -1, 0);
	float foam =  abs(dot(up, nn));

	float4 c = tex2D(textureSampler, input.tex_coord);
	float4 f = tex2D(foamSampler, input.tex_coord);

	c = lerp(c, f, 1.0f - foam);
	float4 fragColor = c * (1.0 - input.fog_factor) + float4(0.25, 0.75, 0.65, 1.0) * (input.fog_factor);


	output.Color.xyz = fragColor.xyz;
	output.Color.a = 1.0f;
	output.Depth = input.Depth.x / input.Depth.y;
	output.Normal.rgb = 0.5f * (nn + 1.0f);
	output.Normal.a = 1.0f;

	output.Material = float4(0.5f,1.0f,0.0f,1.0f);

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