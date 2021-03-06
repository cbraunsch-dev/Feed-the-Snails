using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * We want the Reticle to be available all the time so that we don't have to drag this component into every scene.
 */
public class ReticleCanvas : MonoBehaviour
{
    private static ReticleCanvas instance;

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
}
