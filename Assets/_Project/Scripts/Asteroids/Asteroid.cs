using UnityEngine;

public class Asteroid : MonoBehaviour, IDamageable
{
    [SerializeField]
    private FracturedAsteroid _fracturedAsteroidPrefab;

    [SerializeField]
    private Detonator _explosionPrefab;


    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    public void TakeDamage(int damage, Vector3 hitPosition)
    {
        FractureAsteroid(hitPosition);
    }

    private void FractureAsteroid(Vector3 hitPosition)
    {
        if (_fracturedAsteroidPrefab != null)
        {
            Instantiate(_fracturedAsteroidPrefab, transform.position, transform.rotation);
        }

        if (_explosionPrefab != null)
        {
            Instantiate(_explosionPrefab, hitPosition, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
