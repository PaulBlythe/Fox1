#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float zFar;
float zNear;

texture colorMap;
texture depthmap;

sampler depthSampler = sampler_state
{
	Texture = (depthmap);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	Mipfilter = LINEAR;
};

sampler colorSampler = sampler_state
{
	Texture = (colorMap);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	Mipfilter = LINEAR;
};

struct VertexShaderInput
{
	float3 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

float2 halfPixel;

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output;
	output.Position = float4(input.Position, 1);
	output.TexCoord = input.TexCoord - halfPixel;
	return output;
}

struct PixelShaderOutput
{
	float4 Color		: COLOR0;
	float  Depth		: DEPTH;
};

PixelShaderOutput MainPS(VertexShaderOutput input) 
{
	PixelShaderOutput output;

	output.Color = tex2D(colorSampler, input.TexCoord);
	float d = tex2D(depthSampler, input.TexCoord).r;

	//float nonLinearDepth = (zFar + zNear - 2.0 * zNear * zFar / d) / (zFar - zNear);
	//nonLinearDepth = (nonLinearDepth + 1.0) / 2.0;

	
	output.Depth = d;
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