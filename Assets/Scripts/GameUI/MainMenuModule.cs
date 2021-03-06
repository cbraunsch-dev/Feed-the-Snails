using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuModule : MonoBehaviour, GenericInputDetectionListener
{
    private GameStateManager gameStateManager;
    private InputDetector inputDetector;
    public GameObject newGameItem;
    public GameObject continueGameItem;
    public GameObject mainMenuCamera;

    // Start is called before the first frame update
    void Start()
    {
        inputDetector = InputDetectorFactory.CreateDetector();
        inputDetector.GenericInputDetectionListener = this;
        gameStateManager = GameObject.FindGameObjectWithTag(Tags.GameStateManager).GetComponent<GameStateManager>();
        continueGameItem.SetActive(gameStateManager.SavedGameExists());
    }

    // Update is called once per frame
    void Update()
    {
        inputDetector.CheckForGenericUserInputs();
    }

    public void DidTouchPosition(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit))
        {
            if (rayHit.transform.gameObject == this.newGameItem)
            {
                Debug.Log("Pressed on New Game Item");
                gameStateManager.StartNewGame();
            }
            else if(rayHit.transform.gameObject == this.continueGameItem)
            {
                Debug.Log("Pressed on Continue Game Item");
                gameStateManager.LoadExistingGame();
                
            }
        }
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
