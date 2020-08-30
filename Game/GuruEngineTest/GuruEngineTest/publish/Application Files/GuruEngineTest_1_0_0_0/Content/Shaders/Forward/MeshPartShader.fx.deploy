#include "ShaderVariables.inc"
 
float Damage = 0;
bool Blend = false;


texture ModelTexture;
sampler2D textureSampler = sampler_state 
{
    Texture = (ModelTexture);
};
 
struct VertexShaderInput
{
    float4 Position				: POSITION0;
    float3 Normal				: NORMAL0;
    float2 TextureCoordinate	: TEXCOORD0;
};
 
struct VertexShaderOutput
{
    float4 Pos		: POSITION;
	float2 Tex      : TEXCOORD0;
	float3 Light	: TEXCOORD1;
	float3 Norm		: TEXCOORD2;
	float3 View		: TEXCOORD3;
};

 
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	output.Light = normalize(-SunDirection.xyz);
	output.Tex = input.TextureCoordinate;

	float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);	
    output.Pos = mul(viewPosition, Projection);
 
	output.View = normalize(ViewVector - worldPosition.xyz);
	output.Norm = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	
    return output;
}
 
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 result;
	float4 textureColor = tex2D(textureSampler, input.Tex);

	if (TestAlpha)
	{
		if (textureColor.a < AlphaCut)
			discard;	
	}
	else {
		if (textureColor.a >= Damage)
			textureColor.a = 1;
		else
			discard;
	}

	if (Blend)
	{
	}
	else {
		textureColor.a = 1;
	}

	textureColor.xyz *= MaterialColour.xyz;

	float3 Normal = normalize(input.Norm);
	float3 LightDir = normalize(input.Light);
	float3 ViewDir = normalize(input.View);
	float3 H = normalize(LightDir + ViewDir);

	float3 Ia = LightMask * AmbientIntensity * AmbientColour;
	float3 Id = LightMask * DiffuseIntensity * saturate(dot(Normal, LightDir));
	float3 Is = LightMask * SpecularIntensity * pow(saturate(dot(Normal, H)), Shininess);

	result.xyz = saturate( (saturate(Id+Ia) * textureColor.xyz) + (Is * SpecularColor.xyz));
	result.a = textureColor.a;

	return result;
}
 
technique Textured
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}