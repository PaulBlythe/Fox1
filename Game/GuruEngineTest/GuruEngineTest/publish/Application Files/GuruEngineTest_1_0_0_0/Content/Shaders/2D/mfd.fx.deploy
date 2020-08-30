float brightness;
float contrast;
bool invert;

sampler TextureSampler : register(s0);

float4 PixelShaderFunction(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
	float4 colour = tex2D(TextureSampler,texCoord);
	
	if (invert)
	{
		colour.a  = 1.0;
		colour.g = 1.0 - colour.g;
	}
	if (colour.a < 0.1f)
		discard;
		
	colour.rgb = (colour.rgb * color.rgb);        
	colour.rgb = (colour.rgb - 0.5f) * (contrast) + 0.5f;
	colour.rgb = colour.rgb + brightness;        
	
	return colour;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
    }
}