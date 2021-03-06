using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MobileReticleDrawer : ReticleDrawer
{
    private float animationDuration = 0.25f;

    public ReticleDrawerDataProvider DataProvider { get; set; }

    public void DrawInitialState()
    {
        DataProvider.TheImage.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    }

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
        DataProvider.TheImage.DOColor(new Color(0.0f, 0.0f, 0.0f, 0.0f), animationDuration);
    }

    public void DidTouchPosition(Vector3 position)
    {
        var opaqueColor = new Color(DataProvider.TranslucentReticleColor.r, DataProvider.TranslucentReticleColor.g, DataProvider.TranslucentReticleColor.b, 1f);
        DataProvider.TheImage.DOColor(opaqueColor, animationDuration);
    }

    public void DidStartMoving(MovementDirection direction)
    {
        DataProvider.TheImage.DOColor(new Color(0.0f, 0.0f, 0.0f, 0.0f), animationDuration);
    }
}
