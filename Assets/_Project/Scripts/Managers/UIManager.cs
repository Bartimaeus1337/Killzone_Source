using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] TargetIndicator _targetIndicatorPrefab;

    [SerializeField] Canvas _mainCanvas;
    [SerializeField] TMP_Text _scoreText;
    [SerializeField] TMP_Text _levelText;
    [SerializeField] GameObject _gameOverScreen;
    [SerializeField] GameObject _winScreen;
    [SerializeField] EnemyShipManager _enemyShipManager;
    private int _score;
    List<TargetIndicator> _targetIndicators;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _targetIndicators = new List<TargetIndicator>();
    }

    public void AddTarget(Transform target)
    {
        var targetIndicator = Instantiate(_targetIndicatorPrefab, _mainCanvas.transform);
        targetIndicator.Init(target, _mainCanvas);
        _targetIndicators.Add(targetIndicator);
    }

    public void RemoveTarget(Transform target)
    {
        var key = target.GetInstanceID();
        var indicator = _targetIndicators.FirstOrDefault(i => i.Key == key);
        if (indicator)
        {
            _targetIndicators.Remove(indicator);
            Destroy(indicator.gameObject);
        }
    }

    public void UpdateTargetIndicators(List<Transform> targets, int lockedOnTarget)
    {
        foreach (var targetIndicator in _targetIndicators)
        {
            targetIndicator.gameObject.SetActive(targets.Any(target => target.GetInstanceID() == targetIndicator.Key));
            targetIndicator.LockedOn = targetIndicator.Key == lockedOnTarget;
        }
    }

    void OnEnable()
    {
        SubscribeToEvents();
        _gameOverScreen.SetActive(false);
        _winScreen.SetActive(false);
    }

    void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    void Start()
    {
        SubscribeToEvents();
    }

    void SubscribeToEvents()
    {
        SubscribeToScoreManagerEvents();
        SubscribeToGameManagerEvents();
        SubscribeToEnemyShipManagerEvents();
    }

    private void SubscribeToEnemyShipManagerEvents()
    {
        _enemyShipManager.TotalEnemiesSpawnedChanged += EnemySpawnedShipManagerOnTotalEnemiesSpawnedChanged;
    }

    private void EnemySpawnedShipManagerOnTotalEnemiesSpawnedChanged(int totalEnemiesSpawned)
    {
        _scoreText.text = $"Kill score: {_score.ToString()} / {_enemyShipManager.TotalEnemiesSpawned + 1}";
    }

    void SubscribeToGameManagerEvents()
    {
        //if (!GameManager.Instance) return;
        GameManager.Instance.GameStateChanged += OnGameStateChanged;
    }

    void UnsubscribeFromEvents()
    {
        UnsubscribeFromScoreManagerEvents();
        UnsubscribeToGameManagerEvents();
    }

    void UnsubscribeToGameManagerEvents()
    {
        //if (!GameManager.Instance) return;
        GameManager.Instance.GameStateChanged -= OnGameStateChanged;
    }

    void SubscribeToScoreManagerEvents()
    {
        if (!ScoreManager.Instance) return;
        UnsubscribeFromScoreManagerEvents();
        ScoreManager.Instance.ScoreChanged += OnScoreChanged;
        ScoreManager.Instance.LevelChanged += OnLevelChanged;
    }

    private void OnLevelChanged(int level)
    {
        _levelText.text = $"Level {level.ToString()}";
    }

    void UnsubscribeFromScoreManagerEvents()
    {
        if (!ScoreManager.Instance) return;
        ScoreManager.Instance.ScoreChanged -= OnScoreChanged;
        ScoreManager.Instance.LevelChanged -= OnLevelChanged;
    }

    void OnScoreChanged(int score)
    {
        _score = score;
        _scoreText.text = $"Kill score: {score.ToString()} / {_enemyShipManager.TotalEnemiesSpawned + 1}";
    }

    void OnGameStateChanged(GameState state)
    {
        _gameOverScreen.SetActive(state == GameState.GameOver);
        _winScreen.SetActive(state == GameState.Win);
    }

}