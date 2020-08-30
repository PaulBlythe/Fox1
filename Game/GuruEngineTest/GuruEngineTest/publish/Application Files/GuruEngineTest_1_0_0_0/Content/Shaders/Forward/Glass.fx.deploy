#include "ShaderVariables.inc"


float4 tint = half4(0.25,0.25,0.5,0.125);

float3 etas = { 0.80, 0.82, 0.84 };

// wavelength colors
const float4 colors[3] = {
	{ 1, 0, 0, 1 },
	{ 0, 1, 0, 1 },
	{ 0, 0, 1, 1 },
};

struct VS_IN
{
	float4 Position : POSITION;
	float3 Normal : NORMAL;	
};
struct VS_OUT
{
	float4 Position		: POSITION;
	float3 Reflect		: TEXCOORD0;   
	float  Specular		: TEXCOORD1;
	float3 RefractRGB[3]: TEXCOORD2;	
	
};
struct PS_OUT
{
	float4 Color : COLOR;
};

texture environmentMap;
samplerCUBE CubeMap = sampler_state 
{ 
    texture = <environmentMap> ;
};

float4 CubeMapLookup(float3 CubeTexcoord)
{    
    return texCUBE(CubeMap, CubeTexcoord);
}

float3 refract2(float3 I, float3 N, float3 eta)
{
	float IdotN = dot(I, N);
	float k = 1 - eta*eta*(1 - IdotN*IdotN);

	return eta*I - (eta*IdotN + sqrt(k))*N;
}

VS_OUT VS_Glass(VS_IN input)
{
	VS_OUT output = (VS_OUT)0;
	
	output.Position = mul(input.Position,WorldViewProjection);	
	
	float3 Normal = normalize(mul(input.Normal, WorldInverseTranspose));
	float3 PosWorldr  = (mul(input.Position, World));
	float3 ViewDirection = normalize(PosWorldr - ViewVector);
	
	output.Reflect = reflect(ViewDirection, Normal);	
	
	float3 H = normalize(normalize(SunDirection.xyz) + ViewDirection);
	output.Specular = pow(saturate(dot(Normal, H)), 30);

	// transmission
  	bool fail = false;
    for(int i=0; i<3; i++) 
    	output.RefractRGB[i] = refract2(ViewDirection, Normal, etas[i]);
	
	return output;
}
PS_OUT PS_Glass(VS_OUT input)
{
	PS_OUT output = (PS_OUT)0;
	
	float4 refract = float4(0,0,0,0);
	for(int c=0;c<3;c++)
		refract += CubeMapLookup(input.RefractRGB[c]) * colors[c];

	output.Color = lerp(refract,CubeMapLookup(normalize(input.Reflect)),0.5);
	output.Color = saturate(output.Color + (float4(1, 1, 1, 1)*input.Specular));
	return output;
}

technique Glass
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL VS_Glass();
		PixelShader = compile PS_SHADERMODEL PS_Glass();
	}
}