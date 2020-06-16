#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_3
	#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

float4x4 Projection;
float  pxRange;
float2 textureSize;
float4 bgColor;
float4 fgColor;

Texture2D Texture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <Texture>;
};


struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 UV :       TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	output.Position = mul(input.Position, Projection);
	output.TextureCoordinates = input.UV;

	return output;
}

//based of Chlumsky's basic GLSL shader.

//Chlumsky's MSDF generator repository
//https://github.com/Chlumsky/msdfgen#using-a-multi-channel-distance-field


float median(float r, float g, float b) 
{
	return max(min(r, g), min(max(r, g), b));
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 coord = input.TextureCoordinates;
	float2 msdfUnit = pxRange / textureSize;
	float3 samp = (tex2D(SpriteTextureSampler, coord)).rgb;

	float sigDist = median(samp.r, samp.g, samp.b);
	if (sigDist < 0.4f)
		discard;
	return fgColor;
}

technique SpriteDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
