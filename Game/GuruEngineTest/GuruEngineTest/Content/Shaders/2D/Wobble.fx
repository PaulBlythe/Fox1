
texture Texture : register( t0 );
texture Noise : register( t1 );
float2 Viewport;
float iGlobalTime;

sampler tex = sampler_state
{
    Texture = (Texture);

};
sampler noise = sampler_state
{
    Texture = (Noise);
	AddressU = Wrap;
    AddressV = Wrap;

};
void SpriteVertexShader(inout float4 color    : COLOR0,
                            inout float2 texCoord : TEXCOORD0,
                            inout float4 position : POSITION0)
{
 // Half pixel offset for correct texel centering.

   position.xy -= 0.5;

   // Viewport adjustment.

   position.xy = position.xy / Viewport;

   position.xy *= float2(2, -2);

   position.xy -= float2(1, -1);
}

float2 fault(float2 uv, float s)
{
    float v = pow(0.5 - 0.5 * cos(2.0 * 3.141597 * uv.y), 100.0) * sin(2.0 * 3.141597 * uv.y);
    uv.x += v * 0.05 * s;
    return uv;
}

float4 PixelShaderFunction(in float4 color : COLOR0, in float2 texCoord: TEXCOORD0) : COLOR0
{
	float t = iGlobalTime / 10.0;
	float r = tex2D(noise, float2(t, 0.0)).x;
	float2 uv = fault(texCoord + float2(0.0, frac(t * 2.0)), 5.0 * sign(r) * pow(abs(r), 5.0)) - float2(0.0, frac(t * 2.0));
	float4 clr = tex2D(tex,uv);
	return clr * color;
}

technique Technique1
{
    pass Pass1
    {

        VertexShader = compile vs_4_0_level_9_1 SpriteVertexShader();
        PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
    }
}