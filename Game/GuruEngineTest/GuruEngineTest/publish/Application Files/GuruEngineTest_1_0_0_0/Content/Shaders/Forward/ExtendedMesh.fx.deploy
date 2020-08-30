#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

matrix ModelViewProjection;
matrix ModelView;

// Material settings
float4  GlobalAmbient;
float4  AmbientColor;
float4  EmissiveColor;
float4  DiffuseColor;
float4  SpecularColor;
float4  Reflectance;
float   Opacity;
float   SpecularPower;
float   IndexOfRefraction;
bool    HasAmbientTexture;
bool    HasEmissiveTexture;
bool    HasDiffuseTexture;
bool    HasSpecularTexture;
bool    HasSpecularPowerTexture;
bool    HasNormalTexture;
bool    HasBumpTexture;
bool    HasOpacityTexture;
float   BumpIntensity;
float   SpecularScale;
float   AlphaThreshold;

// Light settings
float4 lightColor;
float4 lightDirectionVS;
float lightIntensity;

Texture2D AmbientTexture        : register(t0);
Texture2D EmissiveTexture       : register(t1);
Texture2D DiffuseTexture        : register(t2);
Texture2D SpecularTexture       : register(t3);
Texture2D SpecularPowerTexture  : register(t4);
Texture2D NormalTexture         : register(t5);
Texture2D BumpTexture           : register(t6);
Texture2D OpacityTexture        : register(t7);

SamplerState LinearRepeatSampler
{
	MagFilter = POINT;
	MinFilter = POINT;
	Mipfilter = POINT;
	AddressU = Wrap;
	AddressV = Wrap;
};


struct VertexShaderInput
{
	float3 position : POSITION;
	float3 tangent  : TANGENT;
	float3 binormal : BINORMAL;
	float3 normal   : NORMAL;
	float2 texCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float3 positionVS   : TEXCOORD0;    // View space position.
	float2 texCoord     : TEXCOORD1;    // Texture coordinate
	float3 tangentVS    : TANGENT;      // View space tangent.
	float3 binormalVS   : BINORMAL;     // View space binormal.
	float3 normalVS     : NORMAL;       // View space normal.
	float4 position     : SV_POSITION;  // Clip space position.
};

VertexShaderOutput MainVS(VertexShaderInput IN)
{
	VertexShaderOutput OUT;

	OUT.position = mul(ModelViewProjection, float4(IN.position, 1.0f));

	OUT.positionVS = mul(ModelView, float4(IN.position, 1.0f)).xyz;
	OUT.tangentVS = mul((float3x3)ModelView, IN.tangent);
	OUT.binormalVS = mul((float3x3)ModelView, IN.binormal);
	OUT.normalVS = mul((float3x3)ModelView, IN.normal);

	OUT.texCoord = IN.texCoord;

	return OUT;
}

float3 ExpandNormal(float3 n)
{
	return n * 2.0f - 1.0f;
}


float4 DoNormalMapping(float3x3 TBN, Texture2D tex, sampler s, float2 uv)
{
	float3 normal = tex.Sample(s, uv).xyz;
	normal = ExpandNormal(normal);

	// Transform normal from tangent space to view space.
	normal = mul(normal, TBN);
	return normalize(float4(normal, 0));
}

float4 DoBumpMapping(float3x3 TBN, Texture2D tex, sampler s, float2 uv, float bumpScale)
{
	// Sample the heightmap at the current texture coordinate.
	float height = tex.Sample(s, uv).r * bumpScale;
	// Sample the heightmap in the U texture coordinate direction.
	float heightU = tex.Sample(s, uv, int2(1, 0)).r * bumpScale;
	// Sample the heightmap in the V texture coordinate direction.
	float heightV = tex.Sample(s, uv, int2(0, 1)).r * bumpScale;

	float3 p = { 0, 0, height };
	float3 pU = { 1, 0, heightU };
	float3 pV = { 0, 1, heightV };

	// normal = tangent x bitangent
	float3 normal = cross(normalize(pU - p), normalize(pV - p));

	// Transform normal from tangent space to view space.
	normal = mul(normal, TBN);

	return float4(normal, 0);
}

float4 DoDiffuse(float4 L, float4 N)
{
	float NdotL = max(dot(N, L), 0);
	return lightColor * NdotL;
}

float4 DoSpecular(float4 V, float4 L, float4 N)
{
	float4 R = normalize(reflect(-L, N));
	float RdotV = max(dot(R, V), 0);

	return lightColor * pow(RdotV, SpecularPower);
}







[earlydepthstencil]
float4 MainPS(VertexShaderOutput IN) : COLOR
{
	float4 eyePos = { 0, 0, 0, 1 };

	float4 diffuse = DiffuseColor;
	if (HasDiffuseTexture)
	{
		float4 diffuseTex = DiffuseTexture.Sample(LinearRepeatSampler, IN.texCoord);
		if (any(diffuse.rgb))
		{
			diffuse *= diffuseTex;
		}
		else
		{
			diffuse = diffuseTex;
		}
	}

	float alpha = diffuse.a;
	if (HasOpacityTexture)
	{
		alpha = OpacityTexture.Sample(LinearRepeatSampler, IN.texCoord).r;
	}

	float4 ambient = AmbientColor;
	if (HasAmbientTexture)
	{
		float4 ambientTex = AmbientTexture.Sample(LinearRepeatSampler, IN.texCoord);
		if (any(ambient.rgb))
		{
			ambient *= ambientTex;
		}
		else
		{
			ambient = ambientTex;
		}
	}
	ambient *= GlobalAmbient;

	float4 emissive = EmissiveColor;
	if (HasEmissiveTexture)
	{
		float4 emissiveTex = EmissiveTexture.Sample(LinearRepeatSampler, IN.texCoord);
		if (any(emissive.rgb))
		{
			emissive *= emissiveTex;
		}
		else
		{
			emissive = emissiveTex;
		}
	}

	float specular = SpecularPower;

	if (HasSpecularPowerTexture)
	{
		specular = SpecularPowerTexture.Sample(LinearRepeatSampler, IN.texCoord).r * SpecularScale;
	}

	float4 N;

	// Normal mapping
	if (HasNormalTexture)
	{
		// For scenes with normal mapping, I don't have to invert the binormal.
		float3x3 TBN = float3x3(normalize(IN.tangentVS), normalize(IN.binormalVS), normalize(IN.normalVS));
		N = DoNormalMapping(TBN, NormalTexture, LinearRepeatSampler, IN.texCoord);
	}
	// Bump mapping
	else if (HasBumpTexture)
	{
		// For most scenes using bump mapping, I have to invert the binormal.
		float3x3 TBN = float3x3(normalize(IN.tangentVS), normalize(-IN.binormalVS), normalize(IN.normalVS));

		N = DoBumpMapping(TBN, BumpTexture, LinearRepeatSampler, IN.texCoord, BumpIntensity);
	}
	// Just use the normal from the model.
	else
	{
		N = normalize(float4(IN.normalVS, 0));
	}
	
	float4 resultDiffuse;
	float4 resultSpecular;
	float4 L = normalize(-lightDirectionVS);

	resultDiffuse = DoDiffuse(L, N) * lightIntensity;
	resultSpecular = DoSpecular(eyePos, L, N) * lightIntensity;

	diffuse *= float4(resultDiffuse.xyz, 1.0f); // Discard the alpha value from the lighting calculations.

	float4 SpecColour = float4(0, 0, 0, 0);

	if (specular > 1.0f) // If specular power is too low, don't use it.
	{
		SpecColour = SpecularColor;
		if (HasSpecularTexture)
		{
			float4 specularTex = SpecularTexture.Sample(LinearRepeatSampler, IN.texCoord);
			if (any(SpecColour.rgb))
			{
				SpecColour *= specularTex;
			}
			else
			{
				SpecColour = specularTex;
			}
		}
		SpecColour *= resultSpecular;
	}

	return float4((ambient + emissive + diffuse + SpecColour).rgb, alpha * Opacity);
}



technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};