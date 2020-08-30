#include "ShaderVariables.inc"

static const float Pi = 3.1415926535897932384626433832795;
#define DR_GAMMA 2.0


/// Converts a color component given in linear space to non-linear gamma space.
/// \param[in] colorLinear  The color component in linear space.
/// \return The color component in non-linear gamma space.
float3 ToGamma(float3 colorLinear)
{
	// If DR_GAMMA is not 2, we might have to use pow(0.000001 + ..., ...).
	return pow(colorLinear, 1 / DR_GAMMA);
}

/// The phase function (which can be used for Rayleigh and Mie).
/// \param[in] cosTheta   The cosine of the angle between the direction to the light
///                       and the viewing direction (e.g. observer to sky).
/// \param[in] g          Scattering symmetry constant g. g is usually 0 for
///                       Rayleigh and within [0.75, 0.999] for Mie scattering.
/// \return The amount of light scattered in the direction of the observer.
float PhaseFunction(float cosTheta, float g)
{
	// Note: Some multiply this by 4 * Pi and divide the extinction/scatter coefficient
	// by 4 * Pi. - But the current form is better because the integral of the sphere
	// is 1!
	float gSquared = g * g;
	float cosThetaSquared = cosTheta * cosTheta;

	// Cornette-Shanks phase function
	return 3 / (8 * Pi)
		* ((1.0 - gSquared) / (2.0 + gSquared))
		* (1.0 + cosThetaSquared)
		/ pow(1.0 + gSquared - 2.0 * g * cosTheta, 1.5);  // Compiler warning: pow does not work for negative values.

														  // Henyey-Greenstein phase function
														  //return 1 / (4 * Pi) * (1.0 - gSquared) /
														  //       pow(1.0 + gSquared - 2.0 * g * cosTheta, 1.5);
}

/// The Rayleigh phase function. Same as PhaseFunction(cosTheta, 0).
/// \param[in] cosTheta   The cosine of the angle between the direction to the light
///                       and the viewing direction (e.g. observer to sky).
float PhaseFunctionRayleigh(float cosTheta)
{
	float cosThetaSquared = cosTheta * cosTheta;
	return 3.0 / (16.0 * Pi) * (1 + cosThetaSquared);
}


/// Computes where a ray hits a sphere (which is centered at the origin).
/// \param[in]  rayOrigin    The start position of the ray.
/// \param[in]  rayDirection The normalized direction of the ray.
/// \param[in]  radius       The radius of the sphere.
/// \param[out] enter        The ray parameter where the ray enters the sphere.
///                          0 if the ray is already in the sphere.
/// \param[out] exit         The ray parameter where the ray exits the sphere.
/// \return  0 or a positive value if the ray hits the sphere. A negative value
///          if the ray does not touch the sphere.
float HitSphere(float3 rayOrigin, float3 rayDirection, float radius, out float enter, out float exit)
{
	// Solve the equation:  ||rayOrigin + distance * rayDirection|| = r
	//
	// This is a straight-forward quadratic equation:
	//   ||O + d * D|| = r
	//   =>  (O + d * D)² = r²  where V² means V.V
	//   =>  d² * D² + 2 * d * (O.D) + O² - r² = 0
	// D² is 1 because the rayDirection is normalized.
	//   =>  d = -O.D + sqrt((O.D)² - O² + r²)

	float OD = dot(rayOrigin, rayDirection);
	float OO = dot(rayOrigin, rayOrigin);
	float radicand = OD * OD - OO + radius * radius;
	enter = max(0, -OD - sqrt(radicand));
	exit = -OD + sqrt(radicand);

	return radicand;  // If radicand is negative then we do not have a result - no hit.
}

/// Computes the approximate Chapman function times exp(-hRelative).
/// \param[in] X          The planet radius divided by H.
/// \param[in] h          The observer altitude divided by H.
/// \param[in] cosZenith  The cosine of the angle between zenith (straight up) and the ray direction.
/// \return The approximate Chapman function corrected by transitive consistency
///         according to [GPUPro3].
float ChapmanApproximation(float X, float h, float cosZenith)
{
	float c = sqrt(X + h);
	if (cosZenith >= 0)
	{
		// Looking above horizon.
		return c / (c * cosZenith + 1) * exp(-h);
	}
	else
	{
		// Looking below horizon.
		float x0 = sqrt(1 - cosZenith * cosZenith) * (X + h);
		float c0 = sqrt(x0);
		return 2 * c0 * exp(X - x0) - c / (1 - c * cosZenith) * exp(-h);
	}
}

/// Computes the optical depth for a ray shooting into the sky.
/// \param[in] h              The absolute observer altitude (height above ground).
/// \param[in] H              The scale height.
/// \param[in] radiusGround   The absolute earth radius (ground level).
/// \param[in] cosZenith      The cosine of the angle between "up" and the ray.
/// \return The optical depth.
/// \remarks
/// This method is based on [GPUPro3].
float GetOpticalDepthSchueler(float h, float H, float radiusGround, float cosZenith)
{
	return H * ChapmanApproximation(radiusGround / H, h / H, cosZenith);
}


/// Computes atmospheric scattering parameters for a viewing ray.
/// The ray must start at the observer and end at the end of the atmosphere
/// or at the terrain.
/// \param[in] rayStart           The ray origin.
/// \param[in] rayDirection       The normalized ray direction.
/// \param[in] rayLength          The ray length.
/// \param[in] hitGround          True if the ray hits the ground. False if looking
///                               at sky or terrain above the ground.
/// \param[in] sunDirection       The direction to the sun.
/// \param[in] radiusGround       The radius of the ground level (0 altitude).
/// \param[in] radiusAtmosphere   The radius of the top of the atmosphere.
/// \param[in] scaleHeight        The scale height.*
/// \param[in] numberOfSamples    The number of integration samples.
///                               This parameter is ignored on XBox. Xbox uses 3 samples.
/// \param[in] betaRayleigh       The extinction/scatter coefficient for Rayleigh.**
/// \param[in] betaMie            The extinction/scatter coefficient for Mie.**
/// \param[out] transmittance     The computed transmittance along the ray.
/// \param[out] colorRayleigh     The inscattered light color from Rayleigh scattering.
/// \param[out] colorMie          The inscattered light color from Mie scattering.
/// \remarks
/// The phase functions are not applied in this function because it is possible
/// to evaluate this function in a vertex shader. The phase functions must be applied in the pixel shader.
/// * We assume that Rayleigh and Mie have the same scale height. In reality they are different (8km for Rayleigh, 1.2km for Mie).
/// ** This function assumes that the extinction and scatter coefficients are equal.
/// This is usually correct for Rayleigh but not necessarily for Mie. For Mie the real extinction coefficient would be bExtinction = bScatter + bAbsorption.
///
void ComputeAtmosphericScattering(float3 rayStart, float3 rayDirection, float rayLength, bool hitGround, float3 sunDirection, float radiusGround, float radiusAtmosphere, float scaleHeight, int numberOfSamples, float3 betaRayleigh, float3 betaMie,
								  out float3 transmittance, out float3 colorRayleigh, out float3 colorMie)
{
	// Scale height (which is also an altitude).
	float H = scaleHeight;

	// For ground hits we have to march in the other direction - cannot compute
	// optical length for any path through the earth ground sphere.
	float neg = hitGround ? -1 : 1;

	// Ray end is the sky or the terrain hit point.
	float3 rayEnd = rayStart + rayDirection * rayLength;
	float radiusEnd = length(rayEnd);

	// Zenith direction is always the vector from the center of the earth to the point.
	float3 zenith = rayEnd / radiusEnd;

	// Altitude of ray end.
	float h = radiusEnd - radiusGround;

	// Optical depth of ray end (which is the sky or the terrain).
	float cosRay = dot(zenith, neg * rayDirection);
	float lastRayDepth = GetOpticalDepthSchueler(h, H, radiusGround, cosRay);

	// Optical depth of ray end to sun.
	float cosSun = dot(zenith, sunDirection);
	float lastSunDepth = GetOpticalDepthSchueler(h, H, radiusGround, cosSun);

	float segmentLength = rayLength / numberOfSamples;
	float3 T = 1;   // The ray transmittance (camera to sky/terrain).
	float3 S = 0;   // The inscattered light.
	for (int i = numberOfSamples - 1; i >= 0; i--)
	{
		float3 samplePoint = rayStart + i * segmentLength * rayDirection;
		float radius = length(samplePoint);
		zenith = samplePoint / radius;

		h = radius - radiusGround;

		// Optical depth of sample point to sky.
		cosRay = dot(zenith, neg * rayDirection);
		float sampleRayDepth = GetOpticalDepthSchueler(h, H, radiusGround, cosRay);
		// Optical depth of the current ray segment.
		float segmentDepth = neg * (sampleRayDepth - lastRayDepth);
		// Transmittance of the current ray segment.
		float3 segmentT = exp(-segmentDepth * (betaRayleigh + betaMie));

		// Optical depth of sample to sun.
		cosSun = dot(zenith, sunDirection);
		float sampleSunDepth = GetOpticalDepthSchueler(h, H, radiusGround, cosSun);
		// Average the depths of the segment end points.
		float segmentSunDepth = 0.5 * (sampleSunDepth + lastSunDepth);
		// Inscattered light is T * sun light intensity. We compute it for a sun intensity of 1.
		float3 segmentS = exp(-segmentSunDepth * (betaRayleigh + betaMie));

		// Last inscatter is attenuated by this segment.
		S = S * segmentT;

		// Add inscatter from current sample point.
		// Schüler uses this
		//S += (1 - segmentT) * segmentS;
		// O'Neil uses this. Not sure why the atmosphere height is here.
		//S += exp(-h / H) * segmentS * sampleLengthRelativeToAtmosphereHeight;
		// Since we do numeric integration, we need to really do this. (Btw.
		// beta coefficients have the unit [m^-1], so the length unit cancels itself
		// correctly.)
		//S += segmentS * segmentLength;
		// Some articles (O'Neil, Crytek) have exp(-h/H) in the integral:
		S += exp(-h / H) * segmentLength * segmentS;

		// Attenuation factors like T are combined by multiplication.
		T = T * segmentT;

		lastRayDepth = sampleRayDepth;
		lastSunDepth = sampleSunDepth;
	}

	transmittance = T;

	// Apply "scatter" coefficients. (The betas of the transmittance computation
	// are the "extinction" coefficients. We assume betaScatter = betaExtinction.)
	colorRayleigh = S * betaRayleigh;
	colorMie = S * betaMie;
}


float3 LerpColors(float3 color0, float3 color1, float shift, float parameter)
{
	float3 colorAverage = (color0 + color1) / 2;
	if (parameter < shift)
		return lerp(color0, colorAverage, parameter / shift);
	else
		return lerp(colorAverage, color1, (parameter - shift) / (1 - shift));
}




float3 SunIntensity;

float4 Radii = float4(1.025, 1, 1, (1.025 - 1) * 0.25);
#define RadiusAtmosphere Radii.x      // Radius of the top of the atmosphere (from earth center)
#define RadiusGround Radii.y          // Earth radius (ground level)
#define RadiusCamera Radii.z          // Distance of camera from earth center
#define ScaleHeight Radii.w           // Scale height as altitude (height above ground)

// The number of sample points taken along the ray.
int NumberOfSamples = 3;

// Extinction/Scatter coefficient for Rayleigh.
float3 BetaRayleigh;

// Extinction/Scatter coefficient for Mie.
float3 BetaMie;

// g for Mie.
float GMie = -0.75;

float Transmittance = 1;

float4 BaseHorizonColor;   // BaseHorizonColor (RGB) + Shift
float3 BaseZenithColor;

//-----------------------------------------------------------------------------
// Structures
//-----------------------------------------------------------------------------

struct VSInput
{
	float4 Position : POSITION;
};

struct VSOutput
{
	float3 PositionWorld	: TEXCOORD0;
	float4 Position			: SV_POSITION;
};


VSOutput VS(VSInput input)
{
	VSOutput output = (VSOutput)0;
	float4 worldpos = mul(input.Position, World);
	float4 viewpos = mul(worldpos, View);
	output.Position = mul(viewpos, Projection);

	output.PositionWorld = input.Position.xyz;
	//output.Position = mul(input.Position, mul(View, Projection)).xyww;  //  Set z to w to move vertex to far clip plane.
	return output;
}


float4 PS(float3 positionWorld, bool useBaseColor, bool correctGamma)
{
	float3 cameraToSkyDirection = normalize(positionWorld);
	//if (cameraToSkyDirection.y < -0.2f)
	//{
	//	clip(-1);
	//	return 0;
	//}

	// We shoot a ray from the camera to the vertex and take samples along the ray.
	float3 rayStart = float3(0, RadiusCamera, 0); // CameraPosition in planet space.

												  // Get length of ray by shooting the ray against the atmosphere top.
	float dummy, rayLength;
	float hasHit = HitSphere(rayStart, cameraToSkyDirection, RadiusAtmosphere, dummy, rayLength);

	// For games in outer space we can abort if we do not hit the atmosphere.
	// clip(hasHit);
	// The ray start would have to be moved to the first point inside the atmosphere...

	float3 colorRayleigh, colorMie;
	float3 transmittance;
	ComputeAtmosphericScattering(rayStart, cameraToSkyDirection, rayLength, false,
		SunDirection, RadiusGround, RadiusAtmosphere,
		ScaleHeight, NumberOfSamples, BetaRayleigh, BetaMie,
		transmittance, colorRayleigh, colorMie);

	// Weigh the colors with the phase function and sum them up.
	// Note: This should be done in the PS. The above part could be done in the VS.
	float cosTheta = dot(SunDirection, cameraToSkyDirection);
	float4 color;
	color.rgb = colorRayleigh * PhaseFunctionRayleigh(cosTheta) + colorMie * PhaseFunction(cosTheta, GMie);
	color.rgb *= SunIntensity;

	if (useBaseColor)
	{
		float f = 1 - saturate(acos(cameraToSkyDirection.y) / Pi * 2); // 1 = zenith, 0 = horizon
		color.rgb += LerpColors(BaseHorizonColor.rgb, BaseZenithColor.rgb, BaseHorizonColor.a, f);
	}

	// transmittance is float3. We arbitrarily use the transmittance of the g channel.
	color.a = 1 - (transmittance.g * Transmittance);

	//color.rgb = TonemapExponential(color.rgb);

	if (correctGamma)
		color.rgb = ToGamma(color.rgb);

	return color;
}
float4 PSLinear(VSOutput IN) : COLOR
{ 
	return PS(IN.PositionWorld, false, false);
}

float4 PSGamma(VSOutput IN) : COLOR
{ 
	return PS(IN.PositionWorld, false, true); 
}

float4 PSLinearWithBaseColor(VSOutput IN) : COLOR
{ 
	return PS(IN.PositionWorld, true, false);
}

float4 PSGammaWithBaseColor(VSOutput IN) : COLOR
{ 
	float4 res = PS(IN.PositionWorld, true, true); 
	return res;
}


technique Linear
{
	pass pass0                  // Output linear color values. No base color.
	{
		VertexShader = compile vs_4_0 VS();
		PixelShader = compile ps_4_0 PSLinear();
	}
}
technique Gamma
{
	pass pass0                  // Output gamma corrected values. No base color.
	{
		VertexShader = compile vs_4_0 VS();
		PixelShader = compile ps_4_0 PSGamma();
	}
}
technique LinearWithBaseColour
{
	pass pass0     // Output linear color values. Add base color.
	{
		VertexShader = compile vs_4_0 VS();
		PixelShader = compile ps_4_0 PSLinearWithBaseColor();
	}
}
technique GammaWithBaseColour
{
	pass pass0      // Output gamma corrected values. Add base color.
	{
		VertexShader = compile vs_4_0 VS();
		PixelShader = compile ps_4_0 PSGammaWithBaseColor();
	}
};