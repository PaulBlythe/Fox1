#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

/*********************************************************************************************************************/
/* Matrices																											 */
/*********************************************************************************************************************/
float4x4 World;
float4x4 View;
float4x4 Projection;
float4x3 WorldInverseTranspose;
float4x4 ViewInverse;
float4x4 WorldViewProjection;
float4x4 WorldView;

/*********************************************************************************************************************/
/* Sun Light																										 */
/*********************************************************************************************************************/
float4 AmbientColour = float4(1, 1, 1, 1);
float4 SunDirection = float4(-3, 0, 6, 1);
float4 SunColour = float4(1, 1, 1, 1);
float LightMask = 0;

/*********************************************************************************************************************/
/* Moon Light																										 */
/*********************************************************************************************************************/
float MoonLit = 0;
float4 MoonDirection = float4(0, 0, 0, 1);

/*********************************************************************************************************************/
/* Material																											 */
/*********************************************************************************************************************/
float4 MaterialColour = float4(1, 1, 1, 1);
float DiffuseIntensity = 0.9;
float Shininess = 200;
float SpecularIntensity = 1;
float AlphaCut = 1.0;
bool TestAlpha = false;
float4 SpecularColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.8;

/*********************************************************************************************************************/
/* Vectors																											 */
/*********************************************************************************************************************/
float3 ViewVector = float3(1, 0, 0);


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

	float3 Ia = AmbientIntensity * AmbientColour;
	float3 Id = DiffuseIntensity * saturate(dot(Normal, LightDir));
	float3 Is = SpecularIntensity * pow(saturate(dot(Normal, H)), Shininess);

	result.xyz = saturate((saturate(Id + Ia) * textureColor.xyz) + (Is * SpecularColor.xyz));
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

