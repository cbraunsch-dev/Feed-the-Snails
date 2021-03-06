using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Gives life to the MainMenuScene by moving around snails etc.
 */
public class MainMenuSceneTimeline : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var playerGameObjects = GameObject.FindGameObjectsWithTag(Tags.Player);
        foreach (GameObject playerGameObject in playerGameObjects)
        {
            var animator = playerGameObject.GetComponentInChildren<Animator>();

            // Usually setting an animator trigger from outside of the state machine is a big no-no.
            // However, we simply want to have the snails start moving so we just make them think that another
            // snail was selected. This is a bit of a whacky work-around but that's fine for this cutscene here.
            animator.SetTrigger(SnailTrigger.SelectOtherSnail);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
