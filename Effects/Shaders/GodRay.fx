sampler uImage0 : register(s0);
float2 uScreenResolution;

float count;
float2 origins[10];
float2 targets[10];

float Segment(float2 p, float2 a, float2 b) 
{
    float2 pa = p - a;
    float2 ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h);
}

float4 PSMain(float2 coord : TEXCOORD0) : COLOR0
{
    float4 sampleColor = tex2D(uImage0, coord);

    float2 uv = float2(coord.x, 1 - coord.y) * 2 - 1;
    uv.x *= uScreenResolution.x / uScreenResolution.y;

    float d = 999;
    
    for (int i = 0; i < count; i++) {
        d = min(d, Segment(uv, origins[i], targets[i]) + 0.48);
    }
    
    float4 color = d < 0.5 ? float4(0.98, 0.98, 0.9, 1) : sampleColor;
    color = lerp(float4(0.98, 0.98, 0.9, 1), color, smoothstep(0, 0.05, abs(d) - 0.49));

    return color;
}

technique Technique1
{
    pass P0
    {
        PixelShader = compile ps_3_0 PSMain();
    }
}