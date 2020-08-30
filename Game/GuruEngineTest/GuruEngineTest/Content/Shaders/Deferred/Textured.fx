#if OPENGL
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
float4x3 WorldInverseTranspose;

float MoonLit;
float LightMask;
float Shininess = 120;
float SpecularIntensity = 1;
float  DiffuseIntensity = 1;

texture Texture1;
sampler2D textureSampler = sampler_state
{
	Texture = (Texture1);
};

struct VertexShaderInput
{
	float4 Position				: POSITION0;
	float3 Normal				: NORMAL0;
	float2 TextureCoordinate	: TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position				: POSITION0;
	float2 TexCoord				: TEXCOORD0;
	float3 Normal				: TEXCOORD1;
	float2 Depth				: TEXCOORD2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.TexCoord = input.TextureCoordinate;
	output.Normal = mul(float4(input.Normal, 1), WorldInverseTranspose).xyz;
	output.Depth.x = viewPosition.z;
	output.Depth.y = viewPosition.w;
	return output;
}

struct PixelShaderOutput
{
	float4 Color		: COLOR0;
	float4 Normal		: COLOR1;
	float  Depth : COLOR2;
	float4 Material		: COLOR3;
};

PixelShaderOutput MainPS(VertexShaderOutput input)
{
	PixelShaderOutput output;
	float4 colour = tex2D(textureSampler, input.TexCoord);
	
	output.Material.x = SpecularIntensity;
	output.Material.y = Shininess;
	output.Material.z = MoonLit;
	output.Material.w = LightMask;
	output.Color = colour;			// output Color
	output.Color.a = 0.2f;
	output.Normal.rgb = 0.5f * (normalize(input.Normal) + 1.0f);		// transform normal domain
	output.Normal.a = 1.0f;
	output.Depth = input.Depth.x/input.Depth.y;							// output Depth

	return output;
}

technique Textured
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile ps_4_0 MainPS();
	}
};