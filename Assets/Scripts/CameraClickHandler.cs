using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClickHandler : MonoBehaviour, GenericInputDetectionListener
{
    public GameObject gameCamera;
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
        this.gameObject.SetActive(false);
        this.gameCamera.SetActive(true);
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
