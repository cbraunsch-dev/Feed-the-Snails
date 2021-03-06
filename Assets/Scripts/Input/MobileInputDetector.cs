using UnityEngine;

// TODO: Clean up the duplication between CheckForSnailUserInputs(), CheckForGenericUserInputs()
// and CheckForCombinedUserInputs().
public class MobileInputDetector : InputDetector
{
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;
    private bool detectSwipeOnlyAfterRelease = true;
    private float minDistanceForSwipe = 20f;

    public SnailInteractionInputDetectorListener SnailInteractionListener { get; set; }
    public GenericInputDetectionListener GenericInputDetectionListener { get; set; }
    public CombinedInputDetectionListener CombinedInputDetectionListener { get; set; }

    public void CheckForSnailUserInputs()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPosition = touch.position;
                fingerDownPosition = touch.position;
            }

            if (!detectSwipeOnlyAfterRelease && touch.phase == TouchPhase.Moved)
            {
                fingerDownPosition = touch.position;
                DetectSwipe();
            }

            if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPosition = touch.position;
                DetectSwipe();
            }
        }
    }

    private void DetectSwipe()
    {
        if (SwipeDetected())
        {
            if (IsVerticalSwipe())
            {
                var direction = fingerDownPosition.y - fingerUpPosition.y > 0 ? MovementDirection.Up : MovementDirection.Down;
                this.SnailInteractionListener.DidStartMoving(direction);
            }
            else
            {
                var direction = fingerDownPosition.x - fingerUpPosition.x > 0 ? MovementDirection.Right : MovementDirection.Left;
                this.SnailInteractionListener.DidStartMoving(direction);
            }
            fingerUpPosition = fingerDownPosition;
        }
    }

    private bool IsVerticalSwipe()
    {
        return VerticalMovementDistance() > HorizontalMovementDistance();
    }

    private bool SwipeDetected()
    {
        return VerticalMovementDistance() > minDistanceForSwipe || HorizontalMovementDistance() > minDistanceForSwipe;
    }

    private float VerticalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.y - fingerUpPosition.y);
    }

    private float HorizontalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x);
    }

    public void CheckForGenericUserInputs()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPosition = touch.position;
                fingerDownPosition = touch.position;
                GenericInputDetectionListener.DidTouchPosition(touch.position);
            }

            if (touch.phase == TouchPhase.Moved)
            {
                fingerDownPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPosition = touch.position;
                if (!SwipeDetected())
                {
                    GenericInputDetectionListener.DidReleaseTouchAtPosition(touch.position);
                }
                fingerUpPosition = fingerDownPosition;
            }
        }
    }

    public void CheckForCombinedUserInputs()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPosition = touch.position;
                fingerDownPosition = touch.position;
                CombinedInputDetectionListener.DidTouchPosition(touch.position);
            }

            if (touch.phase == TouchPhase.Moved)
            {
                fingerDownPosition = touch.position;
                CombinedInputDetectionListener.DidDragToPosition(touch.position);
            }

            if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPosition = touch.position;
                var swipeDetected = DetectedSwipe();
                CombinedInputDetectionListener.DidReleaseTouchAtPosition(touch.position, swipeDetected);    
                fingerUpPosition = fingerDownPosition;
            }
        }
    }

    private bool DetectedSwipe()
    {
        var swipeDetected = SwipeDetected();
        if (swipeDetected)
        {
            if (IsVerticalSwipe())
            {
                var direction = fingerDownPosition.y - fingerUpPosition.y > 0 ? MovementDirection.Up : MovementDirection.Down;
                this.CombinedInputDetectionListener.DidStartMoving(direction);
            }
            else
            {
                var direction = fingerDownPosition.x - fingerUpPosition.x > 0 ? MovementDirection.Right : MovementDirection.Left;
                this.CombinedInputDetectionListener.DidStartMoving(direction);
            }
        }
        return swipeDetected;
    }

    public struct SwipeData
    {
        public Vector2 StartPosition;
        public Vector2 EndPosition;
        public SwipeDirection Direction;
    }

    public enum SwipeDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}
