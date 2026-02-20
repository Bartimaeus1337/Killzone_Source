using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [SerializeField]
    Shield _shield;

    [SerializeField]
    protected MovementControlsBase _movementControls;

    [SerializeField]
    protected WeaponControlsBase _weaponControls;

    [SerializeField]
    ShipDataSo _shipData;

    [SerializeField]
    List<ShipEngine> _engines;

    [SerializeField]
    List<Blaster> _blasters;

    Rigidbody _rigidBody;
    [Range(-1f, 1f)]
    float _pitchAmount, _rollAmount, _yawAmount = 0f;

    protected DamageHandler _damageHandler;


    IMovementControls MovementInput => _movementControls;
    IWeaponControls WeaponInput => _weaponControls;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _damageHandler = GetComponent<DamageHandler>();
    }
    void Start()
    {
        foreach (ShipEngine engine in _engines)
        {
            engine.Init(MovementInput, _rigidBody, _shipData.ThrustForce / _engines.Count);
        }

        foreach (Blaster blaster in _blasters)
        {
            blaster.Init(WeaponInput, _shipData.BlasterCooldown, _shipData.BlasterLaunchForce, _shipData.BlasterProjectileDuration, _shipData.BlasterDamage, _rigidBody);
        }

        if (_shield)
        {
            _shield.Init(_shipData.ShieldStrength);
        }
    }

    public virtual void OnEnable()
    {
        if (_damageHandler == null) return;
        _damageHandler.Init(_shipData.MaxHealth);
        _damageHandler.HealthChanged.AddListener(OnHealthChanged);
        _damageHandler.ObjectDestroyed.AddListener(DestroyShip);
    }

    public virtual void Update()
    {
        _rollAmount = MovementInput.RollAmount;
        _yawAmount = MovementInput.YawAmount;
        _pitchAmount = MovementInput.PitchAmount;
    }

    void FixedUpdate()
    {
        if (!Mathf.Approximately(0f, _pitchAmount))
        {
            _rigidBody.AddTorque(transform.right * (_shipData.PitchForce * _pitchAmount * Time.fixedDeltaTime));
        }

        if (!Mathf.Approximately(0f, _rollAmount))
        {
            _rigidBody.AddTorque(transform.forward * (_shipData.RollForce * _rollAmount * Time.fixedDeltaTime));
        }

        if (!Mathf.Approximately(0f, _yawAmount))
        {
            _rigidBody.AddTorque(transform.up * (_yawAmount * _shipData.YawForce * Time.fixedDeltaTime));
        }
    }

    void DestroyShip()
    {
        //Debug.Log("Ship destroyed.");
        gameObject.SetActive(false);
    }

    void OnHealthChanged()
    {
        //Debug.Log($"{gameObject.name} health is {_damageHandler.Health}/{_damageHandler.MaxHealth}");
    }

    //private void OnCollisionEnter(Collision other)
    //{
    //    if (_damageHandler == null) return;
    //    Debug.Log($"{gameObject.name} collided with {other.gameObject.name} and exploded");
    //    //gameObject.SetActive(false);
    //    _damageHandler.ObjectDestroyed.Invoke();
    //}

    private int i = 0;
    private void OnCollisionEnter(Collision other)
    {
        if (CompareTag("Player"))
        {
            i++;
            Debug.Log($"Player got hit {i} times");
        }
    }
}