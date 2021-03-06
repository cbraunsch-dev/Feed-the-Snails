using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialBehavior : StateMachineBehaviour, SnailInteractionInputDetectorListener, ReticleListener
{
    private Reticle reticle;
    private InputDetector inputDetector;
    private Snail snail;
    private Animator animator;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.animator = animator;
        reticle = GameObject.FindGameObjectWithTag(Tags.Reticle).GetComponent<Reticle>();
        reticle.Listeners.Add(this);
        inputDetector = InputDetectorFactory.CreateDetector();
        inputDetector.SnailInteractionListener = this;
        snail = animator.gameObject.GetComponentInParent<Snail>();
        snail.SelectionIndicator.transform.localScale = Vector3.zero;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // It seems as though OnStateEnter is never called if we immediately trigger the state machine to go to another state once a snail has been created.
        // It should be noted that doing this is a no-no. Only the state machine scripts themselves should trigger transitions. However, in some circumstances
        // we are triggering transitions from the outside. These instances currently include the intro cutscene where we want to have snails move around and the
        // end credits scene where we want to make snails start moving automatically when they are respawned. In both of those occasions we tolerate this weird
        // triggering of states from the outside because both of those places are very much exceptional places for the game. They do not represent standard levels
        // or scenes and thus their implementations should not be applied elsewhere in the game. The risk of spreading the triggering of transitions to other parts
        // of the game are thus pretty low so we tolerate this workaround for now
        if(reticle != null)
        {
            reticle.Listeners.Remove(this);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        inputDetector.CheckForSnailUserInputs();
    }

    void OnDestroy()
    {
        // It appears as though OnDestroy() can be called even if we don't enter this state. I guess this is because these StateMachineBehavior
        // scripts are alive even if the respective state is not active. Entering the given state simply triggers the respective callbacks
        // but does not dictate the lifecycle of the script itself. This is why we need to check for null here because if we never enter this state
        // but the Snail GameObject is destroyed, we get this callback even if we never get the OnStateEnter callback
        if (reticle != null)
        {
            reticle.Listeners.Remove(this);
        }
    }

    //InputDetectorListener interface
    public void DidStartMoving(MovementDirection direction)
    {
        //Nothing to do here
    }

    public void ObjectDidEnterReticle(GameObject gameObject)
    {
        // We have to do this weird workaround with the parent here because the animator that this script is attached to is part of the PlayerAvatar which
        // is a child of the Snail prefab. It's not part of the Snail prefab's root object directly. The callback method here, however, receives the Snail prefab's
        // root object since the Reticle does not want to know about the internals of the Snail prefab.
        if (gameObject == animator.gameObject.transform.parent.gameObject)
        {
            gameObject.GetComponent<Snail>().ScaleIndicatorDiscUp();
        }
    }

    public void DidSelectObject(GameObject gameObject)
    {
        if(gameObject == animator.gameObject.transform.parent.gameObject)
        {
            animator.SetTrigger(SnailTrigger.Select);
        }
        else
        {
            animator.SetTrigger(SnailTrigger.SelectOtherSnail);
        }
    }

    public void DidUnselectObject(GameObject gameObject)
    {
        // No op
    }

    public void ObjectDidExitReticle(GameObject gameObject)
    {
        if(gameObject == animator.gameObject.transform.parent.gameObject)
        {
            gameObject.GetComponent<Snail>().ScaleIndicatorDiscDown();
        }
    }
}
