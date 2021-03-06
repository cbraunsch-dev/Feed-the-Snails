using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCReticleInputDetector: ReticleInputDetector
{
    private bool snailSelectionEnabled = false;
    private SnailSelectionDetector snailSelectionDetector = new SnailSelectionDetector();

    public ReticleInputDetectorDataProvider DataProvider { get; set; }

    public void DidDragToPosition(Vector3 position)
    {
        // No op
    }

    public void DidHoverToPosition(Vector3 position)
    {
        // No op
    }

    public void DidReleaseTouchAtPosition(Vector3 position, bool swipeDetected)
    {
        // No op
    }

    public void DidTouchPosition(Vector3 position)
    {
        if (snailSelectionEnabled)
        {
            snailSelectionDetector.CheckForSnailsInsideReticle(DataProvider.ReticleListeners, DataProvider.TheRectTransform, DataProvider.TheTransform);
            snailSelectionDetector.SelectSnailInsideReticle(DataProvider.ReticleListeners, DataProvider.TheTransform);
        }
    }

    public void OnLevelStarted()
    {
        snailSelectionEnabled = true;
    }

    public void OnLevelFinished()
    {
        snailSelectionEnabled = false;
    }

    public void OnLevelLost(List<Salad> saladsToBeEaten, List<Snail> deadSnailsThatShouldEatSalad, Snail kingSnailThatDied)
    {
        snailSelectionEnabled = false;
    }

    public void DidStartMoving(MovementDirection direction)
    {
        // No op
    }
}


