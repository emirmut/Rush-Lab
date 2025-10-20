using System;
using System.Collections;
using UnityEngine;

public class Ore : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private float _moveSpeed;
    private Vector3 _direction;

    [Header("Explosion")]
    public Coroutine ExplosionCoroutine { get; set; }
    [SerializeField] public float _explodeAfterSeconds;
    [SerializeField] private float _explosionDamage; // this value could vary from ore to ore
    [SerializeField] private float _explosionVFXDuration;
    public Action OreExploded { get; set; }
    public float SecondsRemainingToExplosion { get; set; }
    private bool _isCoroutineRunning;

    private AudioSource _audioSource;

    [SerializeField] private Vector3 _orePositionOffset;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        OreExploded = OreExplode;
    }

    private void Start()
    {
        _direction = transform.forward;
        _isCoroutineRunning = false;
    }

    private void Update()
    {
        if (_isCoroutineRunning)
        {
            SecondsRemainingToExplosion -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Intersection"))
        {
            _direction = transform.right;
        }

        if (other.gameObject.CompareTag("TerminatePoint"))
        {
            _moveSpeed = 0f;
            _rb.linearVelocity = Vector3.zero;
            _rb.isKinematic = true;
            ExplosionCoroutine = StartCoroutine(Explode(_explodeAfterSeconds));
        }
    }

    private void FixedUpdate()
    {
        if (_moveSpeed > 0f)
        {
            MoveOreOnConveyor();
        }
    }

    private IEnumerator Explode(float duration)
    {
        SecondsRemainingToExplosion = duration;
        _isCoroutineRunning = true;
        yield return new WaitForSeconds(duration);

        _isCoroutineRunning = false;
        Player.Instance.UpdateHealth(-_explosionDamage);
        PlayExplosionSFX();
        ExplosionVFX();
    }

    private void ExplosionVFX()
    {
        // patlama efekti olacaksa buraya koyacagız. Patlama efektini animator'le mi yaparız?
        OreExploded.Invoke();
        Destroy(gameObject, _explosionVFXDuration);
    }

    private void PlayExplosionSFX()
    {
        _audioSource.Play();
    }

    public void AttachOreToPlayer(Transform parentTransform)
    {
        transform.localPosition = Vector3.zero;
        transform.SetParent(parentTransform);
        transform.localPosition = transform.position + _orePositionOffset;
    }

    private void MoveOreOnConveyor()
    {
        _rb.linearVelocity = _moveSpeed * _direction + Physics.gravity;
    }

    public void DestroyOre()
    {
        Destroy(gameObject);
    }

    private void OreExplode()
    {
        
    }
}
