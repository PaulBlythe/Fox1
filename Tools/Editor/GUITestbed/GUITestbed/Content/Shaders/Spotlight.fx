#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

float4x4	World;
float4x4	WorldInverseTranspose;
float4x4    LightViewProj;

float3		EyePosition;
float3		LightPosition;
float3		LightDirection;
float		LightConeAngle;

float		SpecularPower = 10;
float		SpecularIntensity = 0.5f;
float		SpotCosOuterCone;
float		DepthBias = 0.004f;


texture ShadowTexture;
sampler2D ShadowSampler = sampler_state
{
	Texture = (ShadowTexture);
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	AddressU = CLAMP;
	AddressV = CLAMP;

};

struct VertexShaderInput
{
	float4 Position				: POSITION0;
	float3 Normal				: NORMAL0;
	float2 TextureCoordinate	: TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position				: SV_POSITION;
	float3 WorldPos				: TEXCOORD0;
	float3 Norm					: TEXCOORD1;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	float2 outp = input.TextureCoordinate.xy * float2(2.0, -2.0) + float2(-1.0, 1.0);
	
	output.Position = float4(outp.x, outp.y, 0.0f, 1);
	output.WorldPos = mul(input.Position, World);
	output.Norm = mul(input.Normal, WorldInverseTranspose).xyz;
	return output;
}

// Calculates the shadow term using PCF
float CalcShadowTermPCF(float light_space_depth, float ndotl, float2 shadow_coord)
{
	float variableBias = clamp(0.001 * tan(acos(ndotl)), 0, DepthBias);
	light_space_depth -= variableBias;

	float size = 1.0 / 1024.0;
	float samples = 0;

	if (light_space_depth < tex2D(ShadowSampler, shadow_coord).r)
	{
		samples = 0.25;
	}
	if (light_space_depth < tex2D(ShadowSampler, shadow_coord + float2(size, 0)).r)
	{
		samples += 0.25;
	}
	if (light_space_depth < tex2D(ShadowSampler, shadow_coord + float2(0, size)).r)
	{
		samples += 0.25;
	}
	if (light_space_depth < tex2D(ShadowSampler, shadow_coord + float2(size, size)).r)
	{
		samples += 0.25;
	}

	return samples;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float3 ToLight = LightPosition - input.WorldPos;
	float3 ToEye = EyePosition - input.WorldPos;
	float DistToLight = length(ToLight);

	// Phong diffuse
	ToLight /= DistToLight; // Normalize
	float NDotL = saturate(dot(ToLight, input.Norm));
	float3 finalColor = float3(1,1,1) * NDotL;

	// Blinn specular
	ToEye = normalize(ToEye);
	float3 HalfWay = normalize(ToEye + ToLight);
	float NDotH = saturate(dot(HalfWay, input.Norm));
	finalColor += pow(NDotH, SpecularPower) * SpecularIntensity;

	float SpotCosConeAttRange = LightConeAngle - SpotCosOuterCone;
	float conAtt = 0;
	// Cone attenuation
	float cosAng = dot(LightDirection, ToLight);
	float shadowContribution = 0.0f;

	float4 lightingPosition = mul(input.WorldPos, LightViewProj);
	float ourdepth = lightingPosition.z;
	lightingPosition.xy /= lightingPosition.w;

	float2 ShadowTexCoord = 0.5f * (float2(lightingPosition.x, -lightingPosition.y) + 1);
	shadowContribution = CalcShadowTermPCF(ourdepth, NDotL, ShadowTexCoord);

	if (cosAng > LightConeAngle)
	{
		conAtt = shadowContribution;
	}
	else if (cosAng > SpotCosOuterCone)
	{
		conAtt = lerp(0, shadowContribution, (cosAng - SpotCosOuterCone) / SpotCosConeAttRange);
	}
	else {
		discard;
	}

	finalColor *= conAtt;
	return float4(finalColor,1);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};