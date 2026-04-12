using UnityEngine;

public static class ProceduralAudio
{
    const int sampleRate = 44100;

    public static AudioClip CreateMenuBGM()
    {
        float duration = 16f;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];

        float[] notes = { 220f, 261.63f, 293.66f, 329.63f, 261.63f, 293.66f, 220f, 246.94f };
        float noteLen = duration / notes.Length;

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            int noteIdx = Mathf.Min((int)(t / noteLen), notes.Length - 1);
            float freq = notes[noteIdx];

            float noteT = (t - noteIdx * noteLen) / noteLen;
            float env = Mathf.Sin(noteT * Mathf.PI);

            float val = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.3f;
            val += Mathf.Sin(2f * Mathf.PI * freq * 2f * t) * 0.1f;
            val += Mathf.Sin(2f * Mathf.PI * freq * 0.5f * t) * 0.15f;

            float pad = Mathf.Sin(2f * Mathf.PI * 110f * t) * 0.08f;
            pad += Mathf.Sin(2f * Mathf.PI * 82.41f * t) * 0.06f;

            data[i] = (val * env + pad) * 0.5f;
        }

        return CreateClip("MenuBGM", data, duration);
    }

    public static AudioClip CreateLevelBGM(int level)
    {
        float duration = 8f;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];

        float bassBPM = level == 1 ? 100f : 130f;
        float beatInterval = 60f / bassBPM;

        float[] melody1 = { 329.63f, 392f, 440f, 392f, 349.23f, 329.63f, 293.66f, 329.63f };
        float[] melody2 = { 392f, 440f, 523.25f, 493.88f, 440f, 392f, 349.23f, 392f };
        float[] melody = level == 1 ? melody1 : melody2;
        float noteLen = duration / melody.Length;

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;

            float beatPhase = (t % beatInterval) / beatInterval;
            float kick = 0f;
            if (beatPhase < 0.15f)
            {
                float kickFreq = Mathf.Lerp(150f, 50f, beatPhase / 0.15f);
                kick = Mathf.Sin(2f * Mathf.PI * kickFreq * t) * (1f - beatPhase / 0.15f) * 0.3f;
            }

            float hihatPhase = (t % (beatInterval * 0.5f)) / (beatInterval * 0.5f);
            float hihat = 0f;
            if (hihatPhase < 0.05f)
            {
                hihat = (Random.Range(-1f, 1f)) * (1f - hihatPhase / 0.05f) * 0.08f;
            }

            int noteIdx = Mathf.Min((int)(t / noteLen), melody.Length - 1);
            float freq = melody[noteIdx];
            float noteT = (t - noteIdx * noteLen) / noteLen;
            float melEnv = noteT < 0.1f ? noteT / 0.1f : Mathf.Max(0, 1f - (noteT - 0.1f) * 1.5f);

            float mel = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.15f;
            mel += Mathf.Sin(2f * Mathf.PI * freq * 2f * t) * 0.05f;

            float bassFreq = level == 1 ? 82.41f : 98f;
            float bass = Mathf.Sin(2f * Mathf.PI * bassFreq * t) * 0.12f;

            data[i] = Mathf.Clamp((kick + hihat + mel * melEnv + bass) * 0.7f, -1f, 1f);
        }

        return CreateClip("Level" + level + "BGM", data, duration);
    }

    public static AudioClip CreateShootSFX()
    {
        float duration = 0.15f;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float env = 1f - t / duration;
            float freq = Mathf.Lerp(800f, 200f, t / duration);
            data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * env * 0.4f;
        }

        return CreateClip("ShootSFX", data, duration);
    }

    public static AudioClip CreateHitSFX()
    {
        float duration = 0.2f;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float env = 1f - t / duration;
            env *= env;
            float val = Mathf.Sin(2f * Mathf.PI * 300f * t) * 0.3f;
            val += Mathf.Sin(2f * Mathf.PI * 150f * t) * 0.2f;
            data[i] = val * env;
        }

        return CreateClip("HitSFX", data, duration);
    }

    public static AudioClip CreateExplosionSFX()
    {
        float duration = 0.5f;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];

        System.Random rng = new System.Random(42);
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float env = Mathf.Pow(1f - t / duration, 2f);
            float noise = (float)(rng.NextDouble() * 2.0 - 1.0);
            float boom = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(100f, 30f, t / duration) * t);
            data[i] = (noise * 0.3f + boom * 0.4f) * env;
        }

        return CreateClip("ExplosionSFX", data, duration);
    }

    public static AudioClip CreatePickupSFX()
    {
        float duration = 0.25f;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float env = t < 0.05f ? t / 0.05f : 1f - (t - 0.05f) / (duration - 0.05f);
            float freq = Mathf.Lerp(500f, 1200f, t / duration);
            data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * env * 0.35f;
        }

        return CreateClip("PickupSFX", data, duration);
    }

    public static AudioClip CreateHurtSFX()
    {
        float duration = 0.3f;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float env = 1f - t / duration;
            float freq = Mathf.Lerp(400f, 100f, t / duration);
            float val = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.3f;
            val += Mathf.Sin(2f * Mathf.PI * freq * 1.5f * t) * 0.15f;
            data[i] = val * env;
        }

        return CreateClip("HurtSFX", data, duration);
    }

    public static AudioClip CreateVictorySFX()
    {
        float duration = 1.2f;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];

        float[] notes = { 523.25f, 659.25f, 783.99f, 1046.5f };
        float noteLen = duration / notes.Length;

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            int noteIdx = Mathf.Min((int)(t / noteLen), notes.Length - 1);
            float freq = notes[noteIdx];
            float noteT = (t - noteIdx * noteLen) / noteLen;
            float env = noteT < 0.1f ? noteT / 0.1f : Mathf.Max(0, 1f - (noteT - 0.1f) * 1.2f);
            float val = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.3f;
            val += Mathf.Sin(2f * Mathf.PI * freq * 2f * t) * 0.1f;
            data[i] = val * env;
        }

        return CreateClip("VictorySFX", data, duration);
    }

    public static AudioClip CreateDefeatSFX()
    {
        float duration = 1f;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];

        float[] notes = { 392f, 349.23f, 293.66f, 196f };
        float noteLen = duration / notes.Length;

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            int noteIdx = Mathf.Min((int)(t / noteLen), notes.Length - 1);
            float freq = notes[noteIdx];
            float noteT = (t - noteIdx * noteLen) / noteLen;
            float env = Mathf.Max(0, 1f - noteT * 0.8f);
            float val = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.25f;
            val += Mathf.Sin(2f * Mathf.PI * freq * 0.5f * t) * 0.15f;
            data[i] = val * env;
        }

        return CreateClip("DefeatSFX", data, duration);
    }

    public static AudioClip CreateButtonClickSFX()
    {
        float duration = 0.08f;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float env = 1f - t / duration;
            data[i] = Mathf.Sin(2f * Mathf.PI * 1000f * t) * env * env * 0.3f;
        }

        return CreateClip("ClickSFX", data, duration);
    }

    static AudioClip CreateClip(string name, float[] data, float duration)
    {
        AudioClip clip = AudioClip.Create(name, data.Length, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
