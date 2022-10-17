using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static Action<int> OnStackMatched;
    public static void StackMatched(int matchCount)
    {
        OnStackMatched?.Invoke(matchCount);
    }

    public static Action OnGameOver;
    public static void GameOver()
    {
        OnGameOver?.Invoke();
    }
    
    public static Action OnLevelCompleted;
    public static void LevelCompleted()
    {
        OnLevelCompleted?.Invoke();
    }
    
    public static Action OnNextLevelStarting;
    public static void NextLevelStarting()
    {
        OnNextLevelStarting?.Invoke();
    }


}
