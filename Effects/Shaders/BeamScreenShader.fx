sampler uImage0 : register(s0);

float4 PSMain(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    color.rgb *= 0;
    return color;
}

technique Technique1
{
    pass P0
    {
        PixelShader = compile ps_2_0 PSMain();
    }
}