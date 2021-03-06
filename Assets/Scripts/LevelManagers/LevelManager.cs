using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface LevelManager
{
    void OnLevelStarted();

    void OnSnailDied(Snail snail);

    void OnLevelFinished();
}
