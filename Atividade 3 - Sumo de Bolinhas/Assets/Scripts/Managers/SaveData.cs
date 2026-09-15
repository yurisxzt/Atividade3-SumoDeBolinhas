using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public string sceneName = "";
    public int levelIndex = 0;
    public int coins = 0;
    public Vector3 playerPosition = Vector3.zero;
    public Vector3 checkpointPosition = Vector3.zero;
    public bool checkpointPassed = false;
    public bool hasReachedCheckpoint = false;
    public string activeCheckpointId = "";
    public List<string> collectedCoinIds = new List<string>();
    public string version = "1.0";

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void FromJson(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
    }
}
