using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This module toggles between multiple LevelSelectorModules depending on
 * how far along the player's progress in the game is.
 */
public class LevelSelectionModule : MonoBehaviour, GameStateManagerObserver
{
    private GameStateManager gameStateManager;
    private const int MaxNumberOfLevelsPerSelector = 6;
    public GameObject tutorialLevelSelector;
    public GameObject realLevelSelector;
    public GameObject otherCamera;  // A third camera that does not point to the level selectors (e.g. the menu camera)
    public GameObject tutorialLevelSelectCamera;
    public GameObject realLevelSelectCamera;

    // Start is called before the first frame update
    void Start()
    {
        gameStateManager = GameObject.FindGameObjectWithTag(Tags.GameStateManager).GetComponent<GameStateManager>();
        gameStateManager.Observers.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**
     * Returns true if the player's progress is on the set of real levels.
     *
     * NB: Currently we keep things simple and don't provide a generic solution that
     * will scale across multiple pages of real levels. Currently we just detect
     * whether we are on the tutorial page or the page of real levels.
     */
    public bool HasPlayerProgressReachedRealLevels()
    {
        var levelIndex = IndexOfLastOpenLevel();
        return levelIndex >= MaxNumberOfLevelsPerSelector;
    }

    private int IndexOfLastOpenLevel()
    {
        // Start at first level and walk along levels until we reach the first locked level
        var level = gameStateManager.Levels[GameStateManager.KeyTutorialLevel1];
        var levelIndex = -1;
        while (level != null && !level.IsLocked)
        {
            levelIndex++;
            level = level.NextLevel;
        }
        return levelIndex;
    }

    public void OnGameLoaded()
    {
        // Show tutorial level selector or real level selector depending on progress
        if(HasPlayerProgressReachedRealLevels())
        {
            ShowRealLevels();
        }
        else
        {
            ShowTutorialLevels();
        }
    }

    public void ShowRealLevels()
    {
        tutorialLevelSelector.SetActive(false);
        realLevelSelector.SetActive(true);
        otherCamera.SetActive(false);
        realLevelSelectCamera.SetActive(true);
    }

    private void ShowTutorialLevels()
    {
        tutorialLevelSelector.SetActive(true);
        realLevelSelector.SetActive(false);
        otherCamera.SetActive(false);
        tutorialLevelSelectCamera.SetActive(true);
    }

    public void OnLevelFinished()
    {
        // No-op
    }

    public void OnNewGameStarted()
    {
        ShowTutorialLevels();
    }
}
