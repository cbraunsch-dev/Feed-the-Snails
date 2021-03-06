using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverModule : MonoBehaviour, GenericInputDetectionListener
{
    public GameObject levelSelectCamera;
    public GameObject gameOverCamera;
    public GameObject gameCamera;
    public GameObject retryButton;
    public GameObject levelsButton;
    private InputDetector inputDetector;
    private bool justStartedScene = true;
    private bool gameIsOver = false;

    // Start is called before the first frame update
    void Start()
    {
        inputDetector = InputDetectorFactory.CreateDetector();
        inputDetector.GenericInputDetectionListener = this;
        if(GameProperties.isRetryingLevel)
        {
            gameOverCamera.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(justStartedScene && GameProperties.isRetryingLevel)
        {
            StartCoroutine(CoroutineUtil.ExecuteAfterTime(0.0f, () =>
            {
                gameOverCamera.SetActive(false);
                gameCamera.SetActive(true);

                justStartedScene = false;
            }));
        }
        inputDetector.CheckForGenericUserInputs();
    }

    public void DidTouchPosition(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit))
        {
            if (rayHit.transform.gameObject == retryButton && gameIsOver)
            {
                GameProperties.isRetryingLevel = true;
                var scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
            }
            else if (rayHit.transform.gameObject == levelsButton && gameIsOver)
            {
                Debug.Log("Go back to level selection");
                GoToLevelSelection();
            }
        }   
    }

    private void GoToLevelSelection()
    {
        gameOverCamera.SetActive(false);
        levelSelectCamera.SetActive(true);
    }

    public void OnGameOver()
    {
        gameIsOver = true;
        gameCamera.SetActive(false);
        gameOverCamera.SetActive(true);
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
