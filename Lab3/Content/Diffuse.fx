﻿float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float4 AmbientColor;
float AmbientIntensity;
float3 DiffuseLightDirection; // L Vector
float4 DiffuseColor;
float DiffuseIntensity;


struct VertexInput
{
	float4 Position : POSITION;
	float4 Normal : NORMAL;
};

struct VertexOutput
{
	float4 Position : POSITION;
	float4 Color : COLOR;
};

VertexOutput VertexShaderFunction(VertexInput input)
{
	VertexOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	float4 normal = mul(input.Normal, WorldInverseTranspose);
	float lightIntensity = max(0,dot(normal.xyz, normalize(DiffuseLightDirection)));
	output.Color = saturate(DiffuseColor * DiffuseIntensity * lightIntensity);


	return output;
}

float4 PixelShaderFunction(VertexOutput input) : COLOR
{
	return saturate(input.Color + AmbientColor * AmbientIntensity);
}

technique Diffuse
{
	pass Pass1 {
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
}