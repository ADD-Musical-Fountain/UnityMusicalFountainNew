using UnityEngine;
public static class Constants
{
    public const float MUSIC_TIME = 223.0f;
    
    public static ParticleSystem.MinMaxCurve getParticleLifeTime()
    {
        return new ParticleSystem.MinMaxCurve(0.5f, 1.5f);
    }

    public static ParticleSystem.MinMaxCurve getParticleSize()
    {
        return new ParticleSystem.MinMaxCurve(0.25f, 0.75f);
    }

    public static ParticleSystem.MinMaxCurve getParticleSpeed()
    {
        return new ParticleSystem.MinMaxCurve(0.4f, 1.2f);
    }

}
