using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * This sprite will briefly scale up and then scale back down.
 */
public class EatingSprite : MonoBehaviour
{
    private bool scaleUpPhase = true;
    private Vector3 scaleVector = new Vector3(0.05f, 0.05f, 0.0f);
    private Vector3 fullScale;
    private Action animationFinishedCallback;
    private bool runAnimation = false;
    private Transform snailTransform;

    // Start is called before the first frame update
    void Start()
    {
        fullScale = transform.localScale;
    }

    public void playAnimation(Transform snailTransform, Action finishedCallback)
    {
        this.snailTransform = snailTransform;
        this.animationFinishedCallback = finishedCallback;
        this.runAnimation = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (runAnimation)
        {   
            //Scale and make invisible
            var currentColor = GetComponent<SpriteRenderer>().color;
            if (currentColor.a <= 0)
            {
                Destroy(gameObject);
                animationFinishedCallback();
            }
            else
            {
                transform.localScale += new Vector3(0.01f, 0.01f, 0.0f);
                currentColor.a -= 0.05f;
                GetComponent<SpriteRenderer>().color = currentColor;
            }
        }
    }
}
