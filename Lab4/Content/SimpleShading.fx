float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float4 AmbientColor;
float AmbientIntensity;
float3 DiffuseLightDirection; // L Vector
float4 DiffuseColor;
float DiffuseIntensity;

// Lab4 variable
float3 CameraPosition;
float3 LightPosition;
float Shininess;
float4 SpecularColor;
float SpecularIntensity = 1;


struct VertexShaderInput
{
	float4 Position : POSITION;
	float4 Normal : NORMAL;

};

struct VertexShaderOutput
{
	float4 Position : POSITION;
	float4 Color : COLOR;
	float4 Normal : TEXCOORD0;
	float4 WorldPosition : TEXCOORD1;
};

VertexShaderOutput GourandVertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = 0;
	output.Normal = 0;

	float3 N = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	float3 V = normalize(CameraPosition - worldPosition.xyz);
	float3 L = normalize(LightPosition);
	float3 R = reflect(-L, N);
	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N, L));
	float4 specular = pow(max(0, dot(V, R)), Shininess) * SpecularColor * SpecularIntensity;
	output.Color = saturate(ambient + diffuse + specular);
	output.Color.w = 1;
	return output;
}

VertexShaderOutput PhongVertexShader(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = worldPosition;
	output.Normal = input.Normal;
	output.Color = 0;
	return output;
}

float4 PhongPixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 N = normalize(input.Normal.xyz);
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 L = normalize(LightPosition);
	float3 R = reflect(-L, N);
	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N, L));
	float4 specular = SpecularIntensity * SpecularColor * pow(max(0, dot(V, R)), Shininess);
	float4 color = saturate(ambient + diffuse + specular);
	color.a = 1;
	return color;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
	return input.Color;
}

technique Diffuse
{
	pass Pass1 {
		VertexShader = compile vs_4_0 GourandVertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
}

technique Phong
{
	pass Pass1 {
		VertexShader = compile vs_4_0 PhongVertexShader();
		PixelShader = compile ps_4_0 PhongPixelShaderFunction();
	}
}