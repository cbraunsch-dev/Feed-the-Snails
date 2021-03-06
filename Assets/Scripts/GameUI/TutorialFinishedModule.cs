using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFinishedModule : MonoBehaviour, GenericInputDetectionListener
{
    public GameObject continueButton;
    public GameObject tutorialFinishedCamera;
    public GameObject levelSelectionModule;
    private InputDetector inputDetector;

    // Start is called before the first frame update
    void Start()
    {
        inputDetector = InputDetectorFactory.CreateDetector();
        inputDetector.GenericInputDetectionListener = this;
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
            if (rayHit.transform.gameObject == continueButton)
            {
                levelSelectionModule.GetComponent<LevelSelectionModule>().ShowRealLevels();
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
