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

float2 halfPixel;


///////////////////////////////////////////////////////////////////////////////////////
// Constants
///////////////////////////////////////////////////////////////////////////////////////

const float total_strength = 1.0;
const float base = 0.0;
const float area = 0.0075;
const float falloff = 0.000001;
const float radius = 0.00002;

///////////////////////////////////////////////////////////////////////////////////////
// Textures
///////////////////////////////////////////////////////////////////////////////////////
texture depthMap;
texture randomMap;
texture normalMap;
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


sampler RandomTextureSampler = sampler_state
{
	Texture = (randomMap);
	AddressU = WRAP;
	AddressV = WRAP;
	MagFilter = POINT;
	MinFilter = POINT;
	Mipfilter = POINT;
};


sampler NormalSampler = sampler_state
{
	Texture = (normalMap);
	AddressU = CLAMP;
	AddressV = CLAMP;
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
// Subroutines
///////////////////////////////////////////////////////////////////////////////////////
float3 normal_from_depth(float2 texcoords) 
{
	float3 normal = tex2D(NormalSampler, texcoords);
	normal = (2.0f * normal) - 1.0f;
	return normalize(normal);
}

float doTest(float3 delta, float radius_depth, float3 random, float3 position, float3 normal, float depth)
{
	float3 ray = radius_depth * reflect(delta, random);
	float3 hemi_ray = position + sign(dot(ray, normal)) * ray;
	float occ_depth = tex2D(depthSampler, saturate(hemi_ray.xy)).r;
	float difference = (depth - occ_depth);
	return step(falloff, difference) * (1.0 - smoothstep(falloff, area, difference));
}

///////////////////////////////////////////////////////////////////////////////////////
// Pixel shaders
///////////////////////////////////////////////////////////////////////////////////////
float4 MainPS(VertexShaderOutput input) : COLOR
{
	float3 random = normalize(tex2D(RandomTextureSampler, input.UV * 3.9f).rgb);
	random = 2.0 *(random - 0.5f);
	float depth = tex2D(depthSampler, input.UV).r;
	float3 position = float3(input.UV, depth);
	float3 normal = normal_from_depth(input.UV);

	float radius_depth = radius / depth;
	float occlusion = 0.0;

	occlusion += doTest(float3(0.5381, 0.1856,-0.4319), radius_depth,random,position,normal,depth);
	occlusion += doTest(float3(0.1379, 0.2486, 0.4430), radius_depth,random,position,normal,depth);
	occlusion += doTest(float3(0.3371, 0.5679,-0.0057), radius_depth,random,position,normal,depth);
	occlusion += doTest(float3(-0.6999,-0.0451,-0.0019),radius_depth,random,position,normal,depth);
	occlusion += doTest(float3(0.0689,-0.1598,-0.8547), radius_depth,random,position,normal,depth);
	occlusion += doTest(float3(0.0560, 0.0069,-0.1843), radius_depth,random,position,normal,depth);
	occlusion += doTest(float3(-0.0146, 0.1402, 0.0762),radius_depth,random,position,normal,depth);
	occlusion += doTest(float3(0.0100,-0.1924,-0.0344), radius_depth,random,position,normal,depth);
	occlusion += doTest(float3(-0.3577,-0.5301,-0.4358),radius_depth,random,position,normal,depth);
	occlusion += doTest(float3(-0.3169, 0.1063, 0.0158),radius_depth,random,position,normal,depth);
	occlusion += doTest(float3(0.0103,-0.5869, 0.0046), radius_depth,random,position,normal,depth);
	occlusion += doTest(float3(-0.0897,-0.4940, 0.3287),radius_depth,random,position,normal,depth);
	occlusion += doTest(float3(0.7119,-0.0154,-0.0918), radius_depth,random,position,normal,depth);
	occlusion += doTest(float3(-0.0533, 0.0596,-0.5411),radius_depth,random,position,normal,depth);
	occlusion += doTest(float3(0.0352,-0.0631, 0.5460), radius_depth,random,position,normal,depth);
	occlusion += doTest(float3(-0.4776, 0.2847,-0.0271),radius_depth,random,position,normal,depth);

	float ao = total_strength * occlusion * (1.0 / 16.0f);
	float res = saturate(ao + base);
	return float4(res, res, res, 1.0);

}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};