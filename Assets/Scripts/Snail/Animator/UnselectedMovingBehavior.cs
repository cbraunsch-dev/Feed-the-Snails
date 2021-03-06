using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UnselectedMovingBehavior : StateMachineBehaviour, SnailInteractionInputDetectorListener, GameplayPausedListener, ReticleListener
{
    private Reticle reticle;
    private GameObject gameObject;
    private Snail snail;
    private Animator animator;
    private InputDetector inputDetector;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.animator = animator;
        reticle = GameObject.FindGameObjectWithTag(Tags.Reticle).GetComponent<Reticle>();
        reticle.Listeners.Add(this);
        inputDetector = InputDetectorFactory.CreateDetector();
        inputDetector.SnailInteractionListener = this;
        var gameSceneManager = GameObject.FindGameObjectWithTag(Tags.SceneManager).GetComponent<GameSceneManager>();
        gameSceneManager.GameplayPausedListeners.Add(this);
        gameObject = animator.gameObject.transform.parent.gameObject;
        snail = gameObject.GetComponent<Snail>();
        snail.SelectionIndicator.transform.DOScale(0.0f, 0.25f);
        snail.ChangeColorIndicatorToUnselectedState();
        snail.StartSliming();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gameObject.transform.position += SnailProperties.Velocity * gameObject.transform.forward * Time.deltaTime;
        inputDetector.CheckForSnailUserInputs();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        reticle.Listeners.Remove(this);
        var gameSceneManager = GameObject.FindGameObjectWithTag(Tags.SceneManager).GetComponent<GameSceneManager>();
        gameSceneManager.GameplayPausedListeners.Remove(this);
        snail.StopSliming();
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

    //InputDetectorListener methods
    public void DidStartMoving(MovementDirection direction)
    {
        //Nothing to do here
    }

    public void onPause()
    {
        animator.SetTrigger(SnailTrigger.Stop);
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

    public void ObjectDidExitReticle(GameObject gameObject)
    {
        if (gameObject == animator.gameObject.transform.parent.gameObject)
        {
            gameObject.GetComponent<Snail>().ScaleIndicatorDiscDown();
        }
    }

    public void DidSelectObject(GameObject gameObject)
    {
        if (gameObject == animator.gameObject.transform.parent.gameObject)
        {
            animator.SetTrigger(SnailTrigger.Select);
        }
    }

    public void DidUnselectObject(GameObject gameObject)
    {
        // No op
    }
}
