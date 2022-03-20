float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float3 CameraPosition;

texture decalMap;
texture environmentMap;
float reflectivity;


sampler tsampler1 = sampler_state
{
	texture = <decalMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

samplerCUBE SkyBoxSampler = sampler_state
{
	texture = <environmentMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Mirror;
	AddressV = Mirror;
};

struct VertexShaderInput {
	float4 Position : POSITION0;
	float2 texCoord : TEXCOORD0;
	float4 normal : NORMAL0;

};

struct VertexShaderOutput {
	float4 Position : POSITION0;
	float2 texCoord : TEXCOORD0;
	float3 R : TEXCOORD1;
};

VertexShaderOutput ReflectionVertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.texCoord = input.texCoord;
	float3 N = mul(input.normal, WorldInverseTranspose).xyz;
	float3 I = normalize(worldPosition.xyz - CameraPosition);
	output.R = reflect(I, normalize(N));
	//output.R = refract(I, normalize(N), 0.98);


	return output;
}

float4 ReflectPixelShader(VertexShaderOutput input) : COLOR0
{
	float4 reflectedColor = texCUBE(SkyBoxSampler, input.R);
	float4 decalColor = tex2D(tsampler1, input.texCoord);
	return lerp(decalColor, reflectedColor, reflectivity);
}

technique Reflection {
	pass Pass1 {
		VertexShader = compile vs_4_0 ReflectionVertexShaderFunction();
		PixelShader = compile ps_4_0 ReflectPixelShader();

	}
}