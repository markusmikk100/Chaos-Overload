sampler uImage0 : register(s0);
float2 viewportSize;

float4 PSMain(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float4 bloom = float4(0, 0, 0, 0);

    float pxSizeX = 1.0f / viewportSize.x;
    float pxSizeY = 1.0f / viewportSize.y;

    float size = 16.0f;

    float stepSize = 1.0f / size;
    for (float i = 0; i <= size; i++) {
        float t = 1 - stepSize * i;
        float multiplier = t * 0.4f;

        float xOffset = i * pxSizeX;
        float yOffset = i * pxSizeY;

        float4 north = tex2D(uImage0, coords + float2(0, yOffset));
        if (any(north)) {
            bloom += north * multiplier;
        }
        float4 east = tex2D(uImage0, coords + float2(xOffset, 0));
        if (any(east)) {
            bloom += east * multiplier;
        }
        float4 south = tex2D(uImage0, coords + float2(0, -yOffset));
        if (any(south)) {
            bloom += south * multiplier;
        }
        float4 west = tex2D(uImage0, coords + float2(-xOffset, 0));
        if (any(west)) {
            bloom += west * multiplier;
        }

        float4 northEast = tex2D(uImage0, coords + float2(xOffset, yOffset));
        if (any(northEast)) {
            bloom += northEast * multiplier;
        }
        float4 southEast = tex2D(uImage0, coords + float2(xOffset, -yOffset));
        if (any(southEast)) {
            bloom += southEast * multiplier;
        }
        float4 southWest = tex2D(uImage0, coords + float2(-xOffset, -yOffset));
        if (any(southWest)) {
            bloom += southWest * multiplier;
        }
        float4 northWest = tex2D(uImage0, coords + float2(-xOffset, yOffset));
        if (any(northWest)) {
            bloom += northWest * multiplier;
        }
    }
    
    return color + bloom * 0.2f;
}

technique Technique1
{
    pass P0
    {
        PixelShader = compile ps_3_0 PSMain();
    }
}