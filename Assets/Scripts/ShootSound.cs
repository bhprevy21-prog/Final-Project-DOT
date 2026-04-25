using UnityEngine;

public class ShootSound : MonoBehaviour
{
    public AudioSource audioSource;

    public void PlayShootSound()
    {
        AudioClip clip = CreateShootSound();
        audioSource.PlayOneShot(clip);
    }

   AudioClip CreateShootSound()
{
    int sampleRate = 44100;
    float duration = 0.15f;

    int samples = (int)(sampleRate * duration);
    float[] data = new float[samples];

    for (int i = 0; i < samples; i++)
    {
        float t = i / (float)sampleRate;

        // random noise (air movement)
        float noise = Random.Range(-1f, 1f);

        // low-frequency rumble (weight of rock)
        float lowWave = Mathf.Sin(2 * Mathf.PI * 80f * t);

        // combine both
        float sound = (noise * 0.7f) + (lowWave * 0.3f);

        // fade out quickly (whoosh shape)
        float envelope = Mathf.Exp(-20 * t);

        data[i] = sound * envelope * 0.5f;
    }

    AudioClip clip = AudioClip.Create("RockThrow", samples, 1, sampleRate, false);
    clip.SetData(data, 0);

    return clip;
}
}