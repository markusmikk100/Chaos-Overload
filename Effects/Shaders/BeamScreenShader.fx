sampler uImage0 : register(s0);
float2 viewportSize;

float4 PSMain(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);

    float4 bloom = float4(0, 0, 0, 0);

    float pxSizeX = 1 / viewportSize.x;
    float pxSizeY = 1 / viewportSize.y;

    float size = 12;

    float multiplier = 1 / size * 0.05f;
    for (int a = -size; a <= size; a++) {
        float newMultiplier = multiplier * abs(a);
        bloom += tex2D(uImage0, coords + float2(a * pxSizeX, 0)) * newMultiplier;
        bloom += tex2D(uImage0, coords + float2(0, a * pxSizeY)) * newMultiplier;
    }

    for (int b = -size; b <= size; b++) {
        float newMultiplier = multiplier * abs(b);
        bloom += tex2D(uImage0, coords + float2(b * pxSizeX, -b * pxSizeY)) * newMultiplier;
        bloom += tex2D(uImage0, coords + float2(b * pxSizeX, b * pxSizeY)) * newMultiplier;
    }
    
    return color + bloom;
}

technique Technique1
{
    pass P0
    {
        PixelShader = compile ps_3_0 PSMain();
    }
}