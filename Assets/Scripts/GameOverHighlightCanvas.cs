using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine.UI;
using System;

/**
 * We want this object to be available all the time so that we don't have to drag this component into every scene.
 */
public class GameOverHighlightCanvas : MonoBehaviour, GameSceneManagerListener, LevelStateListener, GenericInputDetectionListener
{
    public GameObject background;
    public GameObject snailHighlight;
    public GameObject saladHighlight;
    public GameObject gameOverMessageCanvas;
    public GameObject continueMessageCanvas;
    private InputDetector inputDetector;

    private List<GameObject> activeSaladHighlights = new List<GameObject>();
    private List<GameObject> activeSnailHighlights = new List<GameObject>();
    private GameSceneManager gameSceneManager;
    private bool isInteractionEnabled = false;
    private TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> continueMessageTweener;
    private List<TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions>> highlightFadeTweeners = new List<TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions>>();
    private List<TweenerCore<Quaternion, Vector3, DG.Tweening.Plugins.Options.QuaternionOptions>> highlightRotateTweeners = new List<TweenerCore<Quaternion, Vector3, DG.Tweening.Plugins.Options.QuaternionOptions>>();

    private static GameOverHighlightCanvas instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
    }

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

    public void OnSceneManagerDidLoad(GameSceneManager sceneManager)
    {
        this.gameSceneManager = sceneManager;
        sceneManager.LevelStateListeners.Add(this);
    }

    public void OnSceneManagerWillDie(GameSceneManager sceneManager)
    {
        this.gameSceneManager = null;
        sceneManager.LevelStateListeners.Remove(this);
    }

    public void OnLevelStarted()
    {
        // No op
    }

    public void OnLevelFinished()
    {
        // No op
    }

    public void OnLevelLost(List<Salad> saladsToBeEaten, List<Snail> deadSnailsThatShouldEatSalad, Snail kingSnailThatDied)
    {
        // Fade in the background and briefly show the game over message
        background.GetComponent<Image>().DOFade(0.25f, 0.25f);
        gameOverMessageCanvas.GetComponent<CanvasGroup>().DOFade(1.0f, 1.0f);
        StartCoroutine(ExecuteAfterTime(3.0f, () =>
        {
            gameOverMessageCanvas.GetComponent<CanvasGroup>().DOFade(0.0f, 1.0f);

            //Start flashing the continue text after some time and allow user to tap to continue
            StartCoroutine(ExecuteAfterTime(2.0f, () =>
            {
                isInteractionEnabled = true;
                continueMessageTweener = continueMessageCanvas.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            }));
        }));

        // Show highlights around snails and salads
        if(saladsToBeEaten != null)
        {
            foreach(var salad in saladsToBeEaten)
            {
                var saladPosScreenSpace = Camera.main.WorldToScreenPoint(salad.transform.position);
                var highlight = Instantiate(saladHighlight, transform);
                PositionHighlightAndAnimate(highlight, saladPosScreenSpace);
                activeSaladHighlights.Add(highlight);
            }
            
        }
        if(deadSnailsThatShouldEatSalad != null)
        {
            foreach(var snail in deadSnailsThatShouldEatSalad)
            {
                SpawnSnailHighight(snail);
            }
        }
        if(kingSnailThatDied != null)
        {
            SpawnSnailHighight(kingSnailThatDied);
        }
    }

    IEnumerator ExecuteAfterTime(float time, Action task)
    {
        yield return new WaitForSeconds(time);
        task();
    }

    private void SpawnSnailHighight(Snail snail)
    {
        var snailPosScreenSpace = Camera.main.WorldToScreenPoint(snail.transform.position);
        var highlight = Instantiate(snailHighlight, transform);
        PositionHighlightAndAnimate(highlight, snailPosScreenSpace);
        activeSnailHighlights.Add(highlight);
    }

    private void PositionHighlightAndAnimate(GameObject highlight, Vector3 position)
    {
        highlight.transform.position = position;
        var highlightFadeTweener = highlight.GetComponent<Image>().DOFade(0.0f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        highlightFadeTweeners.Add(highlightFadeTweener);
        var rotateTweener = highlight.transform.DORotate(new Vector3(0, 0, 90), 2f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        highlightRotateTweeners.Add(rotateTweener);
    }

    public void DidTouchPosition(Vector3 position)
    {
        if(isInteractionEnabled && gameSceneManager != null)
        {
            isInteractionEnabled = false;
            HideGameOverUI();
            gameSceneManager.ShowGameOverStickyNote();
        }
    }

    private void HideGameOverUI()
    {
        background.GetComponent<Image>().DOFade(0.0f, 0.25f);
        gameOverMessageCanvas.GetComponent<CanvasGroup>().DOFade(0.0f, 1.0f);
        continueMessageTweener.Kill(true);
        continueMessageCanvas.GetComponent<CanvasGroup>().DOFade(0.0f, 1.0f);
        highlightFadeTweeners.ForEach(tweener =>
        {
            tweener.Kill();
        });
        highlightFadeTweeners.Clear();
        highlightRotateTweeners.ForEach(tweener => {
            tweener.Kill();
        });
        highlightRotateTweeners.Clear();
        activeSnailHighlights.ForEach(highlight =>
        {
            Destroy(highlight.gameObject);
        });
        activeSnailHighlights.Clear();
        activeSaladHighlights.ForEach(highlight =>
        {
            Destroy(highlight.gameObject);
        });
        activeSaladHighlights.Clear();
    }

    public void DidReleaseTouchAtPosition(Vector3 position)
    {
        // No op
    }

    public void DidDragToPosition(Vector3 position)
    {
        // No op
    }

    public void DidHoverToPosition(Vector3 position)
    {
        // No op
    }
}
