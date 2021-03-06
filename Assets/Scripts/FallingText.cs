using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class FallingText : MonoBehaviour
{
    private float timeToLive = 7.0f;
    private float fadeTime = 0.2f;
    private float scaleTime = 0.2f;
    private Color fadingColor;
    private bool isFadingIn = false;
    private bool isFadingOut = false;
    private Vector3 smallScale;
    private Vector3 fullScale;

    // Start is called before the first frame update
    void Start()
    {
        smallScale = new Vector3(transform.localScale.x * 0.75f, transform.localScale.y * 0.75f, transform.localScale.z * 0.75f);
        GetComponent<Rigidbody>().useGravity = false;
        fullScale = transform.localScale;
        transform.localScale = smallScale;
        var defaultColor = transform.Find("Raw").GetComponent<Renderer>().materials[0].GetColor("_BaseColor");
        transform.Find("Raw").GetComponent<Renderer>().materials[0].SetColor("_BaseColor", new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0.0f));
        StartFadingAndScalingIn();
    }

    // Update is called once per frame
    void Update()
    {
        if(isFadingIn || isFadingOut)
        {   
            UpdateColor();
        }
        if(isFadingIn && TargetOpacityAndScaleReached(1.0f, fullScale))
        {
            GetComponent<Rigidbody>().useGravity = true;
            isFadingIn = false;
            StartCoroutine(ExecuteAfterTime(timeToLive, () =>
            {
                RemoveText();
            }));
        }
        if(isFadingOut && TargetOpacityAndScaleReached(0.0f, smallScale))
        {
            Destroy(gameObject);
        }
    }

    private bool TargetOpacityAndScaleReached(float targetAlpha, Vector3 targetScale)
    {
        var color = transform.Find("Raw").GetComponent<Renderer>().materials[0].GetColor("_BaseColor");
        return color.a == targetAlpha && transform.localScale == targetScale;
    }

    private void UpdateColor()
    {
        transform.Find("Raw").GetComponent<Renderer>().materials[0].SetColor("_BaseColor", fadingColor);
    }

    private void StartFadingAndScalingIn()
    {
        isFadingIn = true;
        fadingColor = transform.Find("Raw").GetComponent<Renderer>().materials[0].GetColor("_BaseColor");
        var fadedInColor = new Color(fadingColor.r, fadingColor.g, fadingColor.b, 1.0f);
        DOTween.To(() => fadingColor, x => fadingColor = x, fadedInColor, fadeTime);
        transform.DOScale(1.0f, scaleTime);
    }

    private void StartFadingAndScalingOut()
    {
        isFadingOut = true;
        fadingColor = transform.Find("Raw").GetComponent<Renderer>().materials[0].GetColor("_BaseColor");
        var fadedOutColor = new Color(fadingColor.r, fadingColor.g, fadingColor.b, 0.0f);
        DOTween.To(() => fadingColor, x => fadingColor = x, fadedOutColor, fadeTime);
        transform.DOScale(smallScale, scaleTime);
    }

    IEnumerator ExecuteAfterTime(float time, Action task)
    {
        yield return new WaitForSeconds(time);
        task();
    }

    private void RemoveText()
    {
        StartFadingAndScalingOut();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.tag == Tags.Floor) {
            var rigidbody = GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
        }
    }
}
