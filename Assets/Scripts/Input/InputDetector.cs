using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputDetectorFactory
{
    public static InputDetector CreateDetector()
    {
        if(DeviceDetector.TargetDevice == DeviceDetector.Device.PC)
        {
            return new PCInputDetector();
        }
        else
        {
            return new MobileInputDetector();
        }
    }
}

/**
 * This interface and its implementations are currently in a transitional state. Initially
 * this component was designed with two specialized ways to access it: using either
 * CheckForSnailInputs along with the SnailInteractionInputDetectorListener or using the
 * CheckForGenericInputs along with the GenericInputDetectionListener. Most of the code
 * still uses those two methods. Important to note, however, is that those two methods
 * cannot be combined. This means that one component has to either only use CheckForSnailUserInputs or
 * CheckForGenericUserInputs. This limitation was not sufficient to make parts of the code
 * compatible with mobile (e.g. the Reticle). Therefore a third option was introduced: the
 * CheckForCombinedUserInputs along with a corresponding interface, the CombinedInputDetectionListener.
 * This approach gives the caller information about, both, snail-specific inputs as well as generic
 * inputs. I recommend to move everything to use this combined approach in the future and to phase
 * out the specialized approaches.
 */
public interface InputDetector
{
    SnailInteractionInputDetectorListener SnailInteractionListener { get; set; }

    GenericInputDetectionListener GenericInputDetectionListener { get; set; }

    CombinedInputDetectionListener CombinedInputDetectionListener { get; set; }

    /**
     * Must not be combined with calls to CheckForGenericUserInputs() as this will
     * lead to incorrect behavior. If you wish to use both, use CheckForCombinedUserInputs().
     */
    void CheckForSnailUserInputs();

    /**
     * Must not be combined with calls to CheckForSnailUserInputs() as this will
     * lead to incorrect behavior. If you wish to use both, use CheckForCombinedUserInputs().
     */
    void CheckForGenericUserInputs();

    void CheckForCombinedUserInputs();
}

public interface SnailInteractionInputDetectorListener
{
    void DidStartMoving(MovementDirection direction);
}

public interface GenericInputDetectionListener
{
    void DidTouchPosition(Vector3 position);

    void DidReleaseTouchAtPosition(Vector3 position);

    // Called while the user is dragging the mouse/finger. The position is the current
    // position the user has dragged to thus far.
    void DidDragToPosition(Vector3 position);

    // Called while the user is moving the mouse pointer. The position is the current
    // position the user has moved the mouse to.
    void DidHoverToPosition(Vector3 position);
}

public interface CombinedInputDetectionListener
{
    void DidStartMoving(MovementDirection direction);

    void DidTouchPosition(Vector3 position);

    void DidReleaseTouchAtPosition(Vector3 position, bool swipeDetected);

    // Called while the user is dragging the mouse/finger. The position is the current
    // position the user has dragged to thus far.
    void DidDragToPosition(Vector3 position);

    // Called while the user is moving the mouse pointer. The position is the current
    // position the user has moved the mouse to.
    void DidHoverToPosition(Vector3 position);
}

public enum MovementDirection
{
    Up,
    Down,
    Left,
    Right
}
