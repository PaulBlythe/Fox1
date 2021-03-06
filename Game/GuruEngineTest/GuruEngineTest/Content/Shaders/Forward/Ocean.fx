﻿#include "ShaderVariables.inc"



bool sparkle = false;
float time = 0.6;

//////////////// TEXTURES /////////////////// 

texture Texture1;
texture environmentMap;

sampler2D normalMapSamplerHALO  = sampler_state
{
	Texture = <Texture1>;
	// this is a trick from Halo - use point sampling for sparkles 
	MagFilter = Linear;
	MinFilter = Point;
	MipFilter = None;
	AddressU = Wrap;
	AddressV = Wrap;
};
sampler2D normalMapSampler = sampler_state
{
	Texture = <Texture1>;
	MagFilter = Linear;
	MinFilter = Linear;
	MipFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

samplerCUBE envMapSampler = sampler_state
{
	Texture = <environmentMap>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
	AddressU = Mirror;
	AddressV = Mirror;
};

///////// TWEAKABLE PARAMETERS ////////////////// 

float bumpHeight = 0.1;
float2 textureScale = float2(0.2, 0.2);
float2 bumpSpeed = float2(-0.05, 0.0);
float fresnelBias = 0.1;
float fresnelPower = 4.0;
float4 deepColor = float4( 0.0f, 0.0f, 0.3f, 1.0f );
float4 shallowColor = float4( 0.2f, 0.2f, 0.6f, 1.0f );
float4 reflectionColor = float4(1.0f, 1.0f, 1.0f, 1.0f );

float reflectionAmount = 0.45f;
float waterAmount = 0.55f;
float waveAmp  = 2.2;
float2 waveFreq = float2(0.3,-0.3);

float FogStart = 1000.0f;
float FogEnd = 60000.0f;

//////////// CONNECTOR STRUCTS ////////////////// 

struct AppData {
	float4 Position : POSITION;   // in object space 
	float2 TexCoord : TEXCOORD0;
};

struct VertexOutput {
	float4 Position  : SV_Position;  // in clip space 
	float2 TexCoord  : TEXCOORD0;
	float3 Normal    : TEXCOORD1;
	float  Fog       : TEXCOORD2;
	float3 eyeVector : TEXCOORD3;
	float  DepthVS   : TEXCOORD4;
	float3 PositionWS : POSITIONWS;
};
struct PS_input {
	float4 Position  : SV_Position;  // in clip space 
	float2 TexCoord  : TEXCOORD0;
	float3 Normal    : TEXCOORD1;
	float  Fog : TEXCOORD2;
	float3 eyeVector : TEXCOORD3;
	float  DepthVS : TEXCOORD4;
	float3 PositionWS : POSITIONWS;
};



///////// SHADER FUNCTIONS /////////////// 

VertexOutput BumpReflectWaveVS(AppData IN)
{
	VertexOutput OUT;

	float4 P = mul(IN.Position, World);

	//float2 samplepos = textureScale * (P.xz + time * waveFreq);
	float2 samplepos = textureScale * ((P.xz*100.0) + time * waveFreq);
	float4 norm = tex2Dlod(normalMapSampler, float4(samplepos,0,1));
	norm = (2 * norm) - 1;

	P = IN.Position;
	P.y += norm.y * waveAmp;

	OUT.Position = mul(P, WorldViewProjection);
	OUT.Normal = norm.xyz;
	OUT.DepthVS = OUT.Position.w;
	
	// pass texture coordinates for fetching the normal map 
	OUT.TexCoord.xy = IN.TexCoord*textureScale;

	// compute the eye vector (going from shaded point to eye) in cube space 
	float4 worldPos = mul(IN.Position, World);
	OUT.PositionWS = worldPos.xyz;

	OUT.eyeVector = normalize(worldPos - ViewVector);

	float d = length(worldPos - ViewVector);
	OUT.Fog = clamp((d - FogStart) / (FogEnd - FogStart), 0, 1);


	return OUT;
}


// Pixel Shaders 


float4 OceanMain(PS_input IN) : COLOR
{
	float2 samplepos = textureScale * ((IN.TexCoord * 100.0) + time * waveFreq);
	float4 norm = tex2D(normalMapSampler, samplepos);
	norm = (2 * norm) - 1;
	float3 N = mul(norm.xyz , WorldInverseTranspose);
	N = normalize(N);

	float3 LightDir = -normalize(SunDirection);
	float3 ViewDir = normalize(IN.eyeVector);
	float3 Ia = AmbientColour * 0.25f;
	float3 Id = saturate(dot(LightDir,N.xyz));

	
	// reflection 
	float3 R = normalize(-reflect(LightDir, N));	
	R.y = clamp(R.y, 0, 1);
	float4 reflection = texCUBE(envMapSampler,R);
	float4 dcol = shallowColor;

	float4 waterColor = float4(saturate((Id * dcol)),1);


	float specular = dot(R, -ViewDir);
	float Is = 0;
	if (specular > 0)
	{
		Is = pow(specular, 8);

	}
	uint2 screenPos = uint2(IN.Position.xy);
	
	float4 res = saturate(( reflection) + (Is * reflection));

	//float4 res = saturate(waterColor * waterAmount + reflection * reflectionAmount + Is * reflection);
	res.a = 1;
	return res;
 
}

//////////////////// TECHNIQUE //////////////// 

technique Main 
{
	pass p0 
	{
		VertexShader = compile vs_5_0 BumpReflectWaveVS();
		PixelShader = compile ps_5_0 OceanMain();
	}
}
