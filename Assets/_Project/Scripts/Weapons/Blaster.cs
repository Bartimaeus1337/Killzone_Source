using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Blaster : MonoBehaviour
{
    [SerializeField] Projectile _projectilePrefab;
    [SerializeField] AudioClip _fireSound;
    [SerializeField] Transform _front;

    float _coolDownTime;
    int _launchForce, _damage;
    Rigidbody _rigidBody;
    float _duration;
    IWeaponControls _weaponInput;
    AudioSource _audioSource;
    private int i = 0;
    RadarScreen _radarScreen;

    void Awake()
    {
        _audioSource = SoundManager.Configure3DAudioSource(GetComponent<AudioSource>());
        _radarScreen = FindObjectOfType<RadarScreen>();
    }

    bool CanFire
    {
        get
        {
            _coolDown -= Time.deltaTime;
            return _coolDown <= 0f;
        }
    }

    float _coolDown;

    // Update is called once per frame
    void Update()
    {
        if (_weaponInput == null)
        {
            Debug.Log("Enemy tried to shoot but could not.");
        }
        if (_weaponInput == null) return;
        if (CanFire && _weaponInput.PrimaryFired)
        {
            //Debug.Log("Enemy is shooting");
            FireProjectile();
        }
        //else
        //{
        //    i++;
        //    Debug.Log($"Enemy could not fire, {i}x");
        //}
    }

    public void Init(IWeaponControls weaponInput, float coolDown, int launchForce, float duration, int damage, Rigidbody rigidBody)
    {
        //Debug.Log($"Blaster.Init({weaponInput}, {coolDown}, launchForce, {duration}");
        _weaponInput = weaponInput;
        _coolDownTime = coolDown;
        _launchForce = launchForce;
        _duration = duration;
        _damage = damage;
        _rigidBody = rigidBody;
    }

    void FireProjectile()
    {
        if (_fireSound)
        {
            _audioSource.PlayOneShot(_fireSound);
        }
        _coolDown = _coolDownTime;
        Projectile projectile = Instantiate(_projectilePrefab, _front.position, transform.rotation);
        projectile.gameObject.SetActive(false);
        projectile.Init(_launchForce, _damage, _duration, _rigidBody.linearVelocity, _rigidBody.angularVelocity, _radarScreen.LockedOnTarget);
        projectile.gameObject.SetActive(true);
    }

}