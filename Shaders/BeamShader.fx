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

    float x = coord.x * 2700;
    float y = abs(coord.y * 1200 - 600);

    if (x > 204) {
        float yLimit = clamp(pow(2 * log(x - 200) / log(4), 2.5), 0, 300);
    
        // Weak Blur
        if (y > yLimit) {
            float distance = y - yLimit;
            float blurAmount = 100;

            if (distance <= blurAmount)
            {
                float t = distance / blurAmount;
                float blurAmount = pow(-t + 1, 3);

                color = float4(1, 0, 0, 0.25);
                return color * blurAmount;
            }
                
            return color * 0;
        }

        // Edge Color
        float colorSize = 75;
        if (y > yLimit - colorSize) 
        {
            float distance = y - (yLimit - colorSize);
            float t = distance / colorSize;
            float colorAmount = 1 - pow(t, 3);

            return lerp(float4(1, 0, 0, 1) * 0.9, color, colorAmount);
        }

        // Inner color
        return color;
    }

    // Empty space
    return color * 0;
}

technique BasicTechnique
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS_Main();
        PixelShader = compile ps_2_0 PS_Main();
    }
}
