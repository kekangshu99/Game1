

texture MyTexture;

sampler mySampler = sampler_state {
	Texture = <MyTexture>;
};


struct VertexPositionColor {
	float4 Position : POSITION;
	float4 Color : COLOR;
};

struct VertexPositionTexture {
	float4 position : POSITION;
	float2 TextureCoordinate : TEXCOORD;
};


VertexPositionTexture MyVertexShader(VertexPositionTexture input)
{
	return input;
}

float4 MyPixelShader(VertexPositionTexture input) : COLOR
{
	//float4 color = input.Color;
	//if (color.r % 0.1 < 0.05f) return float4(1, 1, 1, 1);
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
