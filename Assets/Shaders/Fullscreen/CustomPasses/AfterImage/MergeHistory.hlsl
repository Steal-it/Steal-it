#ifndef MERGE_HISTORY
#define MERGE_HISTORY

#ifdef SHADERGRAPH_PREVIEW
#else
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#endif

void FragMergeHistory_float(float2 uv,float index, out float3 color)
{
#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
    color = SAMPLE_TEXTURE2D_ARRAY(_HistoryTex, sampler_HistoryTex, uv, index).rgb;
#else
    color = SAMPLE_TEXTURE2D(_HistoryTex, sampler_HistoryTex, uv).rgb;
#endif
}

#endif