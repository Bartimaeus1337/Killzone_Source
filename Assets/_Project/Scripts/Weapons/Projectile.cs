using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Detonator _hitEffect;
    [SerializeField] AudioClip _impactSound;
    float _launchForce;
    int _damage;
    float _range;

    float _duration;
    Rigidbody _rigidBody;
    AudioSource _audioSource;
    private Transform _target;

    bool OutOfFuel
    {
        get
        {
            _duration -= Time.deltaTime;
            return _duration <= 0f;
        }
    }

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _audioSource = SoundManager.Configure3DAudioSource(GetComponent<AudioSource>());
    }

    void OnEnable()
    {
        _rigidBody.AddForce(_launchForce * transform.forward);
        _duration = _range;
    }

    private void OnDisable()
    {
        _rigidBody.linearVelocity = Vector3.zero;
        _rigidBody.angularVelocity = Vector3.zero;
    }

    void Update()
    {
        if (OutOfFuel) Destroy(gameObject);
    }

    ///// <summary>
    ///// Automatic targeting, simply follow the target.
    ///// </summary>
    //void FixedUpdate()
    //{
    //    if (_target != null && _target.CompareTag("Enemy"))
    //    {
    //        Vector3 direction = (_target.position - transform.position).normalized;
    //        float speed = _rigidBody.linearVelocity.magnitude;
    //        _rigidBody.linearVelocity = direction * speed;
    //    }
    //}

    /// <summary>
    /// Automatic targeting, simply follow the target, more natural movement.
    /// </summary>
    void FixedUpdate()
    {
        if (_target != null && _target.CompareTag("Enemy"))
        {
            Vector3 currentDirection = _rigidBody.linearVelocity.normalized;
            Vector3 targetDirection = (_target.position - transform.position).normalized;

            // Set how fast the projectile can turn (radians per second)
            float turnRate = 2f * Mathf.Deg2Rad; // 2 degrees per physics step

            Vector3 newDirection = Vector3.RotateTowards(
                currentDirection,
                targetDirection,
                turnRate,
                0f
            );

            float speed = _rigidBody.linearVelocity.magnitude;
            _rigidBody.linearVelocity = newDirection * speed;
        }
    }

    public void Init(int launchForce, int damage, float range, Vector3 velocity, Vector3 angularVelocity, Transform target)
    {
        _launchForce = launchForce;
        _damage = damage;
        _range = range;
        _rigidBody.linearVelocity = velocity;
        _rigidBody.angularVelocity = angularVelocity;
        _target = target;
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (_impactSound) _audioSource.PlayOneShot(_impactSound);
    //    Debug.Log($"projectile collided with: {collision.collider.name}");
    //    IDamageable damageable = collision.collider.gameObject.GetComponent<IDamageable>();
    //    if (damageable != null)
    //    {
    //        Debug.Log($"Hit Game Object: {gameObject.name}");
    //        Vector3 hitPosition = collision.GetContact(0).point;
    //        damageable.TakeDamage(_damage, hitPosition);

    //        if (_hitEffect != null)
    //        {
    //            Instantiate(_hitEffect, transform.position, Quaternion.identity);
    //        }
    //        Destroy(gameObject);
    //    }

    //    //if (_hitEffect != null)
    //    //{
    //    //    Instantiate(_hitEffect, transform.position, Quaternion.identity);
    //    //}
    //    //Destroy(gameObject);
    //}

    //void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log($"projectile collided with: {collision.collider.name}");
    //    IDamageable damageable = collision.collider.gameObject.GetComponent<IDamageable>();
    //    if (damageable != null)
    //    {
    //        Vector3 hitPosition = collision.GetContact(0).point;
    //        damageable.TakeDamage(_damage, hitPosition);
    //    }

    //    if (_hitEffect != null)
    //    {
    //        Instantiate(_hitEffect, transform.position, Quaternion.identity);
    //    }
    //    Destroy(gameObject);
    //}

    private int i = 0;
    private void OnTriggerEnter(Collider collision)
    {
        i++;
        Debug.Log($"projectile collided with: {collision.name}. {i}x Times");
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Vector3 hitPosition = collision.ClosestPoint(transform.position);
            damageable.TakeDamage(_damage, hitPosition);
        }

        if (_hitEffect != null)
        {
            Instantiate(_hitEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}