using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public event Action<GameState> GameStateChanged = delegate (GameState state) { };

    //[SerializeField]
    //GameObject[] _enemyShips;

    [SerializeField]
    GameObject _playerShip;

    [SerializeField]
    EnemyShipManager _enemyShipManager;

    private int defaultMaxEnemies;

    public GameState GameState { get; private set; }

    bool ShouldQuitGame => Input.GetKeyUp(KeyCode.Escape);

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        defaultMaxEnemies = _enemyShipManager.MaxEnemies;
    }

    void Start()
    {
        GameState = GameState.Combat;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        MusicManager.Instance.PlaySpaceMusic();
    }

    void SetGameState(GameState gameState)
    {
        if (gameState == GameState) return;
        GameState = gameState;
        GameStateChanged(gameState);
    }

    void Update()
    {
        if (_enemyShipManager.ActiveEnemies == 0 && _enemyShipManager.TotalEnemiesSpawned == _enemyShipManager.MaxEnemies)
        {
            PlayerWon();
        }
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            PlayerLost();
        }

        if (GameState == GameState.GameOver)
        {
            HandleGameOverInput();
            return;
        }

        if (GameState == GameState.Win)
        {
            HandleWinInput();
            return;
        }

        if (ShouldQuitGame)
        {
            QuitGame();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            Time.timeScale = 0f;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Confined;
        }
    }

    private void HandleWinInput()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            PlayHigherLevel();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            QuitGame();
        }
    }

    private void PlayHigherLevel()
    {
        ScoreManager.Instance.LevelUp();
        _enemyShipManager.IncreaseDifficulty(ScoreManager.Instance.Level);
        _enemyShipManager.ResetActiveEnemies();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void HandleGameOverInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            _enemyShipManager.MaxEnemies = defaultMaxEnemies; // reset level
            PlayAgain();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            QuitGame();
        }
    }

    void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlayerWon()
    {
        MusicManager.Instance.PlayGameOverMusic();
        SetGameState(GameState.Win);
    }

    public void PlayerLost()
    {
        MusicManager.Instance.PlayGameOverMusic();
        SetGameState(GameState.GameOver);
    }


    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // todo handle WebGL
        Application.Quit();
#endif
    }
}