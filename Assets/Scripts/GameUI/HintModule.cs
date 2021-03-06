using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class HintModule : MonoBehaviour, GenericInputDetectionListener, LevelStateListener
{
    public GameObject levelSelectCamera;
    public GameObject hintCamera;
    public GameObject gameCamera;
    public GameObject confirmButton;
    public GameObject backButton;
    public GameObject continueButton;   // The button that appears when the level was finished. Allows the user to go to the next level
    public GameObject tutorialFinishedCamera;   // The camera to activate once we finished playing the tutorial levels
    public string endCreditsSceneName;  // The name of the scene that will show the end credits. Is shown when the player presses Continue after having played the final level
    public bool isFinalLevelOfSection;  // True if this hint is shown in the final level of a level selection section
    public string LevelKey;
    private LevelInfo levelInfo;
    private InputDetector inputDetector;
    private GameSceneManager gameSceneManager;
    private bool justStartedScene = true;
    private bool gameIsOver = false;

    // Start is called before the first frame update
    void Start()
    {
        gameSceneManager = GameObject.FindGameObjectWithTag(Tags.SceneManager).GetComponent<GameSceneManager>();
        gameSceneManager.LevelStateListeners.Add(this);
        inputDetector = InputDetectorFactory.CreateDetector();
        inputDetector.GenericInputDetectionListener = this;

        var gameStateManager = GameObject.FindGameObjectWithTag(Tags.GameStateManager).GetComponent<GameStateManager>();
        levelInfo = gameStateManager.Levels[LevelKey]; 

        if(!GameProperties.isRetryingLevel)
        {
            levelSelectCamera.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (justStartedScene && GameProperties.isEndCreditsLevel)
        {
            StartLevel();
            justStartedScene = false;
        }
        else if(justStartedScene && !GameProperties.isRetryingLevel)
        {
            StartCoroutine(CoroutineUtil.ExecuteAfterTime(0.0f, () =>
            {
                // Move from level select camera to hint camera as soon as scene is loaded
                levelSelectCamera.SetActive(false);
                hintCamera.SetActive(true);

                justStartedScene = false;
            }));
        }
        else if(justStartedScene && GameProperties.isRetryingLevel)
        {
            gameSceneManager.OnLevelStarted();
            justStartedScene = false;
        }
        inputDetector.CheckForGenericUserInputs();
    }

    void OnDestroy()
    {
        gameSceneManager.LevelStateListeners.Remove(this);
    }

    public void DidTouchPosition(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit))
        {
            if (rayHit.transform.gameObject == confirmButton && !gameIsOver)
            {
                Debug.Log("Start level+");
                StartLevel();
            }
            if (rayHit.transform.gameObject == backButton && !gameIsOver)
            {
                Debug.Log("Go back to level selection");
                GoToLevelSelection();
            }
            if(rayHit.transform.gameObject == continueButton && !gameIsOver)
            {
                if(isFinalLevelOfSection)
                {
                    // We finished playing the final tutorial level and just unlocked the first real level
                    Debug.Log("Finished final tutorial level. Time for the real levels");
                    GoToTutorialLevelFinishedCamera();
                }
                else if(levelInfo.NextLevel == null)
                {
                    // We finished playing the final level of the game
                    Debug.Log("Finished final level of the game");
                    GoToFinalLevelFinishedCamera();
                }
                else
                {
                    Debug.Log("Go back to level selection");
                    GoToLevelSelection();
                }
            }
        }
    }

    public void StartLevel()
    {
        hintCamera.SetActive(false);
        gameCamera.SetActive(true);
        gameSceneManager.OnLevelStarted();
    }

    public void ShowPostIt()
    {
        var postIt = transform.Find("Post-It");
        postIt.gameObject.SetActive(true);
    }

    private void GoToLevelSelection()
    {
        hintCamera.SetActive(false);
        levelSelectCamera.SetActive(true);
    }

    private void GoToTutorialLevelFinishedCamera()
    {
        hintCamera.SetActive(false);
        tutorialFinishedCamera.SetActive(true);
    }

    private void GoToFinalLevelFinishedCamera()
    {
        SceneManager.LoadScene(endCreditsSceneName);
    }

    public void UpdateText(string textToShow)
    {
        var postIt = transform.Find("Post-It");
        var text = postIt.transform.Find("Text");
        var textMesh = text.GetComponent<TextMeshPro>();
        textMesh.text = textToShow;
    }

    public void OnLevelFinished()
    {
        confirmButton.SetActive(false);
        backButton.SetActive(false);
        continueButton.SetActive(true);

        var postIt = transform.Find("Post-It");
        var text = postIt.transform.Find("Text");
        var textMesh = text.GetComponent<TextMeshPro>();
        if (GameProperties.isTutorialLevel)
        {
            textMesh.text = "Congratulations! You successfully completed this tutorial level.";
        }
        else if(GameProperties.isEndCreditsLevel)
        {
            textMesh.text = "Way to go! You have finished the game. Have you completed all of the bonus objectives?";
        }
        else
        {
            textMesh.text = "Nice! You managed to eat all of the salad in this level.";
        }
        gameCamera.SetActive(false);
        hintCamera.SetActive(true);
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

    public void OnLevelStarted()
    {
        // No op
    }

    public void OnLevelLost(List<Salad> saladsToBeEaten, List<Snail> deadSnailsThatShouldEatSalad, Snail kingSnailThatDied)
    {
        gameIsOver = true;
    }
}
