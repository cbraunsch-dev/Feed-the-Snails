using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class GameStateManager : MonoBehaviour
{
    private const string saveGameFilename = "/SavedGame.dat";

    // The keys to access the level info directly
    public static readonly string KeyTutorialLevel1 = "TutorialLevel1";
    public static readonly string KeyTutorialLevel2 = "TutorialLevel2";
    public static readonly string KeyTutorialLevel3 = "TutorialLevel3";
    public static readonly string KeyTutorialLevel4 = "TutorialLevel4";
    public static readonly string KeyTutorialLevel5 = "TutorialLevel5";
    public static readonly string KeyTutorialLevel6 = "TutorialLevel6";
    public static readonly string KeyCastleLevel = "CastleLevel";
    public static readonly string KeyGardenLevel = "GardenLevel";
    public static readonly string KeyEndCreditsLevel = "EndCreditsLevel";

    // The keys for the bonus objectives
    public static readonly string KeyCastleLevelBonusObjective1 = "CastleLevelBonusObjective1";

    // The game state
    GameState gameState = new GameState();

    // All of the level info
    public Dictionary<string, LevelInfo> Levels {
        get
        {
            return gameState.Levels;
        }
    }

    public List<GameStateManagerObserver> Observers { get; private set; }

    // Bonus objectives for each level
    private Dictionary<string, BonusObjective> castleLevelBonusObjectives = new Dictionary<string, BonusObjective>();

    // The GameStateManager is a Singleton that exists throughout the lifetime of the game
    private static GameStateManager instance;   

    void Awake()
    {
        // We always create a new game state. If we load the game from a file, we overwrite this game state with the one
        // loaded from the file
        gameState = new GameState();
        gameState.Levels = new Dictionary<string, LevelInfo>();

        // Set up all the level info
        // Because each level has a reference to the next one, we have to create the data for the last level first

        // 0. The end credits level
        var endCreditsLevel = new LevelInfo(false, null);

        // 1. The first batch of actual levels
        var gardenLevel = new LevelInfo(true, null);
        var castleLevel = new LevelInfo(true, castleLevelBonusObjectives, gardenLevel);

        // 2. Tutorial levels. Tutorial levels do not have bonus objectives
        var tutorialLevel6 = new LevelInfo(true, castleLevel);
        var tutorialLevel5 = new LevelInfo(true, tutorialLevel6);
        var tutorialLevel4 = new LevelInfo(true, tutorialLevel5);
        var tutorialLevel3 = new LevelInfo(true, tutorialLevel4);
        var tutorialLevel2 = new LevelInfo(true, tutorialLevel3);
        var tutorialLevel1 = new LevelInfo(false, tutorialLevel2);  // First tutorial level is unlocked

        this.gameState.Levels[KeyTutorialLevel1] = tutorialLevel1;
        this.gameState.Levels[KeyTutorialLevel2] = tutorialLevel2;
        this.gameState.Levels[KeyTutorialLevel3] = tutorialLevel3;
        this.gameState.Levels[KeyTutorialLevel4] = tutorialLevel4;
        this.gameState.Levels[KeyTutorialLevel5] = tutorialLevel5;
        this.gameState.Levels[KeyTutorialLevel6] = tutorialLevel6;
        this.gameState.Levels[KeyGardenLevel] = gardenLevel;
        this.gameState.Levels[KeyCastleLevel] = castleLevel;
        this.gameState.Levels[KeyEndCreditsLevel] = endCreditsLevel;

        var casleLevelObjective1 = new BonusObjective("Kill all yellow snails");
        castleLevelBonusObjectives.Add(KeyCastleLevelBonusObjective1, casleLevelObjective1);

        Observers = new List<GameStateManagerObserver>();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    public bool GameStarted { get; private set; }

    public bool SavedGameExists()
    {
        return LoadGameState() != null;
    }

    public void StartNewGame()
    {
        this.GameStarted = true;
        Observers.ForEach(x => x.OnNewGameStarted());
    }

    public void LoadExistingGame()
    {
        this.GameStarted = true;
        var loadedGameState = LoadGameState();
        if (loadedGameState != null)
        {
            gameState = loadedGameState;
        }
        Observers.ForEach(x => x.OnGameLoaded());
    }

    public void OnLevelStarted(LevelInfo levelInfo)
    {
        levelInfo.HasBeenPlayed = true;
    }

    public void OnLevelFinished(LevelInfo levelInfo)
    {
        levelInfo.HasBeenFinished = true;

        // Unlock the next level
        var nextLevel = levelInfo.NextLevel;
        if(nextLevel != null)
        {
            nextLevel.IsLocked = false;
        }

        Observers.ForEach(x => x.OnLevelFinished());

        SaveGameState();
    }

    /**
     * It's pretty ugly saving not only the state of the game but also the entire structure of
     * how the levels are connected to one another. One of the reasons that this is ugly is that
     * if new levels were to be added later on after the game had already been released, the structure
     * of the game state would change (e.g. the level that was previously the last one might no longer be
     * the last one and might now have a NextLevel pointer). Since we are serializing the entire game state
     * (including the structure), updating the game state structure might interfere with people's saved games.
     * This could potenetially lead to the issue that updating the already shipped game with new levels could
     * cause people to lose their saved games. This would be pretty bad. Cleaner would be to separate the
     * structure from the actual progress. However, since this is currently just an MVP, this is not an issue.
     * It is probably also not an issue if the existing game is never updated with new content after it's shipped.
     */
    void SaveGameState()
    {
        var filename = Application.persistentDataPath + saveGameFilename;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(filename, FileMode.OpenOrCreate);

        bf.Serialize(file, gameState);
        file.Close();
    }

    GameState LoadGameState()
    {
        var filename = Application.persistentDataPath + saveGameFilename;
        if (File.Exists(filename))
        {
            Debug.Log("Existing game located at: " + filename);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filename, FileMode.Open);
            var loadedGameState = (GameState)bf.Deserialize(file);
            file.Close();

            return loadedGameState;
        }
        return null;
    }
}

[Serializable]
public class BonusObjective
{
    public BonusObjective(string name)
    {
        this.Name = name;
    }

    public string Name { get; set; }

    public bool Completed { get; set; }
}

[Serializable]
public class LevelInfo
{
    public bool IsLocked { get; set; }

    public bool HasBeenPlayed { get; set; }

    /**
     * True if the level's salads have all been eaten. This does not take
     * bonus objectives into account. This means that this can still return true
     * even if not all bonus objectives have been completed yet. This flag just
     * means that the player has managed to at least eat all of the salad of
     * this level.
     */
    public bool HasBeenFinished { get; set; }

    public Dictionary<string, BonusObjective> BonusObjectives { get; set; }

    public LevelInfo NextLevel { get; private set; }

    public LevelInfo(bool isLocked, LevelInfo nextLevel)
    {
        this.IsLocked = isLocked;
        this.NextLevel = nextLevel;
        this.BonusObjectives = new Dictionary<string, BonusObjective>();
    }

    public LevelInfo(bool isLocked, Dictionary<string, BonusObjective> bonusObjectives, LevelInfo nextLevel)
        :this(isLocked, nextLevel)
    {
        this.BonusObjectives = bonusObjectives;
    }
}

[Serializable]
class GameState
{
    public Dictionary<string, LevelInfo> Levels = new Dictionary<string, LevelInfo>();
}

public interface GameStateManagerObserver
{
    /**
     * Notifies observers that the current level was finished (all salads were eaten).
     * The observers are notified after the game state has been updated (ie: after the
     * next level has been unlocked).
     */
    void OnLevelFinished();

    void OnGameLoaded();

    void OnNewGameStarted();
}
