using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardenLevelManager : MonoBehaviour, LevelManager
{
    private LevelManagerInteractive gameSceneManager;
    private GameStateManager gameStateManager;
    private LevelInfo levelInfo;

    // Start is called before the first frame update
    void Start()
    {
        gameSceneManager = GameObject.FindGameObjectWithTag(Tags.SceneManager).GetComponent<GameSceneManager>();
        gameStateManager = GameObject.FindGameObjectWithTag(Tags.GameStateManager).GetComponent<GameStateManager>();
        levelInfo = gameStateManager.Levels[GameStateManager.KeyCastleLevel];
        gameSceneManager.LevelManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLevelFinished()
    {
        gameStateManager.OnLevelFinished(levelInfo);
    }

    public void OnLevelStarted()
    {
        gameStateManager.OnLevelStarted(levelInfo);
    }

    public void OnSnailDied(Snail snail)
    {
        // No op
    }
}
