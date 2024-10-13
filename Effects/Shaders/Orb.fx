sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

float3 uColor;
float uOpacity;
float2 uScreenResolution;
float2 uScreenPosition;
float uTime;
float uIntensity;
float2 uZoom;

float sdCircle(float2 p, float r)
{
    return length(p) - r;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 sampleColor = tex2D(uImage0, coords);

    float2 uv = float2(coords.x, 1 - coords.y) * 2 - 1;
    uv.x *= uScreenResolution.x / uScreenResolution.y;

    float radius = 0.25f * uIntensity * length(uZoom);
    float2 dOffset = float2(uv.x * 0.25f + uTime * 0.03f, uv.y * 0.25f);
    float d = sdCircle(uv, radius) + tex2D(uImage1, dOffset).x * 0.04f;

    float4 color = lerp(float4(uColor, 1), sampleColor, smoothstep(1.4 + 0.6 * uIntensity, 3.2 + 0.4 * uIntensity, d + 2.5));
    color = lerp(float4(uColor, 1), color, smoothstep(0.06 + 0.02 * uIntensity, 0.12 + 0.04 * uIntensity, d + 0.05));
    color = lerp(float4(1, 1, 1, 1), color, smoothstep(0, 0.05, d));
    
    float2 noiseUV = uv / radius;
    float y = asin(noiseUV.y);
    float x = asin(noiseUV.x / cos(y)) + uTime;
    noiseUV = float2(x, y);

    float noise = tex2D(uImage1, noiseUV * float2(0.03, 0.1)).x;
    float4 noiseColor = lerp(float4(uColor, 1), float4(1, 1, 1, 1), smoothstep(0, 0.5, noise));
    noiseColor *= 1.05;

    color = lerp(noiseColor, color, smoothstep(0, 0.01, d + 0.01));
    color = lerp(float4(1, 1, 1, 1), color, smoothstep(0, 0.04 + 0.06 * uIntensity, abs(d + 0.012 * uIntensity)));

    color = lerp(sampleColor, color, uOpacity);

    return color;
}

technique Technique1
{
    pass ModdersToolkitShaderPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}