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

#define NUMSAMPLES 8

//Projection matrix
float4x4 Projection;
//Corner Fustrum
float3 cornerFustrum;
//Sample Radius
float sampleRadius;
//Distance Scale
float distanceScale;



///////////////////////////////////////////////////////////////////////////////////////
// Constants
///////////////////////////////////////////////////////////////////////////////////////

float4 samples[8] =
{
	float4(0.355512, -0.709318, -0.102371, 0.0),
	float4(0.534186, 0.71511, -0.115167, 0.0),
	float4(-0.87866, 0.157139, -0.115167, 0.0),
	float4(0.140679, -0.475516, -0.0639818, 0.0),
	float4(-0.207641, 0.414286, 0.187755, 0.0),
	float4(-0.277332, -0.371262, 0.187755, 0.0),
	float4(0.63864, -0.114214, 0.262857, 0.0),
	float4(-0.184051, 0.622119, 0.262857, 0.0)
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
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	Mipfilter = LINEAR;
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
	float2 UV				: TEXCOORD0;
	float3 ViewDirection	: TEXCOORD1;
};

///////////////////////////////////////////////////////////////////////////////////////
// Vertex shaders
///////////////////////////////////////////////////////////////////////////////////////
VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = float4(input.Position, 1);
	output.UV = input.UV;

	//Set up ViewDirection vector
	output.ViewDirection = float3(-cornerFustrum.x * input.Position.x, cornerFustrum.y * input.Position.y, cornerFustrum.z);

	return output;
}

float3 decode(float3 enc)
{
	return (2.0f * enc.xyz - 1.0f);
}

///////////////////////////////////////////////////////////////////////////////////////
// Pixel shaders
///////////////////////////////////////////////////////////////////////////////////////
float4 MainPS(VertexShaderOutput input) : COLOR
{
	//Normalize the input ViewDirection
	float3 ViewDirection = normalize(input.ViewDirection);

	//Sample the depth
	float depth = tex2D(depthSampler, input.UV).r;

	//Calculate the depth at this pixel along the view direction
	float3 se = depth * ViewDirection;

	//Sample a random normal vector
	float3 randNormal = decode(tex2D(randomSampler, input.UV * 200.0f).xyz);

	//Sample the Normal for this pixel
	float3 normal = decode(tex2D(normalSampler, input.UV).xyz);

	//No assymetry in HLSL, workaround
	float finalColor = 0.0f;

	//SSAO loop
	for (int i = 0; i < NUMSAMPLES; i++)
	{
		//Calculate the Reflection Ray
		float3 ray = reflect(samples[i].xyz, randNormal) * sampleRadius;

		//Test the Reflection Ray against the surface normal
		if (dot(ray, normal) < 0) ray += normal * sampleRadius;

		//Calculate the Sample vector
		float4 sample = float4(se + ray, 1.0f);

		//Project the Sample vector into ScreenSpace
		float4 ss = mul(sample, Projection);

		//Convert SS into UV space
		float2 sampleTexCoord = (0.5f * ss.xy / ss.w) + float2(0.5f, 0.5f);

		//Sample the Depth along the ray
		float sampleDepth = tex2D(depthSampler, sampleTexCoord).r;

		//Check the sampled depth value
		if (sampleDepth > depth)
		{
			//Non-Occluded sample
			finalColor++;
		}
		else
		{
			//Calculate Occlusion
			float occlusion = distanceScale * max(depth - sampleDepth, 0.0f);
			//Accumulate to finalColor
			finalColor += 1.0f / (1.0f + occlusion * occlusion * 0.1);
		}
	}

	//Output the Average of finalColor
	return float4(finalColor / NUMSAMPLES, finalColor / NUMSAMPLES, finalColor / NUMSAMPLES, 1.0f);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};