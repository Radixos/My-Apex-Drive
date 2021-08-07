[System.Serializable]
public class CameraShake
{
    public float Strength;
    public float Duration;
    public float TargetStrength;
    public float SmoothInDuration;
    public float SmoothOutDuration;

    public CameraShake(float strength, float duration, float smoothInDuration = 0.0f, float smoothOutDuration = 0.0f)
    {
        TargetStrength = strength;
        Duration = duration;

        SmoothInDuration = smoothInDuration;
        SmoothOutDuration = smoothOutDuration;

        if(smoothInDuration > 0) Strength = 0.0f;
        else Strength = strength;
    }
}
