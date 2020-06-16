#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 Projection;
float3 LightDirection = float3(0.0, -1.00, 0.5f);
float3 LightDirection2 = float3(0.0, -1.00, 0.5f);
float4 HiLite = float4(0.6, 0.6, 0.6, 1.0);

struct VertexShaderInput
{
	float4 Position		: POSITION0;
	float4 Colour		: COLOR0;
	float4 Region		: NORMAL0;
	float2 UV			: TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Colour	: COLOR0;
	float2 UV		: TEXCOORD0;
	float4 Region	: TEXCOORD1;
};


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	output.Position = mul(input.Position, Projection);
	output.UV = input.UV;
	output.Region = input.Region;
	output.Colour = input.Colour;

	return output;
}



float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 outcolor = input.Colour;

	float3 N = normalize(float3(input.UV, 1));
	float3 L = normalize(LightDirection);
	float3 L2 = normalize(LightDirection2);
	float NdotL = max(dot(N, L), 0);
	float NdotL2 = pow(max(dot(N, L2), 0), 32);

	outcolor = saturate(outcolor + (NdotL *  float4(1, 1, 0.7, 1)));
	outcolor = saturate(outcolor + (NdotL2 *  float4(1, 1, 1, 1)));

	float onex = 1.0 / input.Region.z;
	float oney = 1.0 / input.Region.w;

	float edge = 0;
	if (input.UV.x < (2 * onex))
		edge = 1;

	if (input.UV.y > 1.0 - (2 * onex))
		edge = 1;

	float edge2 = 0;
	if (input.UV.x > 1 - (2 * onex))
		edge2 = 1;

	if (input.UV.y < (2 * onex))
		edge2 = 1;

	outcolor = lerp(outcolor, HiLite, edge2);
	outcolor = lerp(outcolor, float4(0, 0, 0, 1), edge);

	return outcolor;
}

technique SpriteDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};