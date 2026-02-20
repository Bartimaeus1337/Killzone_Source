using UnityEngine;
using UnityEngine.Events;

public class DamageHandler : MonoBehaviour, IDamageable
{
    [SerializeField] GameObject _explosionPrefab;
    UnityEvent _healthChangedEvent;
    UnityEvent _objectDestroyedEvent;
    public int MaxHealth { get; private set; }
    public int Health { get; private set; }

    public UnityEvent HealthChanged => _healthChangedEvent ??= new UnityEvent();
    public UnityEvent ObjectDestroyed => _objectDestroyedEvent ??= new UnityEvent();

    public void Init(int maxHealth)
    {
        Health = MaxHealth = maxHealth;
        HealthChanged.Invoke();
    }

    public void TakeDamage(int damage, Vector3 hitPosition)
    {
        Health -= damage;
        HealthChanged.Invoke();
        Debug.Log($"Health changed: {Health}");
        if (Health > 0) return;
        if (_explosionPrefab)
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        }
        ObjectDestroyed.Invoke();
    }

    ///// <summary>
    ///// Explosion of the object when object takes fatal damage, for example by collision with the ground.
    ///// </summary>
    //public void Explode()
    //{
    //    if (_explosionPrefab)
    //    {
    //        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
    //    }
    //    ObjectDestroyed.Invoke();
    //}


}