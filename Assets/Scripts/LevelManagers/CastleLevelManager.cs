using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The LevelManager for the Castle level. Bonus objectives for this level are:
 * - Complete level with the yellow snails all being dead
 */
public class CastleLevelManager : MonoBehaviour, LevelManager
{
    private LevelManagerInteractive gameSceneManager;
    private GameStateManager gameStateManager;
    private BonusObjectivesToast bonusObjectivesToast;
    private bool bonusObjectiveComplete;
    private LevelInfo levelInfo;
    private HintOverlayCanvas hintOverlayCanvas;

    // Start is called before the first frame update
    void Start()
    {
        gameSceneManager = GameObject.FindGameObjectWithTag(Tags.SceneManager).GetComponent<GameSceneManager>();
        gameStateManager = GameObject.FindGameObjectWithTag(Tags.GameStateManager).GetComponent<GameStateManager>();
        bonusObjectivesToast = GameObject.FindGameObjectWithTag(Tags.BonusObjectivesToast).GetComponent<BonusObjectivesToast>();
        levelInfo = gameStateManager.Levels[GameStateManager.KeyCastleLevel];
        gameSceneManager.LevelManager = this;
        hintOverlayCanvas = GameObject.FindGameObjectWithTag(Tags.HintOverlayCanvas).GetComponent<HintOverlayCanvas>();
    }

    void Update()
    {
        // The following is a workaround because we currently don't know when the cinemachine camera movement has ended and thus need to constantly update the
        // position of the hint overlays
        var kingSnails = ObtainKingSnails();
        if (kingSnails.Count > 1)
        {
            var king1 = kingSnails[0];
            var king2 = kingSnails[1];

            // This check is necessary. I guess we can't really rely on the snails list, for instance, staying in sync with when the associated GO has been deleted
            if (king1 != null && king2 != null)
            {
                hintOverlayCanvas.UpdateHintOverlay(king1.transform, king2.transform);
            }
        }
    }

    private List<Snail> ObtainKingSnails()
    {
        var kingSnails = new List<Snail>();
        gameSceneManager.Snails.ForEach(snail =>
        {
            if (snail.king)
            {
                kingSnails.Add(snail);
            }
        });
        return kingSnails;
    }

    public void OnLevelFinished()
    {
        // Store completed objectives in GameStateManager
        var bonusObjectives = levelInfo.BonusObjectives;

        // Only update the bonus objective completed flag if we completed it. We don't want to reset it if the player plays the level again
        // after having completed the bonus objective
        if(bonusObjectiveComplete)
        {
            bonusObjectives[GameStateManager.KeyCastleLevelBonusObjective1].Completed = true;
        }
        gameStateManager.OnLevelFinished(levelInfo);
    }

    public void OnSnailDied(Snail snail)
    {
        //See if all yellow snails have died
        if(snail.gameColor == GameColor.Yellow && gameSceneManager.NumberOfAliveSnailsOfColor(GameColor.Yellow) == 0)
        {
            // Bonus objective has been completed
            var toastMessage = "BONUS OBJECTIVE COMPLETED: Kill all yellow snails";
            Debug.Log(toastMessage);
            bonusObjectivesToast.ShowToast(toastMessage);
            bonusObjectiveComplete = true;
        }
    }

    public void OnLevelStarted()
    {
        // If this level has never been played before, show the hint overlay. Otherwise disable it
        if (!levelInfo.HasBeenPlayed)
        {
            hintOverlayCanvas.gameObject.SetActive(true);
            var kingSnails = ObtainKingSnails();
            hintOverlayCanvas.ShowHintOverlay(kingSnails[0].gameObject, "This is a king snail", new Vector2(0.5f, -1.25f), TMPro.TextAlignmentOptions.Bottom, kingSnails[1].gameObject, "This too. King snails must never die", new Vector2(0.5f, 2.5f), TMPro.TextAlignmentOptions.Top);
        }
        else
        {
            hintOverlayCanvas.gameObject.SetActive(false);
        }
        gameStateManager.OnLevelStarted(levelInfo);
    }
}

