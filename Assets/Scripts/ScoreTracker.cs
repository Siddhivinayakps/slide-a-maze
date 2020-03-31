using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTracker : MonoBehaviour
{

    public static ScoreTracker Instance;
    public int currentLevel;
    public int CurrentLevel
    {
        get
        {
            return currentLevel;
        }
        set
        {
            currentLevel = value;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        }
    }


    private void Awake()
    {
        Instance = this;
        if (!PlayerPrefs.HasKey("CurrentLevel")) { PlayerPrefs.SetInt("CurrentLevel", 1); }
        currentLevel = PlayerPrefs.GetInt("CurrentLevel");
    }
}
