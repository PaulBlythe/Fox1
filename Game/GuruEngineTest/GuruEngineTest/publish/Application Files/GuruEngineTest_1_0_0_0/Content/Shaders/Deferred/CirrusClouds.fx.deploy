#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif


/////////////////////////////////////////////////////////////////////////////////////////
/// constants
/////////////////////////////////////////////////////////////////////////////////////////
const float near_dist = 40000;
const float near_fade_dist = 10000;
const float MAX_CIRRUS_OPACITY = 0.9f;
const int num_layers = 15;
/////////////////////////////////////////////////////////////////////////////////////////
/// input variables
/////////////////////////////////////////////////////////////////////////////////////////
float3 sunDir;
float4 sun_color;
float4 fog_color;
float3 cameraPosWorld;

float cirrus_height;
float cirrus_layer_thickness;
float planet_radius;

bool is_far_camera = false;
bool is_upper_side = false;

float4x4 WorldViewProjection;

/////////////////////////////////////////////////////////////////////////////////////////
/// Textures
/////////////////////////////////////////////////////////////////////////////////////////

texture CirrusTexture;
sampler2D sampler_cirrus = sampler_state
{
	Texture = (CirrusTexture);
};


/////////////////////////////////////////////////////////////////////////////////////////
/// Functions
/////////////////////////////////////////////////////////////////////////////////////////
bool sphereIntersection(float3 ray_origin, float3 ray_dir, float ray_length, float3 sphere_center, float sphere_radius, out float t0, out float t1)
{
	float3 L = sphere_center - ray_origin;
	float3 D = ray_dir;
	float tCA = dot(L, D);

	if (tCA < 0)
	{
		return false;
	}

	float d = sqrt(dot(L, L) - dot(tCA, tCA));

	if (d < 0)
	{
		return false;
	}

	float tHC = sqrt(sphere_radius*sphere_radius - d * d);

	t0 = tCA - tHC;
	t1 = tCA + tHC;

	return true;
}

float getSphereIntersectionFromInside(float3 rayStart, float3 rayDir, float3 sphere_center, float radius)
{
	// scalar projection
	// = distFromCameraToDeepestPoint
	// may be negative
	float rayStartOntoRayDirScalar = dot(sphere_center - rayStart, rayDir);

	if (isnan(rayStartOntoRayDirScalar)) {
		return 0.0;
	}

	float3 deepestPoint = rayStart + rayDir * rayStartOntoRayDirScalar;

	float deepestPointHeight = distance(deepestPoint, sphere_center);

	float distFromDeepestPointToIntersection = sqrt(pow(radius, 2.0) - deepestPointHeight * deepestPointHeight);

	if (isnan(distFromDeepestPointToIntersection)) 
	{
		return 0.0;
	}

	if (distFromDeepestPointToIntersection < 0)
	{
		return 0.0;
	}

	float dist = rayStartOntoRayDirScalar + distFromDeepestPointToIntersection;

	return dist;
}

float fogAndToneMap( float3 in_color)
{
	float3 out_color = in_color;
	out_color *= 1.0 - sun_color.w;
	out_color = lerp(out_color, float3(1,1,1), sun_color.xyz);
	out_color = lerp(out_color, fog_color.xyz, fog_color.w);
	return out_color;
}




float3 calcCirrusLight(float3 pos)
{
	float3 lightColorHigh = float3(1.0, 1.0, 0.95);
	float3 lightColorLow = float3(1.0, 0.9, 0.7);
	float3 lightColorVeryLow = float3(0.6, 0.43, 0.28);
	float3 lightColorLowest = float3(0.5, 0.2, 0.05);

	float3 lightColor = lightColorHigh;

	lightColor = lerp(lightColorLow, lightColor, smoothstep(0.0, 0.2, sunDir.y));
	lightColor = lerp(lightColorVeryLow, lightColor, smoothstep(0.0, 0.15, sunDir.y));
	lightColor = lerp(lightColorLowest, lightColor, smoothstep(-0.4, -0.0, sunDir.y));

	lightColor *= smoothstep(-0.5, 0.0, sunDir.y);

	return lightColor;
}

float getCloudDensityNear(float3 view_dir)
{
	
	const float layer_alpha = 1.0 / num_layers;

	float cloud_density_near = 0;

	for (int i = 0; i < num_layers; i++)
	{
		float pos_in_layer = float(i) / float(num_layers);
		pos_in_layer = (2 * pos_in_layer) - 1.0;

		float density = 1 - abs(pos_in_layer);

		float height = cirrus_height + (pos_in_layer * (cirrus_layer_thickness / 2));

		float3 coord = float3(0,0,0);
		float dist = 0;

		float3 sphere_center = float3(cameraPosWorld.xy, -planet_radius);
		float sphere_radius = planet_radius + height;

		if (cameraPosWorld.z < height)
		{
			float t0 = getSphereIntersectionFromInside(cameraPosWorld, view_dir, sphere_center, sphere_radius);
			if (t0 > 0)
			{
				coord = cameraPosWorld + view_dir * t0;
				dist = t0;
			}
		}
		else
		{
			float t0, t1;
			if (sphereIntersection(cameraPosWorld, view_dir, 0, sphere_center, sphere_radius, t0, t1))
			{
				if (t0 > 0)
				{
					coord = cameraPosWorld + view_dir * t0;
					dist = t0;
				}
				else
				{
					coord = cameraPosWorld + view_dir * t1;
					dist = t1;
				}
			}
		}

		float bias = 4 * smoothstep(60000, 200000, dist);

		density *= tex2Dlod(sampler_cirrus, float4(coord.xy * 0.00002, 0 , bias)).x;

		if (dist > 0)
			cloud_density_near += density * layer_alpha;
	}

	cloud_density_near = min(2 * cloud_density_near, 1);

	return cloud_density_near;
}

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL;
};

struct VertexShaderOutput
{
	float4 Position		: SV_POSITION;
	float3 PositionView	: TEXCOORD0;
	float3 Normal		: TEXCOORD1;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	float4 pos = input.Position;
	output.Normal = input.Normal.xyz;

	float height = is_upper_side ? cirrus_height + (cirrus_layer_thickness / 2) : cirrus_height - (cirrus_layer_thickness / 2);

	pos.xyz *= planet_radius + cirrus_height;
	pos.y -= planet_radius;
	pos.xz += cameraPosWorld.xy;

	output.PositionView = pos.xyz;
	output.Position = mul(pos, WorldViewProjection);

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 result;
	float3 view_dir = (input.PositionView - cameraPosWorld);
	float dist = length(view_dir);
	view_dir = normalize(view_dir);

	float3 normal = normalize(input.Normal);
	if (cameraPosWorld.y > cirrus_height)
		normal = -normal;

	float cloud_density = MAX_CIRRUS_OPACITY * getCloudDensityNear(view_dir);

	float3 color = calcCirrusLight(input.PositionView);

	result.w = clamp(cloud_density, 0, 1);

	float near_fade = clamp(near_dist - dist, 0, near_fade_dist) / near_fade_dist;
	if (is_far_camera)
		result.w *= 1 - near_fade;
	else
		result.w *= near_fade;

	result.xyz = color;
	result.xyz = fogAndToneMap(result.xyz);
	return result;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};