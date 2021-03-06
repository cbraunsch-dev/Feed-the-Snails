using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileReticleInputDetector : ReticleInputDetector
{
    private bool snailSelectionShouldEnable = false;
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
        if(!swipeDetected)
        {
            if (snailSelectionShouldEnable)
            {
                snailSelectionEnabled = true;
                snailSelectionShouldEnable = false;
            }
            else if (snailSelectionEnabled)
            {
                snailSelectionDetector.CheckForSnailsInsideReticle(DataProvider.ReticleListeners, DataProvider.TheRectTransform, DataProvider.TheTransform);
                snailSelectionDetector.SelectSnailInsideReticle(DataProvider.ReticleListeners, DataProvider.TheTransform);
            }
        }
    }

    public void DidStartMoving(MovementDirection direction)
    {
        // No op
    }

    public void DidTouchPosition(Vector3 position)
    {
        // No op
    }

    public void OnLevelFinished()
    {
        snailSelectionShouldEnable = false;
        snailSelectionEnabled = false;
    }

    public void OnLevelLost(List<Salad> saladsToBeEaten, List<Snail> deadSnailsThatShouldEatSalad, Snail kingSnailThatDied)
    {
        snailSelectionShouldEnable = false;
        snailSelectionEnabled = false;
    }

    public void OnLevelStarted()
    {
        snailSelectionShouldEnable = true;
    }
}
