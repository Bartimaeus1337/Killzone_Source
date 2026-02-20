using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShipEngine : MonoBehaviour
{
    [SerializeField] GameObject _thruster;
    [SerializeField] AudioClip _thrusterSound;

    IMovementControls _shipMovementControls;
    Rigidbody _rigidbody;
    float _thrustForce;
    float _thrustAmount;
    AudioSource _audioSource;

    bool ThrustersEnabled => !Mathf.Approximately(0f, _shipMovementControls.ThrustAmount);

    private void Awake()
    {
        _audioSource = SoundManager.Configure3DAudioSource(GetComponent<AudioSource>());
    }

    void Update()
    {
        ActivateThrusters();
    }

    void FixedUpdate()
    {
        if (!ThrustersEnabled) return;
        //Debug.Log($"{_rigidbody.name} Thrusting, amount: {_thrustAmount}");
        _rigidbody.AddForce(transform.forward * _thrustAmount * Time.fixedDeltaTime);
    }

    public void Init(IMovementControls movementControls, Rigidbody rb, float thrustForce)
    {
        _shipMovementControls = movementControls;
        _rigidbody = rb;
        _thrustForce = thrustForce;
    }

    void ActivateThrusters()
    {
        _thruster.SetActive(ThrustersEnabled);
        if (_thrusterSound)
        {
            _audioSource.PlayOneShot(_thrusterSound);
        }
        if (!ThrustersEnabled) return;
        _thrustAmount = _thrustForce * _shipMovementControls.ThrustAmount;
    }

}