#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif


struct VertexShaderInput
{
	float4 Position : POSITION0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output;

	output.Position = float4(input.Position.xyz, 1);
	return output;
}

struct PixelShaderOutput
{
	float4 Color	: COLOR0;
	float4 Normal	: COLOR1;
	float4 Depth	: COLOR2;
	float4 Material : COLOR3;
};

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
	PixelShaderOutput output;
	output.Color = 0.0f;
	output.Color.a = 0.0f;
	output.Normal.rgb = 0.5f;
	output.Normal.a = 0.0f;
	output.Depth = 10000.0f;
	output.Material = 0.0f; 
	return output;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
};