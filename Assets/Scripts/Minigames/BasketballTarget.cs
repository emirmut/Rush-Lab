using UnityEngine;

public class BasketballTarget : MonoBehaviour
{
    [Header("Visual Feedback")]
    public ParticleSystem scoreEffect;
    public AudioClip scoreSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && scoreSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {

        BasketballOre ore = other.GetComponent<BasketballOre>();
        if (ore != null)
        {

            if (scoreEffect != null)
            {
                scoreEffect.Play();
            }

            if (audioSource != null && scoreSound != null)
            {
                audioSource.PlayOneShot(scoreSound);
            }
        }
    }
}