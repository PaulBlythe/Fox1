#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float3 lightDirection;
float3 Color;
float3 cameraPosition;

float2 halfPixel;

float4x4 InvertViewProjection;
bool isSun = false;
bool isMoon = false;

//texture colorMap;
texture normalMap;
texture depthMap;
texture MaterialMap;

//sampler colorSampler = sampler_state
//{
//	Texture = (colorMap);
//	AddressU = CLAMP;
//	AddressV = CLAMP;
//	MagFilter = LINEAR;
//	MinFilter = LINEAR;
//	Mipfilter = LINEAR;
//};

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

sampler materialSampler = sampler_state
{
	Texture = (MaterialMap);
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

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	output.Position = float4(input.Position, 1);

	//align texture coordinates
	output.TexCoord = input.TexCoord - halfPixel;
	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	//get normal data from the normalMap
	float4 normalData = tex2D(normalSampler,input.TexCoord);
	float4 materialData = tex2D(materialSampler, input.TexCoord);

	//tranform normal back into [-1,1] range
	float3 normal = (2.0f * normalData.xyz) - 1.0f;
	//get specular power, and get it into [0,255] range]
	float specularPower = materialData.y * 255;
	float specularIntensity = materialData.x;

	//read depth
	float depthVal = tex2D(depthSampler, input.TexCoord).r;

	//compute screen-space position
	float4 position;
	position.x = input.TexCoord.x * 2.0f - 1.0f;
	position.y = -(input.TexCoord.y * 2.0f - 1.0f);
	position.z = depthVal;
	position.w = 1.0f;

	//transform to world space
	position = mul(position, InvertViewProjection);
	position /= position.w;

	//surface-to-light vector
	float3 lightVector = -normalize(lightDirection);
	//compute diffuse light
	float NdL = max(0, dot(normal, lightVector));
	float3 diffuseLight = NdL * Color.rgb;
	//reflexion vector
	float3 reflectionVector = -normalize(reflect(lightVector, normal));
	//camera-to-surface vector
	float3 directionToCamera = normalize(cameraPosition - position);
	//compute specular light
	float specularLight = specularIntensity * pow(saturate(dot(reflectionVector, directionToCamera)), specularPower);

	if (isSun)
	{
		diffuseLight *= materialData.w;
		specularLight *=  materialData.w;
	}
	if (isMoon)
	{
		diffuseLight *= materialData.z;
		specularLight *= materialData.z;
	}
	//output the two lights
	return float4(diffuseLight.rgb, specularLight);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
};