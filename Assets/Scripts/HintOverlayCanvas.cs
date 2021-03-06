using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine.UI;
using TMPro;
using System;

public class HintOverlayCanvas : MonoBehaviour, GenericInputDetectionListener
{
    public GameObject scrim;
    public GameObject highlight1Prototype;  // A prefab will be instantiated from this at runtime
    public GameObject text1Prototype; // A prefab will be instantiated from this at runtime 
    public GameObject highlight2Prototye; // A prefab will be instantiated from this at runtime
    public GameObject text2Prototype; // A prefab will be instantiated from this at runtime
    public GameObject continueMessageCanvas;
    public GameObject highlight1Canvas;
    public GameObject highlight2Canvas;

    private InputDetector inputDetector;
    private TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> continueMessageTweener;
    private List<TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions>> highlightFadeTweeners = new List<TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions>>();
    private List<TweenerCore<Quaternion, Vector3, DG.Tweening.Plugins.Options.QuaternionOptions>> highlightRotateTweeners = new List<TweenerCore<Quaternion, Vector3, DG.Tweening.Plugins.Options.QuaternionOptions>>();
    private GameObject activeHighlight1;
    private GameObject activeText1;
    private GameObject activeHighlight2;
    private GameObject activeText2;
    private bool hintFullyViewed = false;

    public bool IsHintOverlayVisible
    {
        get
        {
            return GetComponent<CanvasGroup>().alpha == 1;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        inputDetector = InputDetectorFactory.CreateDetector();
        inputDetector.GenericInputDetectionListener = this;

        // TODO See if this setting of text depending on target device can be solved using resources somehow
        if (DeviceDetector.TargetDevice == DeviceDetector.Device.Mobile)
        {
            continueMessageCanvas.gameObject.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Tap anywhere to start";
        }
    }

    // Update is called once per frame
    void Update()
    {
        inputDetector.CheckForGenericUserInputs();
    }

    /**
     * Creates and positions highlights around the points of interest.
     * @param text1PositionOffset If you are not sure what to pass here, try a Vector2(0.5f, 0.5f).
     * @param text2PositionOffset If you are not sure what to pass here, try a Vector2(0.5f, 0.5f).
     */
    public void ShowHintOverlay(GameObject poi1, string hint1, Vector2 text1PositionOffset, TextAlignmentOptions text1Alignment, GameObject poi2, string hint2, Vector2 text2PositionOffset, TextAlignmentOptions text2Alignment)
    {
        scrim.GetComponent<Image>().DOFade(0.25f, 0.25f);

        activeHighlight1 = SpawnHighlightObject(highlight1Prototype, poi1.transform.position, highlight1Canvas.transform);
        activeText1 = SpawnHighlightObject(text1Prototype, poi1.transform.position, highlight1Canvas.transform);
        activeText1.GetComponent<TextMeshProUGUI>().text = hint1;
        activeText1.transform.position = activeHighlight1.transform.position;
        activeText1.GetComponent<TextMeshProUGUI>().alignment = text1Alignment;
        activeText1.GetComponent<RectTransform>().pivot = text1PositionOffset;

        activeHighlight2 = SpawnHighlightObject(highlight2Prototye, poi2.transform.position, highlight2Canvas.transform);
        activeText2 = SpawnHighlightObject(text2Prototype, poi2.transform.position, highlight2Canvas.transform);
        activeText2.GetComponent<TextMeshProUGUI>().text = hint2;
        activeText2.transform.position = activeHighlight2.transform.position;
        activeText2.GetComponent<TextMeshProUGUI>().alignment = text2Alignment;
        activeText2.GetComponent<RectTransform>().pivot = text2PositionOffset;

        StartCoroutine(ExecuteAfterTime(0.5f, () =>
        {
            ShowAndAnimate(highlight1Canvas, activeHighlight1);
        }));
        StartCoroutine(ExecuteAfterTime(2.0f, () =>
        {
            ShowAndAnimate(highlight2Canvas, activeHighlight2);
        }));
        StartCoroutine(ExecuteAfterTime(4.0f, () =>
        {
            continueMessageTweener = continueMessageCanvas.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            hintFullyViewed = true;
        }));
    }

    /**
     * Updates the position of the highlights without spawning the highlights
     */
    public void UpdateHintOverlay(Transform poi1Position, Transform poi2Position)
    {
        if (activeHighlight1 != null && activeText1 != null)
        {
            var highlightPosScreenSpace = Camera.main.WorldToScreenPoint(poi1Position.position);
            activeHighlight1.transform.position = highlightPosScreenSpace;
            activeText1.transform.position = activeHighlight1.transform.position;
        }
        if(activeHighlight2 != null && activeText2 != null)
        {
            var highlightPosScreenSpace = Camera.main.WorldToScreenPoint(poi2Position.position);
            activeHighlight2.transform.position = highlightPosScreenSpace;
            activeText2.transform.position = activeHighlight2.transform.position;
        }
    }

    /**
     * @sourceObjectPosition the position of the object that you want to highlight (in world space)
     * @highlightOffset An offset that is applied to the position of the highlight (in screen space)
     */
    private GameObject SpawnHighlightObject(GameObject highlightPrefab, Vector3 sourceObjectPosition, Transform parentTransform)
    {
        var highlightPosScreenSpace = Camera.main.WorldToScreenPoint(sourceObjectPosition);
        var highlight = Instantiate(highlightPrefab, sourceObjectPosition, Quaternion.identity, parentTransform);
        highlight.transform.position = highlightPosScreenSpace;
        return highlight;
    }

    IEnumerator ExecuteAfterTime(float time, Action task)
    {
        yield return new WaitForSeconds(time);
        task();
    }

    private void ShowAndAnimate(GameObject highlightCanvas, GameObject highlight)
    {
        highlightCanvas.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f);
        var highlightFadeTweener = highlight.transform.GetComponent<Image>().DOFade(0.0f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        highlightFadeTweeners.Add(highlightFadeTweener);
        var rotateTweener = highlight.transform.DORotate(new Vector3(0, 0, 90), 2f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        highlightRotateTweeners.Add(rotateTweener);
    }

    public void DidDragToPosition(Vector3 position)
    {
        // No op
    }

    public void DidHoverToPosition(Vector3 position)
    {
        // No op
    }

    public void DidReleaseTouchAtPosition(Vector3 position)
    {
        // No op
    }

    public void DidTouchPosition(Vector3 position)
    {
        if(hintFullyViewed)
        {
            // Dismiss the hint overlay
            GetComponent<CanvasGroup>().DOFade(0.0f, 0.5f);
            highlightFadeTweeners.ForEach(tweener =>
            {
                tweener.Kill();
            });
            highlightFadeTweeners.Clear();
            highlightRotateTweeners.ForEach(tweener =>
            {
                tweener.Kill();
            });
            highlightRotateTweeners.Clear();
            continueMessageTweener.Kill();
        }
    }
}
