using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class CoroutineUtil
{
    public static IEnumerator ExecuteAfterTime(float time, Action task)
    {
        yield return new WaitForSeconds(time);
        task();
    }
}
