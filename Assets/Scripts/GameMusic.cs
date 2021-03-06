using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * We want the game music to play the entire time the game is played. This is why we make it a singleton that never
 * gets destroyed.
 */
public class GameMusic : MonoBehaviour
{
    private static GameMusic instance;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
