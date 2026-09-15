using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    public Vector3 centerOffset = Vector3.zero;
    public bool activated = false;
    public string checkpointId = "checkpoint";

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleTrigger(other.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleTrigger(other.gameObject);
    }

    private void HandleTrigger(GameObject obj)
    {
        if (activated || !obj.CompareTag("Player"))
        {
            return;
        }

        activated = true;

        var save = new SaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            coins = FindFirstObjectByType<HUDController>()?.Coins ?? 0,
            playerPosition = transform.position + centerOffset,
            checkpointPassed = true,
            checkpointPosition = transform.position + centerOffset,
            activeCheckpointId = string.IsNullOrEmpty(checkpointId) ? gameObject.name : checkpointId,
            collectedCoinIds = new List<string>(SaveManager.Instance != null ? SaveManager.Instance.CurrentCollectedCoins : new List<string>())
        };

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveToSlot(0, save);
        }

        var evt = Resources.Load<VoidEventChannel>("EventChannels/CheckpointReached");
        if (evt != null)
        {
            evt.Raise();
        }
    }
}
