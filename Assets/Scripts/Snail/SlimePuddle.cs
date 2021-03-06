using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePuddle : MonoBehaviour
{
    public GameColor GameColor { get; set; }
    private Renderer modelRenderer;
    private Color currentColor;
    private Vector3 scaleFactor;
    private bool appearing = false;
    private float fadeFactor = 0.01f;
    private float minScaleFactor = 0.01f;
    private float maxScaleFactor = 0.03f;

    // Start is called before the first frame update
    void Start()
    {
        // Make puddle invisible initially
        var primaryColor = this.GameColor.PrimaryColorRepresentation().Value;
        currentColor = new Color(primaryColor.r, primaryColor.g, primaryColor.b, 0.0f);   //Make slime puddle invisible initially
        var model = this.transform.Find("SlimePuddleModel").gameObject;
        modelRenderer = model.GetComponent<Renderer>();
        modelRenderer.materials[0].SetColor("_BaseColor", currentColor);
        transform.localScale = Vector3.zero;

        // Don't scale it uniformly. This will make the puddles look more unique
        scaleFactor = new Vector3(Random.Range(minScaleFactor, maxScaleFactor), minScaleFactor, Random.Range(minScaleFactor, maxScaleFactor));

        appearing = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Scale it up and fade it in
        if (appearing)
        {
            currentColor = new Color(currentColor.r, currentColor.g, currentColor.b, currentColor.a + fadeFactor);
            if (currentColor.a > 1)
            {
                currentColor.a = 1.0f;
            }
            if(transform.localScale.magnitude < 1)
            {
                transform.localScale += scaleFactor;
            }
            
            modelRenderer.materials[0].SetColor("_BaseColor", currentColor);
        }

        if (appearing && currentColor.a >= 1)
        {
            appearing = false;
        }
    }
}
