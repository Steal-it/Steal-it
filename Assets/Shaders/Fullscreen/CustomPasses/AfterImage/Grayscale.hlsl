// MyCustomFunctions.hlsl

void Grayscale_float(float3 InColor, out float4 OutVector)
{
    // Calculate luminance (the grayscale value)
    float lum = 0.199 * InColor.r + 0.587 * InColor.g + 0.114 * InColor.b;

    // Pack it into a float4 (RGBA)
    // Here we set RGB to the luminance value and Alpha to 1.0
    OutVector = float4(lum, lum, lum, 1.0);
}