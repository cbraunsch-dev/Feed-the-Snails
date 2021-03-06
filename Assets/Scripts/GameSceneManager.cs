using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameSceneManager : MonoBehaviour, SaladListener, SnailListener, LevelManagerInteractive, ReticleInteractive
{
    public AudioClip saladEatenSoundEffect;
    public AudioClip allSaladsEatenSoundEffect;
    public AudioClip gameOverSoundEffect;
    readonly List<Snail> snails = new List<Snail>();
    readonly List<Salad> salads = new List<Salad>();
    private Reticle reticle;
    private GameOverHighlightCanvas gameOverHighlightCanvas;

    public GameObject deathSprite;
    public GameObject eatingSprite;
    public GameObject crownSprite;
    private InputDetector inputDetector;
    private bool gameOver = false;
    private List<GameplayPausedListener> myGameplayPausedListeners = new List<GameplayPausedListener>();
    private List<LevelStateListener> myLevelStateListeners = new List<LevelStateListener>();
    
    // Start is called before the first frame update
    void Start()
    {
        var playerGameObjects = GameObject.FindGameObjectsWithTag(Tags.Player);
        reticle = GameObject.FindGameObjectWithTag(Tags.Reticle).GetComponent<Reticle>();
        reticle.OnSceneManagerDidLoad(this);
        gameOverHighlightCanvas = GameObject.FindGameObjectWithTag(Tags.GameOverHighlightCanvas).GetComponent<GameOverHighlightCanvas>();
        gameOverHighlightCanvas.OnSceneManagerDidLoad(this);
        foreach(GameObject playerGameObject in playerGameObjects)
        {
            var snail = playerGameObject.GetComponent<Snail>();
            snail.SnailListener = this;
            snails.Add(snail);
            if(snail.king)
            {
                var crown = Instantiate(crownSprite, Vector3.zero, Quaternion.Euler(90, 0, 0));
                crown.GetComponent<CrownSprite>().snail = snail.gameObject;
            }
        }
        var saladGameObjects = GameObject.FindGameObjectsWithTag(Tags.Salad);
        foreach (GameObject saladGameObject in saladGameObjects)
        {
            var salad = saladGameObject.GetComponent<Salad>();
            salad.Listener = this;
            salads.Add(salad);
        }
    }

    void OnDestroy()
    {
        reticle.OnSceneManagerWillDie(this);
        gameOverHighlightCanvas.OnSceneManagerWillDie(this);
    }

    public void OnLevelStarted()
    {
        this.LevelManager.OnLevelStarted();
        LevelStateListeners.ForEach(x => { x.OnLevelStarted(); });
    }

    public List<Snail> Snails {
        get
        {
            return snails;
        }
    }

    public List<Salad> Salads
    {
        get
        {
            return salads;
        }
    }

    public LevelManager LevelManager { private get; set; }

    private void PlaySound(AudioClip clip)
    {
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
    }

    public void SaladEaten(Salad salad, GameObject snail)
    {
        PlaySound(saladEatenSoundEffect);
        ShowEatingAnimation(snail, () => {
            this.salads.Remove(salad);
            if (salads.Count == 0)
            {
                //Level finished
                PlaySound(allSaladsEatenSoundEffect);
                Debug.Log("Level finished!");
                this.LevelManager.OnLevelFinished();
                this.LevelStateListeners.ForEach(listener => {
                    listener.OnLevelFinished();
                });
                GameObject.FindGameObjectWithTag(Tags.HintModule).GetComponent<HintModule>().OnLevelFinished();
                this.GameplayPausedListeners.ForEach(x => { x.onPause(); });
            }
        });
    }

    void ShowEatingAnimation(GameObject snail, Action animationFinishedCallback)
    {
        var rotation = Quaternion.Euler(90, 0, 0);
        var snailHeight = snail.GetComponent<BoxCollider>().bounds.size.y;
        var targetPosition = new Vector3(snail.transform.position.x, snail.transform.position.y + snailHeight, snail.transform.position.z);
        var sprite = Instantiate(eatingSprite, targetPosition, rotation);
        sprite.GetComponent<EatingSprite>().playAnimation(snail.transform, animationFinishedCallback);
    }

    public void SnailDying(Snail snail)
    {
        ShowDeathAnimation(snail);
    }

    public void SnailDied(Snail snail)
    {
        CheckIfKingSnailDied(snail);
        CheckIfStillEnoughSnailsToEatAllSalads();
        this.LevelManager.OnSnailDied(snail);
    }

    void ShowDeathAnimation(Snail snail)
    {
        var rotation = Quaternion.Euler(90, 0, 0);
        var snailHeight = snail.GetComponent<BoxCollider>().bounds.size.y;
        var targetPosition = new Vector3(snail.transform.position.x, snail.transform.position.y + snailHeight, snail.transform.position.z);
        Instantiate(deathSprite, targetPosition, rotation);
    }

    void CheckIfKingSnailDied(Snail snail)
    {
        if(snail.king)
        {
            ShowGameOverScreen(null, null, snail);
        }
    }

    void CheckIfStillEnoughSnailsToEatAllSalads()
    {
        foreach(GameColor color in System.Enum.GetValues(typeof(GameColor)))
        {
            var nrOfSaladsOfColor = NumberOfSaladsOfColor(color);
            if(nrOfSaladsOfColor > 0)
            {
                var nrOfAliveSnailsOfColor = NumberOfAliveSnailsOfColor(color);
                if (nrOfAliveSnailsOfColor == 0)
                {
                    var saladsOfColor = SaladsOfColor(color);
                    var snailsOfColor = SnailsOfColor(color);
                    ShowGameOverScreen(saladsOfColor, snailsOfColor, null);
                    break;
                }
            }
        }
    }

    public int NumberOfAliveSnailsOfColor(GameColor color)
    {
        var number = 0;
        foreach(Snail snail in snails)
        {
            if(!snail.Dead && snail.gameColor == color)
            {
                number++;
            }
        }
        return number;
    }

    List<Snail> SnailsOfColor(GameColor color)
    {
        var snailsOfColor = new List<Snail>();
        foreach (Snail snail in snails)
        {
            if (snail.gameColor == color)
            {
                snailsOfColor.Add(snail);
            }
        }
        return snailsOfColor;
    }

    int NumberOfSaladsOfColor(GameColor color)
    {
        var number = 0;
        foreach(Salad salad in salads)
        {
            if(salad.gameColor == color)
            {
                number++;
            }
        }
        return number;
    }

    List<Salad> SaladsOfColor(GameColor color)
    {
        var saladsOfColor = new List<Salad>();
        foreach (Salad salad in salads)
        {
            if (salad.gameColor == color)
            {
                saladsOfColor.Add(salad);
            }
        }
        return saladsOfColor;
    }

    void ShowGameOverScreen(List<Salad> saladsToBeEaten, List<Snail> deadSnailsThatShouldEatSalad, Snail kingSnailThatDied)
    {
        if (!gameOver)
        {
            Debug.Log("Game Over!");
            gameOver = true;
            PlaySound(gameOverSoundEffect);
            this.GameplayPausedListeners.ForEach(x => { x.onPause(); });
            this.LevelStateListeners.ForEach(x => { x.OnLevelLost(saladsToBeEaten, deadSnailsThatShouldEatSalad, kingSnailThatDied); });
        }
    }

    public void ShowGameOverStickyNote()
    {
        GameObject.FindGameObjectWithTag(Tags.GameOverModule).GetComponent<GameOverModule>().OnGameOver();
    }

    public void AddSnail(Snail snail)
    {
        this.Snails.Add(snail);
        snail.SnailListener = this;
    }

    public List<GameplayPausedListener> GameplayPausedListeners
    {
        get
        {
            return myGameplayPausedListeners;
        }
        set
        {
            myGameplayPausedListeners = value;
        }
    }

    public List<LevelStateListener> LevelStateListeners { get
        {
            return myLevelStateListeners;
        }
        set
        {
            myLevelStateListeners = value;
        }
    }
}

/**
 * Provides a LevelManager access to specific methods and properties (interface view).
 */
    public interface LevelManagerInteractive
{
    List<Snail> Snails { get; }

    List<Salad> Salads { get; }

    int NumberOfAliveSnailsOfColor(GameColor color);

    LevelManager LevelManager { set; }

    void AddSnail(Snail snail);

    List<GameplayPausedListener> GameplayPausedListeners { get; }
}

/**
 * Provides the Reticle access to specific methods and properties (interface view).
 */
public interface ReticleInteractive
{
    List<Snail> Snails { get; }
}

public interface GameplayPausedListener
{
    void onPause();
}

/**
 * Anyone interested in the state of the current level (whether the level has been started, finished, lost etc)
 * can implement this interface and be notified accordingly.
 */
public interface LevelStateListener
{
    void OnLevelStarted();

    void OnLevelFinished();

    /**
     * Called when the player died in this level
     * @param saladsToBeEaten Optional. Pass null if not relevant. The salads that should still be eaten but cannot be eaten anymore because no snails of that color are alive anymore
     * @param deadSnailsThatShouldEatSalad Optional. Pass null if not relevant. The dead snails that should still be alive to eat the salad that remains.
     * @param kingSnailThatDied. Optional. Pass null if not relevant. The king snail that died.
     */
    void OnLevelLost(List<Salad> saladsToBeEaten, List<Snail> deadSnailsThatShouldEatSalad, Snail kingSnailThatDied);
}

public interface GameSceneManagerListener
{
    void OnSceneManagerDidLoad(GameSceneManager sceneManager);

    void OnSceneManagerWillDie(GameSceneManager sceneManager);
}
