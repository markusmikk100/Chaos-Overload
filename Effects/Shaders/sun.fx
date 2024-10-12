sampler uImage0 : register(s0);
sampler uImage1 : register(s1); // Automatically Images/Misc/Perlin via Force Shader testing option
sampler uImage2 : register(s2); // Automatically Images/Misc/noise via Force Shader testing option
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
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

    float d = sdCircle(uv, 0.1 * length(uZoom));
    float4 color = lerp(float4(1, 0.7, 0, 1), sampleColor, smoothstep(1.4, 3.2, d + 2.5));
    color = lerp(float4(1, 0.6, 0, 1), color, smoothstep(0.06, 0.12, d + 0.05));
    color = lerp(float4(1, 1, 1, 1), color, smoothstep(0, 0.05, d));

    float2 noiseUV = coords * float2(0.25, 1) + float2(uTime * 0.04, 0);
    noiseUV += uv * d * 0.5;
    float noise = tex2D(uImage1, noiseUV).x;
    float4 noiseColor = lerp(float4(1, 0.6, 0, 1), float4(1, 1, 1, 1), smoothstep(0, 0.5, noise));
    noiseColor *= 1.05;

    color = lerp(noiseColor, color, smoothstep(0, 0.01, d + 0.01));
    color = lerp(float4(1, 1, 1, 1), color, smoothstep(0, 0.04, abs(d)));

    return color;
}

technique Technique1
{
    pass ModdersToolkitShaderPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}