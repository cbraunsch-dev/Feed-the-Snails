using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelector : MonoBehaviour, GenericInputDetectionListener, GameStateManagerObserver
{
    public string AssociatedLevel;
    public bool AssociatedLevelIsTutorialLevel = false;
    public string LevelKey;
    private GameStateManager gameStateManager;
    private InputDetector inputDetector;
    private TextMeshPro unlockedSelectorText;
    private GameObject unlockedNote;
    private LevelInfo levelInfo;
    private Transform unlockedIndicator;
    private Transform lockedIndicator;

    // Start is called before the first frame update
    void Start()
    {
        inputDetector = InputDetectorFactory.CreateDetector();
        inputDetector.GenericInputDetectionListener = this;
        gameStateManager = GameObject.FindGameObjectWithTag(Tags.GameStateManager).GetComponent<GameStateManager>();
        gameStateManager.Observers.Add(this);
        levelInfo = gameStateManager.Levels[LevelKey];
        unlockedIndicator = transform.Find("UnlockedIndicator");
        lockedIndicator = transform.Find("LockedIndicator");
        unlockedSelectorText = unlockedIndicator.Find("Text (TMP)").GetComponent<TextMeshPro>();
        unlockedNote = unlockedIndicator.Find("Note").gameObject;
        UpdateLevelSelectorDependingOnLockedState();
        UpdateLevelSelectorDependingOnCompletedObjectives();
    }

    void UpdateLevelSelectorDependingOnLockedState()
    {
        // Display the correct text depending on whether level is locked or not
        if (levelInfo.IsLocked)
        {
            lockedIndicator.gameObject.SetActive(true);
            unlockedIndicator.gameObject.SetActive(false);
        }
        else
        {
            unlockedIndicator.gameObject.SetActive(true);
            lockedIndicator.gameObject.SetActive(false);
        }
    }

    void UpdateLevelSelectorDependingOnCompletedObjectives()
    {
        if (levelInfo.BonusObjectives.Count > 0)
        {
            var associatedBonusObjectives = levelInfo.BonusObjectives;
            var allCompleted = true;
            foreach (KeyValuePair<string, BonusObjective> objective in associatedBonusObjectives)
            {
                allCompleted = allCompleted && objective.Value.Completed;
            }
            if (allCompleted)
            {
                MarkAsAllDone();
            }
        }
        else if(levelInfo.BonusObjectives.Count == 0 && levelInfo.HasBeenFinished)
        {
            MarkAsAllDone();
        }
    }

    private void MarkAsAllDone()
    {
        unlockedSelectorText.text = "All done";
        var noteRenderer = unlockedNote.GetComponent<Renderer>();
        noteRenderer.materials[0].SetColor("_BaseColor", Color.green);
    }

    // Update is called once per frame
    void Update()
    {
        inputDetector.CheckForGenericUserInputs();
    }

    void OnDestroy()
    {
        gameStateManager.Observers.Remove(this);
    }

    public void DidTouchPosition(Vector3 position)
    {
        if(gameStateManager.GameStarted)
        {
            Ray ray = Camera.main.ScreenPointToRay(position);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit))
            {
                if (rayHit.transform.gameObject == gameObject && !levelInfo.IsLocked)
                {
                    // Selector was hit -> Load associated level
                    Debug.Log("Hit selector");
                    GameProperties.isTutorialLevel = AssociatedLevelIsTutorialLevel;
                    GameProperties.isEndCreditsLevel = false;
                    GameProperties.isRetryingLevel = false;
                    SceneManager.LoadScene(AssociatedLevel);
                }
            }
        }
    }

    public void OnLevelFinished()
    {
        UpdateLevelSelectorDependingOnCompletedObjectives();
        UpdateLevelSelectorDependingOnLockedState();
    }

    public void OnGameLoaded()
    {
        levelInfo = gameStateManager.Levels[LevelKey];
        UpdateLevelSelectorDependingOnCompletedObjectives();
        UpdateLevelSelectorDependingOnLockedState();
    }

    public void OnNewGameStarted()
    {
        // No op
    }

    public void DidDragToPosition(Vector3 position)
    {
        // No op
    }

    public void DidReleaseTouchAtPosition(Vector3 position)
    {
        // No op
    }

    public void DidHoverToPosition(Vector3 position)
    {
        // No op
    }
}
