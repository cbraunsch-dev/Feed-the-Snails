using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownSprite : MonoBehaviour
{
    public GameObject snail;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         
    }

    private void LateUpdate()
    {
        transform.position = snail.transform.position + new Vector3(0.04f, 0f, 0.04f);
    }
}
