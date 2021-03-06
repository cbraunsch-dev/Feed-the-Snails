using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class EndCreditsSceneLevelManager : MonoBehaviour, LevelManager, ReticleListener, GameplayPausedListener
{
    private LevelManagerInteractive gameSceneManager;
    private bool levelStarted = false;
    private bool firstTextSpawn = true;
    private float timeUntilFirstTextSpawn = 2.0f;
    private float timeBetweenTextSpawns = 6.0f;
    private float timeElapsedSinceLastSpawn = 0.0f;
    private int indexOfTextPrefabToSpawn = 0;
    private Reticle reticle;
    private bool playerSelectedFirstSnail = false;
    private LevelInfo levelInfo;
    private GameStateManager gameStateManager;
    private bool gameIsPaused = false;

    public GameObject hintModule;
    public List<GameObject> textPrefabs = new List<GameObject>();
    public List<GameObject> textSpawnPoints = new List<GameObject>();
    public GameObject yellowSnailSpawnPoint;
    public GameObject redSnailSpawnPoint;
    public GameObject blueSnailSpawnPoint;
    public GameObject greenSnailSpawnPoint;
    public GameObject snailPrefab;  // We spawn a new snail each time a snail dies

    // Start is called before the first frame update
    void Start()
    {
        gameSceneManager = GameObject.FindGameObjectWithTag(Tags.SceneManager).GetComponent<GameSceneManager>();
        gameSceneManager.LevelManager = this;
        gameSceneManager.GameplayPausedListeners.Add(this);
        gameStateManager = GameObject.FindGameObjectWithTag(Tags.GameStateManager).GetComponent<GameStateManager>();
        reticle = GameObject.FindGameObjectWithTag(Tags.Reticle).GetComponent<Reticle>();
        reticle.Listeners.Add(this);
        levelInfo = gameStateManager.Levels[GameStateManager.KeyEndCreditsLevel];
        GameProperties.isEndCreditsLevel = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameIsPaused)
        {
            if (!levelStarted)
            {
                levelStarted = true;
                StartCoroutine(ExecuteAfterTime(1.0f, () =>
                {
                    SpawnInitialSnails();
                }));
            }
            else if (playerSelectedFirstSnail)
            {
                timeElapsedSinceLastSpawn += Time.deltaTime;
                if (firstTextSpawn && timeElapsedSinceLastSpawn >= timeUntilFirstTextSpawn)
                {
                    firstTextSpawn = false;
                    timeElapsedSinceLastSpawn = 0.0f;
                    SpawnNewText();
                }
                else if (timeElapsedSinceLastSpawn >= timeBetweenTextSpawns)
                {
                    timeElapsedSinceLastSpawn = 0.0f;
                    SpawnNewText();
                }
            }
        }
    }

    void OnDestroy()
    {
        reticle.Listeners.Remove(this);
        gameSceneManager.GameplayPausedListeners.Remove(this);
    }

    private void SpawnInitialSnails()
    {
        SpawnSnail(GameColor.Blue, false);
        SpawnSnail(GameColor.Green, false);
        SpawnSnail(GameColor.Red, false);
        SpawnSnail(GameColor.Yellow, false);
    }

    private void SpawnSnail(GameColor snailColor, bool snailStartsMoving)
    {
        var newSnail = Instantiate(snailPrefab);
        var spawnPoint = ObtainSpawnPoint(snailColor);
        newSnail.GetComponent<Snail>().gameColor = snailColor;
        newSnail.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y + 0.01f, spawnPoint.transform.position.z);
        newSnail.transform.localScale = Vector3.zero;
        newSnail.transform.forward = DetermineSnailDirection(snailColor);
        newSnail.transform.DOScale(1.0f, 0.5f);
        if(snailStartsMoving)
        {
            MakeSnailStartMoving(newSnail);
        }
        gameSceneManager.AddSnail(newSnail.GetComponent<Snail>());
    }

    private void MakeSnailStartMoving(GameObject snail)
    {
        var animator = snail.GetComponentInChildren<Animator>();

        // Usually setting an animator trigger from outside of the state machine is a big no-no.
        // However, we simply want to have the snails start moving so we just make them think that another
        // snail was selected. This is a bit of a whacky work-around but that's fine for this funny little end-credits scene here.
        animator.SetTrigger(SnailTrigger.SelectOtherSnail);
    }

    private GameObject ObtainSpawnPoint(GameColor snailColor)
    {
        switch(snailColor)
        {
            case GameColor.Blue:
                return blueSnailSpawnPoint;
            case GameColor.Green:
                return greenSnailSpawnPoint;
            case GameColor.Red:
                return redSnailSpawnPoint;
            default:
                return yellowSnailSpawnPoint;
        }
    }

    private Vector3 DetermineSnailDirection(GameColor snailColor)
    {
        switch (snailColor)
        {
            case GameColor.Blue:
            case GameColor.Red:
                return Vector3.forward;
            case GameColor.Green:
            case GameColor.Yellow:
                return Vector3.back;
            default:
                return Vector3.back;
        }
    }

    private void SpawnNewText()
    {
        if(indexOfTextPrefabToSpawn < textPrefabs.Count)
        {
            var newTextPrefab = textPrefabs[indexOfTextPrefabToSpawn];
            var spawnPoint = textSpawnPoints[indexOfTextPrefabToSpawn];
            indexOfTextPrefabToSpawn++;
            var newText = Instantiate(newTextPrefab);
            newText.transform.position = spawnPoint.transform.position;
        }
    }

    IEnumerator ExecuteAfterTime(float time, Action task)
    {
        yield return new WaitForSeconds(time);
        task();
    }

    public void OnLevelFinished()
    {
        hintModule.GetComponent<HintModule>().ShowPostIt();
        gameStateManager.OnLevelFinished(levelInfo);
    }

    public void OnLevelStarted()
    {
        gameStateManager.OnLevelStarted(levelInfo);
    }

    public void OnSnailDied(Snail snail)
    {
        if (!gameIsPaused)
        {
            // Newly spawned snails when a snail dies will automatically start moving to add to the frenzy
            SpawnSnail(snail.gameColor, true);
        }
    }

    public void ObjectDidEnterReticle(GameObject gameObject)
    {
        // No op
    }

    public void ObjectDidExitReticle(GameObject gameObject)
    {
        // No op
    }

    public void DidSelectObject(GameObject gameObject)
    {
        if(gameObject.tag == Tags.Player)
        {
            playerSelectedFirstSnail = true;
        }
    }

    public void DidUnselectObject(GameObject gameObject)
    {
        // No op
    }

    public void onPause()
    {
        gameIsPaused = true;
    }
}
