using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public string sceneName = "";

    public int levelIndex = 0;

    // Quantidade de moedas do estado salvo
    public int coins = 0;

    // Mantido por compatibilidade.
    // O carregamento usará o início da fase
    // ou checkpointPosition.
    public Vector3 playerPosition = Vector3.zero;

    public Vector3 checkpointPosition = Vector3.zero;

    public bool checkpointPassed = false;

    // Mantido para compatibilidade
    public bool hasReachedCheckpoint = false;

    public string activeCheckpointId = "";

    // Moedas coletadas ATÉ o estado salvo
    public List<string> collectedCoinIds =
        new List<string>();

    public string version = "2.0";

    // =========================================================
    // JSON
    // =========================================================

    public string ToJson()
    {
        return JsonUtility.ToJson(
            this
        );
    }

    public void FromJson(
        string json
    )
    {
        JsonUtility.FromJsonOverwrite(
            json,
            this
        );
    }

    // =========================================================
    // CÓPIA
    // =========================================================

    public SaveData Clone()
    {
        string json =
            JsonUtility.ToJson(
                this
            );

        SaveData copy =
            JsonUtility.FromJson<SaveData>(
                json
            );

        if (copy.collectedCoinIds == null)
        {
            copy.collectedCoinIds =
                new List<string>();
        }

        return copy;
    }
}