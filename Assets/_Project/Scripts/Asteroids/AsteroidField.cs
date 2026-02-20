using System.Collections.Generic;
using UnityEngine;

public class AsteroidField : MonoBehaviour
{
    [SerializeField][Range(50, 1000)] private int _asteroidCount = 50;
    [SerializeField][Range(50, 1000)] private float _radius = 50f;
    [SerializeField][Range(0.5f, 5f)] private float _maxScale = 5f;
    [SerializeField] private List<GameObject> _asteroidPrefabs;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    private void OnEnable()
    {
        SpawnAsteroids();
    }

    void SpawnAsteroids()
    {
        for (int i = 0; i < _asteroidCount; i++)
        {
            GameObject asteroid = Instantiate(
                _asteroidPrefabs[Random.Range(0, _asteroidPrefabs.Count)],
                _transform.position,
                Quaternion.identity
                );
            float scale = Random.Range(0.5f, _maxScale);
            asteroid.transform.localScale = new Vector3(scale, scale, scale);
            asteroid.transform.position += Random.insideUnitSphere * _radius;
            asteroid.GetComponent<Rigidbody>()?.AddTorque(Random.insideUnitSphere * Random.Range(0f, 50f));
        }
    }
}
