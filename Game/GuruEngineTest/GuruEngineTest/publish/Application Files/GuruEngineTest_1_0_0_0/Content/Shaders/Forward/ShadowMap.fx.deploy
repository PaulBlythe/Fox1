

matrix WorldViewProjection;

struct VertexShaderInput
{
	float4 Position				: POSITION0;
};

struct VSOutput
{
	float4 position : POSITION;
	float2 depth	: TEXCOORD0;
};

VSOutput VSShadowMap(VertexShaderInput IN)
{
	VSOutput output;
	output.position = mul(IN.Position, WorldViewProjection);
	output.depth = output.position.zw;
	return output;
}

float4 PSShadowMap(VSOutput input) : COLOR
{
	return float4(input.depth.x / input.depth.y, 0, 0, 1);
}

technique ShadowMap
{
	pass
	{
		VertexShader = compile vs_4_0 VSShadowMap();
		PixelShader = compile ps_4_0 PSShadowMap();
	}
}