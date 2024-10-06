sampler2D TextureSampler : register(s0);
float4x4 WorldViewProjection;

struct VS_Input
{
    float4 Position : POSITION0; 
    float2 TexCoord : TEXCOORD0;
};

struct VS_Output
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

VS_Output VS_Main(VS_Input input)
{
    VS_Output output;
    output.Position = mul(input.Position, WorldViewProjection);
    output.TexCoord = input.TexCoord;
    return output;
}

float4 PS_Main(float2 coord: TEXCOORD0) : COLOR0
{    
    float4 color = float4(1, 1, 1, 1);
    return color;
}

technique BasicTechnique
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS_Main();
        PixelShader = compile ps_2_0 PS_Main();
    }
}
