// Lab2
float4x4 World;
float4x4 View;
float4x4 Projection;

float3 offset; // for rotation

texture MyTexture;

sampler mySampler = sampler_state {
	Texture = <MyTexture>;
};


struct VertexPositionTexture {
	float4 Position : POSITION;
	float2 TextureCoordinate : TEXCOORD;
};


VertexPositionTexture MyVertexShader(VertexPositionTexture input)
{
	// *** Lab2 for 3D
	VertexPositionTexture output;
	float4 worldPos = mul(input.Position, World); // Step 1
	float4 viewPos = mul(worldPos, View); // Step 2
	float4 projPos = mul(viewPos, Projection); // Step 3
	output.Position = projPos;
	output.TextureCoordinate = input.TextureCoordinate; // UV
	return output;
}

float4 MyPixelShader(VertexPositionTexture input) : COLOR
{
	float2 textureCoodinate = input.TextureCoordinate;
	return tex2D(mySampler, textureCoodinate);
}

technique MyTechnique
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 MyVertexShader();
		PixelShader = compile ps_4_0 MyPixelShader();
	}
}
