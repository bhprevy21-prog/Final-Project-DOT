using UnityEngine;

public class ShootSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip shootClip;

    public void PlayShootSound()
    {
        if (audioSource != null && shootClip != null)
            audioSource.PlayOneShot(shootClip);
    }
}