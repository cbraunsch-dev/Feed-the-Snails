using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailSelectionDetector
{
    private List<Snail> snailsInsideTheReticle = new List<Snail>();
    private Snail currentlySelectedSnail = null;

    public void CheckForSnailsInsideReticle(List<ReticleListener> Listeners, RectTransform rectTransform, Transform transform)
    {
        // Find out which snails entered the reticle
        ReticleInteractive gameSceneManager = GameObject.FindGameObjectWithTag(Tags.SceneManager).GetComponent<GameSceneManager>();
        gameSceneManager.Snails.ForEach(snail =>
        {
            if (SnailInsideReticle(snail, rectTransform) && !snailsInsideTheReticle.Contains(snail))
            {
                // If the snail has entered the reticle, add it to the list of snails are inside the reticle
                // and notify the listener
                snailsInsideTheReticle.Add(snail);
                Listeners.ForEach(listener =>
                {
                    listener.ObjectDidEnterReticle(snail.gameObject);
                });
            }
            else if (!SnailInsideReticle(snail, rectTransform) && snailsInsideTheReticle.Contains(snail))
            {
                // If a snail is no longer inside the reticle, remove it from the list of snails
                // that are inside the reticle and notify the listener
                snailsInsideTheReticle.Remove(snail);
                Listeners.ForEach(listener =>
                {
                    listener.ObjectDidExitReticle(snail.gameObject);
                });
            }
        });
    }

    private bool SnailInsideReticle(Snail snail, RectTransform rectTransform)
    {
        var snailPosScreenSpace = Camera.main.WorldToScreenPoint(snail.transform.position);
        var rectTransformLeftX = rectTransform.position.x - (rectTransform.rect.size.x / 2);
        var rectTransformRightX = rectTransformLeftX + rectTransform.rect.size.x;
        var rectTransformBottomY = rectTransform.position.y - (rectTransform.rect.size.y / 2);
        var rectTransformTopY = rectTransformBottomY + rectTransform.rect.size.y;

        if (snailPosScreenSpace.x > rectTransformLeftX &&
            snailPosScreenSpace.x < rectTransformRightX &&
            snailPosScreenSpace.y > rectTransformBottomY &&
            snailPosScreenSpace.y < rectTransformTopY)
        {
            // Snail transform is inside the reticle
            return true;
        }
        return false;
    }

    public void SelectSnailInsideReticle(List<ReticleListener> Listeners, Transform transform)
    {
        // Select snail closest to center
        if (snailsInsideTheReticle.Count > 0)
        {
            // Once the user releases the touch, the reticle disappears which means every snail that was inside
            // the reticle is now no longer inside the reticle
            snailsInsideTheReticle.ForEach(snail =>
            {
                Listeners.ForEach(listener =>
                {
                    listener.ObjectDidExitReticle(snail.gameObject);
                });
            });

            // Notify listeners of selection of snail
            var snailToSelect = SnailClosestToCenterOfReticle(snailsInsideTheReticle, transform);
            if (currentlySelectedSnail != null && currentlySelectedSnail != snailToSelect)
            {
                Listeners.ForEach(listener =>
                {
                    listener.DidUnselectObject(currentlySelectedSnail.gameObject);
                });
            }
            currentlySelectedSnail = snailToSelect;
            Listeners.ForEach(listener =>
            {
                listener.DidSelectObject(snailToSelect.gameObject);
            });
        }
        snailsInsideTheReticle.Clear();
    }

    /**
     * Returns the snail that is closest to the center of the reticle.
     * @param snails Must be a non-empty list of Snails. If the list provided is empty, this method will throw
     * an exception.
     */
    private Snail SnailClosestToCenterOfReticle(List<Snail> snails, Transform transform)
    {
        // The list of snails must not be empty
        if (snails.Count == 0)
        {
            throw new IllegalStateException();
        }

        Snail closestSnail = snails[0];
        var snailPosScreenSpace = Camera.main.WorldToScreenPoint(closestSnail.transform.position);
        var distanceOfClosestSnailToCenter = Vector3.Distance(snailPosScreenSpace, transform.position);
        snails.ForEach(snail =>
        {
            snailPosScreenSpace = Camera.main.WorldToScreenPoint(snail.transform.position);
            var distanceOfCurrentSnailToCenter = Vector3.Distance(snailPosScreenSpace, transform.position);
            if (distanceOfCurrentSnailToCenter < distanceOfClosestSnailToCenter)
            {
                closestSnail = snail;
                distanceOfClosestSnailToCenter = distanceOfCurrentSnailToCenter;
            }
        });
        return closestSnail;
    }
}
