using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The level manager for the first tutorial level
 * 
 * TODO: Right now there is a bit of code duplication between this class and the default TutorialLevelManager. Get rid of this by either composition
 * or (less preferably) inheritance. Decided to leave this in for now because it's a very small amount of duplication. Mainly just boiler plate.
 */
public class TutorialLevel1Manager : MonoBehaviour, LevelManager
{
    public string KeyTutorialLevel;
    private LevelManagerInteractive gameSceneManager;
    private GameStateManager gameStateManager;
    private LevelInfo levelInfo;
    private HintOverlayCanvas hintOverlayCanvas;

    public GameObject controlsHint;
    public GameObject hintModule;

    // Start is called before the first frame update
    void Start()
    {
        gameSceneManager = GameObject.FindGameObjectWithTag(Tags.SceneManager).GetComponent<GameSceneManager>();
        gameStateManager = GameObject.FindGameObjectWithTag(Tags.GameStateManager).GetComponent<GameStateManager>();
        levelInfo = gameStateManager.Levels[KeyTutorialLevel];
        hintOverlayCanvas = GameObject.FindGameObjectWithTag(Tags.HintOverlayCanvas).GetComponent<HintOverlayCanvas>();
        gameSceneManager.LevelManager = this;

        if(DeviceDetector.TargetDevice == DeviceDetector.Device.Mobile)
        {
            controlsHint.SetActive(false);

            // TODO See if this setting of text depending on target device can be solved using resources somehow
            hintModule.GetComponent<HintModule>().UpdateText("1. Tap on snail to select it \n\n2. Swipe to change directions");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // The following is a workaround because we currently don't know when the cinemachine camera movement has ended and thus need to constantly update the
        // position of the hint overlays
        if (gameSceneManager.Snails.Count > 0 && gameSceneManager.Salads.Count > 0)
        {
            var snail = gameSceneManager.Snails[0];
            var salad = gameSceneManager.Salads[0];

            // This check is necessary. I guess we can't really rely on the salads list, for instance, staying in sync with when the associated GO has been deleted
            if(snail != null && salad != null)
            {
                hintOverlayCanvas.UpdateHintOverlay(snail.transform, salad.transform);
            }
        }
    }

    public void OnLevelFinished()
    {
        gameStateManager.OnLevelFinished(levelInfo);
        controlsHint.SetActive(false);
    }

    public void OnSnailDied(Snail snail)
    {
        // No op
    }

    public void OnLevelStarted()
    {
        hintOverlayCanvas.ShowHintOverlay(gameSceneManager.Snails[0].gameObject, "Select the snail", new Vector2(0.5f, 2.3f), TMPro.TextAlignmentOptions.Center, gameSceneManager.Salads[0].gameObject, "Move the snail to the salad", new Vector2(0.5f, -1.7f), TMPro.TextAlignmentOptions.Center);
        gameStateManager.OnLevelStarted(levelInfo);
    }
}
