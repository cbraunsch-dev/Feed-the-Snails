using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salad : MonoBehaviour
{
    public SaladListener Listener { get; set; }
    public GameColor gameColor;

    // Start is called before the first frame update
    void Start()
    {
        Color? saladColor = null;
        switch (gameColor)
        {
            case GameColor.Blue:
                saladColor = Color.blue;
                break;
            case GameColor.Red:
                saladColor = Color.red;
                break;
            case GameColor.Green:
                saladColor = Color.green;
                break;
            case GameColor.Yellow:
                saladColor = Color.yellow;
                break;
        }
        
        var saladRenderer = transform.Find("Model").GetComponent<Renderer>();
        saladRenderer.materials[0].SetColor("_BaseColor", saladColor.Value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == Tags.Player && other.gameObject.GetComponent<Snail>().gameColor == this.gameColor)
        {
            Destroy(gameObject);
            this.Listener.SaladEaten(this, other.gameObject);
        }
    }
}

public interface SaladListener
{
    void SaladEaten(Salad salad, GameObject snail);
}
