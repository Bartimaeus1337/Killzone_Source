using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public event Action<int> ScoreChanged = delegate (int i) { };
    public event Action<int> LevelChanged = delegate (int i) { };
    public int Level { get; private set; } = 1;

    public int Score { get; private set; }

    public void ResetScore()
    {
        Score = 0;
        ScoreChanged(Score);
    }

    public void AddPoints(int points)
    {
        Score += points;
        ScoreChanged(Score);
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void LevelUp()
    {
        Level++;
        LevelChanged(Level);
    }
}