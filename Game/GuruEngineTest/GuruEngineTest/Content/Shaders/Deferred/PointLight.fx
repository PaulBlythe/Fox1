﻿#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;

float2 halfPixel;

//color of the light 
float3 Color;

//position of the camera, for specular light
float3 cameraPosition;

//this is used to compute the world-position
float4x4 InvertViewProjection;

//this is the position of the light
float3 lightPosition;

//how far does this light reach
float lightRadius;

//control the brightness of the light
float lightIntensity = 0.0f;

// diffuse color, and specularIntensity in the alpha channel
//texture colorMap;

// normals, and specularPower in the alpha channel
texture normalMap;

//depth
texture depthMap;

texture material;


//sampler colorSampler = sampler_state
//{
//	Texture = (colorMap);
//	AddressU = CLAMP;
//	AddressV = CLAMP;
//	MagFilter = LINEAR;
//	MinFilter = LINEAR;
//	Mipfilter = LINEAR;
//};
//
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
	Texture = (material);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = POINT;
	MinFilter = POINT;
	Mipfilter = POINT;
};


struct VertexShaderInput
{
	float3 Position : POSITION0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 ScreenPosition : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(float4(input.Position, 1), World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.ScreenPosition = output.Position;
	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	//obtain screen position
	input.ScreenPosition.xy /= input.ScreenPosition.w;

	//obtain textureCoordinates corresponding to the current pixel the screen coordinates are in [-1,1]*[1,-1]
	//the texture coordinates need to be in [0,1]*[0,1]
	float2 texCoord = 0.5f * (float2(input.ScreenPosition.x,-input.ScreenPosition.y) + 1);

	//align texels to pixels
	texCoord -= halfPixel;

	//get normal data from the normalMap
	float4 normalData = tex2D(normalSampler,texCoord);
	float4 materialData = tex2D(materialSampler, texCoord);

	//tranform normal back into [-1,1] range
	float3 normal = 2.0f * normalData.xyz - 1.0f;
	//get specular power
	float specularPower = materialData.y * 255;
	//get specular intensity from the colorMap
	float specularIntensity = materialData.x;
	//read depth
	float depthVal = tex2D(depthSampler,texCoord).r;

	//compute screen-space position
	float4 position;
	position.xy = input.ScreenPosition.xy;
	position.z = depthVal;
	position.w = 1.0f;

	//transform to world space
	position = mul(position, InvertViewProjection);
	position /= position.w;

	//surface-to-light vector
	float3 lightVector = lightPosition - position;

	//compute attenuation based on distance - linear attenuation
	float attenuation =  max(1.0f - (length(lightVector) / lightRadius),0);


	//normalize light vector
	lightVector = normalize(lightVector);

	//compute diffuse light
	float NdL = max(0,dot(normal,lightVector));
	float3 diffuseLight = NdL * Color.rgb;

	//reflection vector
	float3 reflectionVector = normalize(reflect(-lightVector, normal));
	//camera-to-surface vector
	float3 directionToCamera = normalize(cameraPosition - position);
	//compute specular light
	float specularLight = specularIntensity * pow(saturate(dot(reflectionVector, directionToCamera)), specularPower);

	//take into account attenuation and lightIntensity.
	return attenuation * lightIntensity * float4(diffuseLight.rgb, specularLight);

}


technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
};