#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

///////////////////////////////////////////////////////////////////////////////////////
// Inputs
///////////////////////////////////////////////////////////////////////////////////////

float sampleRadius;
float distanceScale;
float4x4 Projection;
float3 viewDirection;


///////////////////////////////////////////////////////////////////////////////////////
// Constants
///////////////////////////////////////////////////////////////////////////////////////
float4 samples[8] =
{
	float4(0.355512, 	-0.709318, 	-0.102371,	0.0),
	float4(0.534186, 	0.71511, 	-0.115167,	0.0),
	float4(-0.87866, 	0.157139, 	-0.115167,	0.0),
	float4(0.140679, 	-0.475516, 	-0.0639818,	0.0),
	float4(-0.0796121, 	0.158842, 	-0.677075,	0.0),
	float4(-0.0759516, 	-0.101676, 	-0.483625,	0.0),
	float4(0.12493, 	-0.0223423,	-0.483625,	0.0),
	float4(-0.0720074, 	0.243395, 	-0.967251,	0.0)
};

///////////////////////////////////////////////////////////////////////////////////////
// Textures
///////////////////////////////////////////////////////////////////////////////////////
texture depthMap;
texture normalMap;
texture randomMap;

///////////////////////////////////////////////////////////////////////////////////////
// Samplers
///////////////////////////////////////////////////////////////////////////////////////
sampler depthSampler = sampler_state
{
	Texture = (depthMap);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = POINT;
	MinFilter = POINT;
	Mipfilter = POINT;
};

sampler normalSampler = sampler_state
{
	Texture = (normalMap);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = POINT;
	MinFilter = POINT;
	Mipfilter = POINT;
};

sampler randomSampler = sampler_state
{
	Texture = (randomMap);
	AddressU = WRAP;
	AddressV = WRAP;
	MagFilter = POINT;
	MinFilter = POINT;
	Mipfilter = POINT;
};

///////////////////////////////////////////////////////////////////////////////////////
// Structures
///////////////////////////////////////////////////////////////////////////////////////
struct VertexShaderInput
{
	float3 Position : POSITION0;
	float2 UV		: TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position			: SV_POSITION;
	float2 UV				: TEXCOORD1;
};

///////////////////////////////////////////////////////////////////////////////////////
// Vertex shaders
///////////////////////////////////////////////////////////////////////////////////////
VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = float4(input.Position, 1);
	output.UV = input.UV;

	return output;
}

///////////////////////////////////////////////////////////////////////////////////////
// Pixel shaders
///////////////////////////////////////////////////////////////////////////////////////
float4 MainPS(VertexShaderOutput input) : COLOR
{
	float3 view = normalize(viewDirection);

	float depth = tex2D(depthSampler, input.UV).r;
	float3 se = depth * view;

	float3 randNormal = tex2D(randomSampler, input.UV).rgb;
	randNormal = (2.0 * randNormal) - 1.0;

	//float3 normal = tex2D(normalSampler, input.UV).rgb;
	//normal = (2.0 * normal) - 1.0;

	float finalColor = 0.0f;
	float sampleDepth;

	[unroll]
	for (int i = 0; i < 8; i++)
	{
		float3 ray = reflect(samples[i].xyz, randNormal) * sampleRadius;

		float4 smple = float4(se + ray, 1.0f);
		float4 ss = mul(smple, Projection);

		float2 sampleTexCoord = (0.5f * ss.xy / ss.w) + float2(0.5f, 0.5f);
		sampleDepth = tex2D(depthSampler, sampleTexCoord).r;

		if (sampleDepth > depth)
		{
			finalColor++;
		}	
		else
		{
			//float occlusion = distanceScale * max(depth - sampleDepth, 0.0f);
			//finalColor += 1.0f / (1.0f + occlusion * occlusion );
		}
	}
	return float4(finalColor / 8.0, finalColor / 8.0, finalColor / 8.0, 1.0f);
	

}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};