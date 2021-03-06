using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BonusObjectivesNote : MonoBehaviour, GameStateManagerObserver
{
    public string LevelKey;
    private GameStateManager gameStateManager;
    private TextMeshPro noteText;
    private Dictionary<string, BonusObjective> associatedBonusObjectives;
    private string bullet = "-";
    private LevelInfo levelInfo;

    // Start is called before the first frame update
    void Start()
    {
        gameStateManager = GameObject.FindGameObjectWithTag(Tags.GameStateManager).GetComponent<GameStateManager>();
        gameStateManager.Observers.Add(this);
        levelInfo = gameStateManager.Levels[LevelKey];
        var body = transform.Find("Body");
        noteText = body.GetComponent<TextMeshPro>();
        associatedBonusObjectives = levelInfo.BonusObjectives;
        SetNoteTextDependingOnObjectiveCompletionState();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        gameStateManager.Observers.Remove(this);
    }

    public void OnLevelFinished()
    {
        SetNoteTextDependingOnObjectiveCompletionState();
    }

    private void SetNoteTextDependingOnObjectiveCompletionState()
    {
        // Mark any completed objectives as completed
        // Clear the text first
        noteText.text = string.Empty;
        foreach (KeyValuePair<string, BonusObjective> objective in associatedBonusObjectives)
        {
            if (objective.Value.Completed)
            {
                noteText.text = noteText.text + bullet + " " + "COMPLETED: " + objective.Value.Name + System.Environment.NewLine;
            }
            else
            {
                noteText.text = noteText.text + bullet + " " + objective.Value.Name + System.Environment.NewLine;
            }
        }
    }

    public void OnGameLoaded()
    {
        // No op
    }

    public void OnNewGameStarted()
    {
        // No op
    }
}
