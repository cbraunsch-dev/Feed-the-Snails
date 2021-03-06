using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The default LevelManager for all tutorial levels. This is the manager used by all tutorial levels unless otherwise specified.
 */
public class TutorialLevelManager : MonoBehaviour, LevelManager
{
    public string KeyTutorialLevel;
    private LevelManagerInteractive gameSceneManager;
    private GameStateManager gameStateManager;
    private LevelInfo levelInfo;

    // Start is called before the first frame update
    void Start()
    {
        gameSceneManager = GameObject.FindGameObjectWithTag(Tags.SceneManager).GetComponent<GameSceneManager>();
        gameStateManager = GameObject.FindGameObjectWithTag(Tags.GameStateManager).GetComponent<GameStateManager>();
        levelInfo = gameStateManager.Levels[KeyTutorialLevel];
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

    public void OnSnailDied(Snail snail)
    {
        // No Op
    }

    public void OnLevelStarted()
    {
        gameStateManager.OnLevelStarted(levelInfo);
    }
}
