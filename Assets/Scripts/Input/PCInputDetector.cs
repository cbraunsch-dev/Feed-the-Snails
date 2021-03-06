using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Clean up the duplication between CheckForSnailUserInputs(), CheckForGenericUserInputs()
// and CheckForCombinedUserInputs().
public class PCInputDetector : InputDetector
{
    public SnailInteractionInputDetectorListener SnailInteractionListener { get; set; }
    public GenericInputDetectionListener GenericInputDetectionListener { get; set; }
    public CombinedInputDetectionListener CombinedInputDetectionListener { get; set; }

    private bool dragging = false;

    public void CheckForGenericUserInputs()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.GenericInputDetectionListener.DidTouchPosition(Input.mousePosition);
            this.dragging = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            this.GenericInputDetectionListener.DidReleaseTouchAtPosition(Input.mousePosition);
            this.dragging = false;
        }
        if(dragging && !Input.GetMouseButton(0))
        {
            // If you are were previously dragging but are no longer holding the mouse button down, notify
            // the listener that the mouse button has been released
            this.GenericInputDetectionListener.DidReleaseTouchAtPosition(Input.mousePosition);
            this.dragging = false;
        }
        if (dragging)
        {
            this.GenericInputDetectionListener.DidDragToPosition(Input.mousePosition);
        }
        else
        {
            this.GenericInputDetectionListener.DidHoverToPosition(Input.mousePosition);
        }
    }

    public void CheckForSnailUserInputs()
    {
        if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.SnailInteractionListener.DidStartMoving(MovementDirection.Down);
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.SnailInteractionListener.DidStartMoving(MovementDirection.Left);
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.SnailInteractionListener.DidStartMoving(MovementDirection.Right);
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.SnailInteractionListener.DidStartMoving(MovementDirection.Up);
        }
    }

    public void CheckForCombinedUserInputs()
    {
        // Check generic inputs
        if (Input.GetMouseButtonDown(0))
        {
            this.CombinedInputDetectionListener.DidTouchPosition(Input.mousePosition);
            this.dragging = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            this.CombinedInputDetectionListener.DidReleaseTouchAtPosition(Input.mousePosition, false);
            this.dragging = false;
        }
        if (dragging && !Input.GetMouseButton(0))
        {
            // If you are were previously dragging but are no longer holding the mouse button down, notify
            // the listener that the mouse button has been released
            this.CombinedInputDetectionListener.DidReleaseTouchAtPosition(Input.mousePosition, false);
            this.dragging = false;
        }
        if (dragging)
        {
            this.CombinedInputDetectionListener.DidDragToPosition(Input.mousePosition);
        }
        else
        {
            this.CombinedInputDetectionListener.DidHoverToPosition(Input.mousePosition);
        }

        // Check snail-specific inputs
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.CombinedInputDetectionListener.DidStartMoving(MovementDirection.Down);
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.CombinedInputDetectionListener.DidStartMoving(MovementDirection.Left);
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.CombinedInputDetectionListener.DidStartMoving(MovementDirection.Right);
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.CombinedInputDetectionListener.DidStartMoving(MovementDirection.Up);
        }
    }
}
