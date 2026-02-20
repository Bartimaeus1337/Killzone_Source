using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] AudioMixerSnapshot _spaceMusicSnapshot;
    [SerializeField] AudioMixerSnapshot _gameOverSnapshot;
    [SerializeField] AudioClip[] _spaceMusic;
    [SerializeField] AudioSource _spaceMusicAudioSource;
    [SerializeField] AudioSource _gameOverAudioSource;

    int _patrolMusicIndex, _spaceMusicIndex;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PlaySpaceMusic()
    {
        _spaceMusicAudioSource.Stop();
        _spaceMusicAudioSource.clip = _spaceMusic[_spaceMusicIndex];
        _spaceMusicAudioSource.Play();
        _spaceMusicSnapshot.TransitionTo(1f);
        _spaceMusicIndex = (_spaceMusicIndex + 1) % _spaceMusic.Length;
    }

    public void PlayGameOverMusic()
    {
        if (_gameOverSnapshot != null)
        {
            _gameOverAudioSource.Play();
            _gameOverSnapshot.TransitionTo(1f);
        }

    }
}