using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

// TODO: The GameSceneManager should be obtained each time a new level is started and
// not in every update or mouse tapped. This is too computationally expensive.
public class Reticle : MonoBehaviour, CombinedInputDetectionListener, GameSceneManagerListener, LevelStateListener, ReticleInputDetectorDataProvider, ReticleDrawerDataProvider
{
    private InputDetector inputDetector;
    private RectTransform rectTransform;
    private Color translucentReticleColor;
    public float animationDuration = 0.25f;
    private ReticleInputDetector reticleInputDetector;
    private ReticleDrawer reticleDrawer;

    public List<ReticleListener> Listeners { get; set; }

    public List<ReticleListener> ReticleListeners => this.Listeners;

    public RectTransform TheRectTransform => this.rectTransform;

    public Transform TheTransform => this.transform;

    public Image TheImage => GetComponent<Image>();

    public Color TranslucentReticleColor { get => translucentReticleColor; }

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        this.Listeners = new List<ReticleListener>();
        inputDetector = InputDetectorFactory.CreateDetector();
        inputDetector.CombinedInputDetectionListener = this;
        translucentReticleColor = GetComponent<Image>().color;
        reticleInputDetector = ReticleInputDetectorFactory.CreateDetector();
        reticleInputDetector.DataProvider = this;
        reticleDrawer = ReticleDrawerFactory.CreateDrawer();
        reticleDrawer.DataProvider = this;
        reticleDrawer.DrawInitialState();
    }

    public void OnSceneManagerDidLoad(GameSceneManager sceneManager)
    {
        sceneManager.LevelStateListeners.Add(this);
    }

    public void OnSceneManagerWillDie(GameSceneManager sceneManager)
    {
        sceneManager.LevelStateListeners.Remove(this);
    }

    // Update is called once per frame
    void Update()
    {
        inputDetector.CheckForCombinedUserInputs();
    }

    public void DidTouchPosition(Vector3 position)
    {
        if(!IsHintOverlayVisible())
        {
            this.reticleInputDetector.DidTouchPosition(position);
        }
        transform.position = position;
        this.reticleDrawer.DidTouchPosition(position);
    }

    private bool IsHintOverlayVisible()
    {
        var hintOverlayGameObject = GameObject.FindGameObjectWithTag(Tags.HintOverlayCanvas);
        if (hintOverlayGameObject != null)
        {
            return hintOverlayGameObject.GetComponent<HintOverlayCanvas>().IsHintOverlayVisible;
        }
        return false;
    }

    public void DidDragToPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void DidReleaseTouchAtPosition(Vector3 position, bool swipeDetected)
    {
        if (!IsHintOverlayVisible())
        {
            this.reticleInputDetector.DidReleaseTouchAtPosition(position, swipeDetected);
        }
        this.reticleDrawer.DidReleaseTouchAtPosition(position, swipeDetected);
    }

    public void DidHoverToPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void OnLevelStarted()
    {
        reticleInputDetector.OnLevelStarted();
    }

    public void OnLevelFinished()
    {
        reticleInputDetector.OnLevelFinished();
    }

    public void OnLevelLost(List<Salad> saladsToBeEaten, List<Snail> deadSnailsThatShouldEatSalad, Snail kingSnailThatDied)
    {
        reticleInputDetector.OnLevelLost(saladsToBeEaten, deadSnailsThatShouldEatSalad, kingSnailThatDied);
    }

    public void DidStartMoving(MovementDirection direction)
    {
        reticleDrawer.DidStartMoving(direction);
    }
}

public class IllegalStateException: Exception {
}

public interface ReticleListener
{
    void ObjectDidEnterReticle(GameObject gameObject);

    void ObjectDidExitReticle(GameObject gameObject);

    void DidSelectObject(GameObject gameObject);

    void DidUnselectObject(GameObject gameObject);
}
