
#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

/*********************************************************************************************************************/
/* Matrices																											 */
/*********************************************************************************************************************/
float4x4 World;
float4x4 View;
float4x4 Projection;
float4x3 WorldInverseTranspose;
float4x4 ViewInverse;
float4x4 WorldViewProjection;
float4x4 WorldView;

/*********************************************************************************************************************/
/* Sun Light																										 */
/*********************************************************************************************************************/
float4 AmbientColour = float4(1, 1, 1, 1);
float4 SunDirection = float4(-3, 0, 6, 1);
float4 SunColour = float4(1, 1, 1, 1);

/*********************************************************************************************************************/
/* Material																											 */
/*********************************************************************************************************************/
float4 MaterialColour = float4(1, 1, 1, 1);
float DiffuseIntensity = 0.9;
float Shininess = 200;
float SpecularIntensity = 1;
float AlphaCut = 1.0;
bool TestAlpha = false;
float4 SpecularColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.8;

/*********************************************************************************************************************/
/* Vectors																											 */
/*********************************************************************************************************************/
float3 ViewVector = float3(1, 0, 0);


