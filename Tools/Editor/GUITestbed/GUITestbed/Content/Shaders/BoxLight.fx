#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

float4x4 World;
float3 Minimums;
float3 Maximums;
float2 Offset;
float Scale;

float4x4 WorldInverseTranspose;
float3 EyePosition;

struct VertexShaderInput
{
	float4 Position				: POSITION0;
	float3 Normal				: NORMAL0;
	float2 TextureCoordinate	: TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position				: SV_POSITION;
	float3 WorldPos				: TEXCOORD0;
	float3 Norm					: TEXCOORD1;
	float3 View					: TEXCOORD2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	float2 sp = float2(-1,-1) + (Scale * input.TextureCoordinate );
	sp += Offset;

	output.Position = float4(sp.x, sp.y, 1.0f, 1);                        
	output.WorldPos = mul(input.Position, World);
	output.View = normalize(EyePosition - output.WorldPos.xyz);
	output.Norm = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 res = float4(0,0,0,1);
	if (input.WorldPos.x > Minimums.x)
	{
		if (input.WorldPos.y > Minimums.y)
		{
			if (input.WorldPos.z > Minimums.z)
			{
				if (input.WorldPos.x < Maximums.x)
				{
					if (input.WorldPos.y < Maximums.y)
					{
						if (input.WorldPos.z < Maximums.z)
						{
							float3 Normal = normalize(input.Norm);
							float3 LightDir = float3(0, -1, 0);
							float3 ViewDir = normalize(input.View);
							float3 H = normalize(LightDir + ViewDir);

							float3 Id = saturate(dot(Normal, LightDir));
							float3 Is = pow(saturate(dot(Normal, H)), 15);
							float l = saturate(0.4f + Id + Is);
							res = float4(l, l, l, 1.0);
						}
					}
				}
			}
		}
	}
	return res;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};