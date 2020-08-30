float4x4 Projection;
float4 colour;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 UV :       TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 UV		: TEXCOORD0;
};

texture Texture;

sampler tex = sampler_state
{
    Texture = (Texture);

};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = mul(input.Position, Projection);
	output.UV = input.UV;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	return tex2D(tex,input.UV) * colour;
}

technique Technique1
{
    pass Pass1
    {

        VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
        PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
    }
}