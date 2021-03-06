using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BonusObjectivesToast : MonoBehaviour
{
    const float fadeSpeedFactor = 0.5f;

    private void Start()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
    }

    public void ShowToast(string message)
    {
        GetComponent<AudioSource>().Play();
        var panel = transform.Find("Panel");
        var textComponent = panel.Find("Text");
        var text = textComponent.GetComponent<TextMeshProUGUI>();
        text.text = message;
        StartCoroutine(FadeIn());
        StartCoroutine(FadeOutAfterTime(5));
    }

    IEnumerator FadeIn()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        while(canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / fadeSpeedFactor;
            yield return null;
        }

        yield return null;
    }

    IEnumerator FadeOutAfterTime(float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);

        // Fade out
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeSpeedFactor;
            yield return null;
        }

        yield return null;
    }
}
