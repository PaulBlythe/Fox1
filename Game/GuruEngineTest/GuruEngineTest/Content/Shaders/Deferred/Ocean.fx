#include "../Forward/ShaderVariables.inc"

bool sparkle = false;
float time = 0.6;

//////////////// TEXTURES /////////////////// 

texture Texture1;
texture environmentMap;

sampler2D normalMapSamplerHALO = sampler_state
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


struct PixelShaderOutput
{
	float4 Color		 : COLOR0;
	float4 Normal		 : COLOR1;
	float  Depth		 : COLOR2;
	float4 Material		 : COLOR3;
};

///////// TWEAKABLE PARAMETERS ////////////////// 

float bumpHeight = 0.1;
float2 textureScale = float2(18.0, 14.0);
float2 bumpSpeed = float2(-0.05, 0.0);
float fresnelBias = 0.1;
float fresnelPower = 4.0;
float4 deepColor = float4(0.0f, 0.0f, 0.3f, 1.0f);
float4 shallowColor = float4(0.0f, 0.1f, 0.8f, 1.0f);
float4 reflectionColor = float4(0.7f, 0.7f, 1.0f, 1.0f);

float reflectionAmount = 1.00f;
float waterAmount = 0.0000045f;
float waveAmp = 2.2;
float2 waveFreq = float2(0.0003, -0.0003);

float FogStart = 10000;
float FogEnd = 12000;

//////////// CONNECTOR STRUCTS ////////////////// 

struct AppData {
	float4 Position : POSITION;   // in object space 
	float2 TexCoord : TEXCOORD0;
};

struct VertexOutput {
	float4 Position		: SV_Position;  // in clip space 
	float2 TexCoord		: TEXCOORD0;
	float3 Normal		: TEXCOORD1;
	float  Fog			: TEXCOORD2;
	float3 eyeVector	: TEXCOORD3;
	float3  DepthVS		: TEXCOORD4;
	float3 PositionWS	: POSITIONWS;
};



///////// SHADER FUNCTIONS /////////////// 

VertexOutput BumpReflectWaveVS(AppData IN)
{
	VertexOutput OUT;

	float4 P = mul(IN.Position, World);

	float2 samplepos = textureScale * (P.xz + time * waveFreq);
	float4 norm = tex2Dlod(normalMapSampler, float4(samplepos, 0, 1));
	norm = (2 * norm) - 1;

	P = mul(P, View);
	OUT.DepthVS = P.xyz;

	P = IN.Position;
	P.y += norm.y * waveAmp;

	OUT.Position = mul(P, WorldViewProjection);
	OUT.Normal = norm.xyz;
	//OUT.DepthVS.x = OUT.Position.z;
	//OUT.DepthVS.y = OUT.Position.w;
	


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


PixelShaderOutput OceanMain(VertexOutput IN) : COLOR
{
	PixelShaderOutput output;

	float3 N = normalize(IN.Normal);
	N = mul(N , WorldInverseTranspose);
	N = normalize(N);

	float3 LightDir = -normalize(SunDirection.xyz);

	// reflection 
	float3 R = normalize(-reflect(LightDir, N));
	
	float4 reflection = texCUBE(envMapSampler,R);
	float4 dcol = shallowColor;

	float4 res = saturate(dcol * waterAmount + reflection * reflectionAmount);
	res.a = 1;


	output.Color.xyz = reflection.xyz;
	output.Color.a = 0.0f;
	//output.Depth = (IN.DepthVS.x / IN.DepthVS.y);
	output.Depth = length(IN.DepthVS);
	output.Normal.rgb = 0.5f * (N + 1.0f);
	output.Normal.a = 1.0f;
	output.Material = float4(0.75f, 0.25f, 1.0f, 1.0f);

	return output;

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
