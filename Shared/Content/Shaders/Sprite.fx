float4x4 View;
float4x4 Projection;

texture SpriteSheet;
sampler2D textureSampler = sampler_state
{
	Texture = (SpriteSheet);
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 TextureCoordinate : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderOutput input)
{
	float4 viewPosition = mul(input.Position, View);
	input.Position = mul(viewPosition, Projection);

	return input;
}

float4 TexturedPixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	return tex2D(textureSampler, input.TextureCoordinate);
}

float4 SolidColorPixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);

	if (textureColor.a == 1) {
		textureColor = input.Color;
	}
	else {
		textureColor.a = 0;
	}

	return textureColor;
}

technique Textured 
{
	pass Pass1
	{
		VertexShader = compile vs_1_1 VertexShaderFunction();
		PixelShader = compile ps_2_0 TexturedPixelShaderFunction();
	}
}

technique Solid
{
	pass Pass1
	{
		VertexShader = compile vs_1_1 VertexShaderFunction();
		PixelShader = compile ps_2_0 SolidColorPixelShaderFunction();
	}
}