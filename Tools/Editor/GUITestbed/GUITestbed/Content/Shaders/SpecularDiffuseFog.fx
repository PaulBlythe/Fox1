#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 Projection;
float4x3 WorldInverseTranspose;
float4x4 World;
float4x4 View;

float4 SpecularColor;
float3 SunDirection;
float3 ViewVector;

float FogStart;
float FogEnd;
texture Texture;
float3 AmbientColour;
float3 FogColour;
float Shininess;


sampler texSampler = sampler_state
{
	Texture = <Texture>;
	MinFilter = Linear;
	MipFilter = Linear;
	MagFilter = Linear;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
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
	float3 FV		: TEXCOORD4;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output;

	output.Light = normalize(-SunDirection);
	output.Tex = input.TextureCoordinate;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Pos = mul(viewPosition, Projection);

	output.View = normalize(ViewVector - worldPosition.xyz);
	output.Norm = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	output.FV = ViewVector - worldPosition.xyz;
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 textureColor = tex2D(texSampler, input.Tex);
	if (textureColor.w < 0.2)
		discard;

	float sd = abs(length(ViewVector - input.FV));
	float fogFactor = saturate((FogEnd - sd) / (FogEnd - FogStart));

	float4 result;

	float3 Normal = normalize(input.Norm);
	float3 LightDir = normalize(input.Light);
	float3 ViewDir = normalize(input.View);
	float3 H = normalize(LightDir + ViewDir);

	float3 Ia = AmbientColour;
	float3 Id = textureColor.xyz * saturate(dot(Normal, LightDir));
	float3 Is = SpecularColor.xyz * pow(saturate(dot(Normal, H)), Shininess);

	result.xyz = saturate(saturate(Id + Ia) + Is);
	//result.xyz = (fogFactor * result.xyz) + (1.0 - fogFactor) * FogColour;

	result.a = textureColor.a;


	return result;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};