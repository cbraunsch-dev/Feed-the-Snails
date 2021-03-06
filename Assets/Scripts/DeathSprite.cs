using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSprite : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Scale and make invisible
        var currentColor = GetComponent<SpriteRenderer>().color;
        if(currentColor.a <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.localScale += new Vector3(0.01f, 0.01f, 0.0f);
            currentColor.a -= 0.05f;
            GetComponent<SpriteRenderer>().color = currentColor;
        }

    }
}
