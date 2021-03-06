using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using UnityEngine.Playables;

public class FadeFromBlackCanvas : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private bool startFading = false;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(ExecuteAfterTime(1.5f, () =>
        {
            startFading = true;
            StartPanningCamera();
        }));
    }

    // Update is called once per frame
    void Update()
    {
        if(startFading)
        {
            canvasGroup.DOFade(0.0f, 1.0f);
            startFading = false;
        }
        if (canvasGroup.alpha <= 0)
        {
            this.enabled = false;
        }
    }

    private void StartPanningCamera()
    {
        GameObject.FindGameObjectWithTag(Tags.Timeline).GetComponent<PlayableDirector>().Play();
    }

    IEnumerator ExecuteAfterTime(float time, Action task)
    {
        yield return new WaitForSeconds(time);
        task();
    }
}
