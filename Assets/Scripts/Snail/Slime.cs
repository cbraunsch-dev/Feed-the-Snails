using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour, GameplayPausedListener
{
    public GameColor GameColor { get; set; }
    public float TimeToLive { get; set; }
    private float timeSinceAlive = 0.0f;
    private bool appearing = false;
    private bool disappearing = false;
    private Color currentColor;
    private Renderer modelRenderer;
    private Vector3 defaultScale;
    private float fadeFactor = 0.05f;
    private Vector3 scaleFactor = new Vector3(0.01f, 0.01f, 0.01f);
    private GameSceneManager gameSceneManager;
    private bool isGamePaused = false;

    // Start is called before the first frame update
    void Start()
    {
        this.TimeToLive = 6.0f;
        var primaryColor = this.GameColor.PrimaryColorRepresentation().Value;
        currentColor = new Color(primaryColor.r, primaryColor.g, primaryColor.b, 0.0f);   //Make slime invisible initially
        var model = this.transform.Find("SlimeModel").gameObject;
        modelRenderer = model.GetComponent<Renderer>();
        modelRenderer.materials[0].SetColor("_BaseColor", currentColor);
        defaultScale = transform.localScale;
        transform.localScale *= 0.5f;
        appearing = true;
        gameSceneManager = GameObject.FindGameObjectWithTag(Tags.SceneManager).GetComponent<GameSceneManager>();
        gameSceneManager.GameplayPausedListeners.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isGamePaused)
        {
            // Fade in/Fade out slime depending whether it's currently appearing or disappearing
            if (appearing)
            {
                currentColor = new Color(currentColor.r, currentColor.g, currentColor.b, currentColor.a + fadeFactor);
                if (currentColor.a > 1)
                {
                    currentColor.a = 1.0f;
                }
                transform.localScale += scaleFactor;
                if (transform.localScale.magnitude > defaultScale.magnitude)
                {
                    transform.localScale = defaultScale;
                }
            }
            if (disappearing)
            {
                currentColor = new Color(currentColor.r, currentColor.g, currentColor.b, currentColor.a - fadeFactor);
                if (currentColor.a < 0)
                {
                    currentColor.a = 0.0f;
                }
                transform.localScale -= scaleFactor;
                if (vectorHasNegativeComponent(transform.localScale))
                {
                    transform.localScale = Vector3.zero;
                }
            }
            if (appearing || disappearing)
            {
                modelRenderer.materials[0].SetColor("_BaseColor", currentColor);
            }

            // Determine whether we are fading in or fading out the slime
            if (appearing && currentColor.a >= 1 && transform.localScale == defaultScale)
            {
                appearing = false;
            }
            if (!appearing && !disappearing)
            {
                timeSinceAlive += Time.deltaTime;
                if (timeSinceAlive >= TimeToLive)
                {
                    disappearing = true;

                }
            }

            // Destroy the slime once we are done fading it out
            if (disappearing && currentColor.a <= 0 && transform.localScale == Vector3.zero)
            {
                gameSceneManager.GameplayPausedListeners.Remove(this);
                Destroy(this.gameObject);
            }
        }
    }

    private bool vectorHasNegativeComponent(Vector3 vector)
    {
        return vector.x < 0 || vector.y < 0 || vector.z < 0;
    }

    public void onPause()
    {
        this.isGamePaused = true;
    }
}
