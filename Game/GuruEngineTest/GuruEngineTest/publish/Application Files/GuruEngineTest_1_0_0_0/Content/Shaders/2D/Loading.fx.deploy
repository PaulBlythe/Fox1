

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
	AddressU = Mirror;
	AddressV = Mirror;
	AddressW = Mirror;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;

};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};


float iTime = 0;
float2 iResolution = float2(1920, 1080);
float aspectratio;

float mod(float x, float y)
{
	return x - y * floor(x / y);
}


float iqhash(float n)
{
	return frac(sin(n)*43758.5453);
}

float noise(float3 x)
{
	// The noise function returns a value in the range -1.0f -> 1.0f
	float3 p = floor(x);
	float3 f = frac(x);

	f = f * f*(3.0 - 2.0*f);
	float n = p.x + p.y*57.0 + 113.0*p.z;
	return lerp(lerp(lerp(iqhash(n + 0.0), iqhash(n + 1.0), f.x),
		lerp(iqhash(n + 57.0), iqhash(n + 58.0), f.x), f.y),
		lerp(lerp(iqhash(n + 113.0), iqhash(n + 114.0), f.x),
			lerp(iqhash(n + 170.0), iqhash(n + 171.0), f.x), f.y), f.z);
}


//float noise(in float3 x)
//{
//	float3 p = floor(x);
//	float3 f = frac(x);
//	f = f * f*(3.0 - 2.0*f);
//
//	float2 uv = (p.xy + float2(37.0, 17.0)*p.z) + f.xy;
//	float2 rg = tex2Dlod(SpriteTextureSampler,float4( (uv + 0.5) / 256.0,0,0)).yx;
//
//	return -1.0 + 2.0*lerp(rg.x, rg.y, f.z);
//}

float map5(in float3 p)
{
	float3 q = p - float3(0.0, 0.1, 1.0)*iTime;
	float f;
	f = 0.50000*noise(q); q = q * 2.02;
	f += 0.25000*noise(q); q = q * 2.03;
	f += 0.12500*noise(q); q = q * 2.01;
	f += 0.06250*noise(q); q = q * 2.02;
	f += 0.03125*noise(q);
	return clamp(1.5 - p.y - 2.0 + 1.75*f, 0.0, 1.0);
}

float map4(in float3 p)
{
	float3 q = p - float3(0.0, 0.1, 1.0)*iTime;
	float f;
	f = 0.50000*noise(q); q = q * 2.02;
	f += 0.25000*noise(q); q = q * 2.03;
	f += 0.12500*noise(q); q = q * 2.01;
	f += 0.06250*noise(q);
	return clamp(1.5 - p.y - 2.0 + 1.75*f, 0.0, 1.0);
}
float map3(in float3 p)
{
	float3 q = p - float3(0.0, 0.1, 1.0)*iTime;
	float f;
	f = 0.50000*noise(q); q = q * 2.02;
	f += 0.25000*noise(q); q = q * 2.03;
	f += 0.12500*noise(q);
	return clamp(1.5 - p.y - 2.0 + 1.75*f, 0.0, 1.0);
}
float map2(in float3 p)
{
	float3 q = p - float3(0.0, 0.1, 1.0)*iTime;
	float f;
	f = 0.50000*noise(q); q = q * 2.02;
	f += 0.25000*noise(q);;
	return clamp(1.5 - p.y - 2.0 + 1.75*f, 0.0, 1.0);
}

float3 sundir = normalize(float3(-1.0, 0.0, -1.0));

float4 integrate(in float4 sum, in float dif, in float den, in float3 bgcol, in float t)
{
	// lighting
	float3 lin = float3(0.65, 0.7, 0.75)*1.4 + float3(1.0, 0.6, 0.3)*dif;
	float4 col = float4(lerp(float3(1.0, 0.95, 0.8), float3(0.25, 0.3, 0.35), den), den);
	col.xyz *= lin;
	col.xyz = lerp(col.xyz, bgcol, 1.0 - exp(-0.003*t*t));
	// front to back blending    
	col.a *= 0.4;
	col.rgb *= col.a;
	return sum + col * (1.0 - sum.a);
}

#define MARCH(STEPS,MAPLOD) for(i=0; i<STEPS; i++) { float3  pos = ro + t*rd; if( pos.y<-3.0 || pos.y>2.0 || sum.a > 0.99 ) break; float den = MAPLOD( pos ); if( den>0.01 ) { float dif =  clamp((den - MAPLOD(pos+0.3*sundir))/0.6, 0.0, 1.0 ); sum = integrate( sum, dif, den, bgcol, t ); } t += max(0.05,0.02*t); }

float4 raymarch(in float3 ro, in float3 rd, in float3 bgcol)
{
	float4 sum = float4(0.0,0,0,0);

	float t = 0.0;//0.05*texelFetch( iChannel0, px&255, 0 ).x;
	int i;
	MARCH(30, map5);
	MARCH(30, map4);
	MARCH(30, map3);
	MARCH(30, map2);

	return clamp(sum, 0.0, 1.0);
}

float3x3 setCamera(in float3 ro, in float3 ta, float cr)
{
	float3 cw = normalize(ta - ro);
	float3 cp = float3(sin(cr), cos(cr), 0.0);
	float3 cu = normalize(cross(cw, cp));
	float3 cv = normalize(cross(cu, cw));
	return float3x3(cu, cv, cw);
}

float4 render(in float3 ro, in float3 rd)
{
	// background sky     
	float sun = clamp(dot(sundir, rd), 0.0, 1.0);
	float3 col = float3(0.6, 0.71, 0.75) - rd.y*0.2*float3(1.0, 0.5, 1.0) + 0.15*0.5;
	col += 0.2*float3(1.0, .6, 0.1)*pow(sun, 8.0);

	// clouds    
	float4 res = raymarch(ro, rd, col);
	col = col * (1.0 - res.w) + res.xyz;

	// sun glare    
	col += 0.2*float3(1.0, 0.4, 0.2)*pow(sun, 3.0);

	return float4(col, 1.0);
}



float4 MainPS(VertexShaderOutput input) : COLOR
{
	//float2 q = input.TextureCoordinates / iResolution;
	//float2 p = q - 0.5;
	float2 p = (-iResolution.xy + 2.0*input.Position.xy) / iResolution.y;
	p.y = 1 - p.y;
	p.x *= aspectratio;
	// camera
	float3 ro = 4.0*normalize(float3(0,0.125f,1));
	float3 ta = float3(0.0, -1.0, 0.0);
	float3x3 ca = setCamera(ro, ta, 0.0);
	// ray
	float3 rd = mul(ca, normalize(float3(p.xy, 1.5)));
	
	return render(ro, rd);
}


technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile ps_4_0 MainPS();
	}
};