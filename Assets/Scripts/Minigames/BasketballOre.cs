using UnityEngine;

public class BasketballOre : MonoBehaviour
{
    private BasketballMinigame minigame;
    private bool hasScored = false;
    private float despawnHeight = -10f;

    public AudioClip sfxHit;
    [Range(0f, 1f)] public float hitVolume = 0.7f;
    private AudioSource sfx; 

    public void Initialize(BasketballMinigame game)
    {
        minigame = game;
        hasScored = false;
    }

    void Awake()
    {
        sfx = gameObject.AddComponent<AudioSource>();
        sfx.playOnAwake = false;
        sfx.spatialBlend = 0f; // 2D for consistency in popup
    }

    void Update()
    {

        if (transform.position.y < despawnHeight)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (hasScored) return;
        if (other.collider.CompareTag("BasketballTarget") || other.collider.name.Contains("Target"))
        {
            hasScored = true;
            minigame.OnOreScored();
            Destroy(gameObject, 0.5f);
        }

        if (sfxHit && (other.collider.name.Contains("Rim") || other.collider.name.Contains("Board")))
            sfx.PlayOneShot(sfxHit, hitVolume);
    }


    void OnTriggerEnter(Collider other)
    {
        if (hasScored) return;


        if (other.CompareTag("BasketballTarget") || other.name.Contains("Target"))
        {
            hasScored = true;
            minigame.OnOreScored();


            Destroy(gameObject, 0.5f);
        }
    }
}