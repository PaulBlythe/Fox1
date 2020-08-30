float4x4 Projection;
float noise;
float4 fgColor;

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
    float mask = tex2D(tex,input.UV).r;
	float4 clr;
	
	clr.rgb = fgColor.rgb;

	if( mask < 0.4 )  
		discard;
	else  
		clr.a = 1.0;

	// do some anti-aliasing
	clr.a *= smoothstep(0.125, 1.00, mask);
	return clr;
}

technique Technique1
{
    pass Pass1
    {

        VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
        PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
    }
}
