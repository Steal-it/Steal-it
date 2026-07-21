using System;

using UnityEngine.Rendering;

[Serializable]
[VolumeComponentMenu("Custom/AfterImageEffect")]
public class AfterImageEffect : VolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0, 0, 1, true);

    public bool IsActive()
    {
        return intensity.value > 0;
    }

    public float GetIntensity()
    {
        return intensity.value;
    }
}
