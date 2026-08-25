using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Vector3 centerOffset = Vector3.zero;
    public bool activated = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!activated && other.CompareTag("Player"))
        {
            activated = true;
            var save = new SaveData();
            save.sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            save.coins = FindObjectOfType<HUDController>()?.Coins ?? 0;
            save.playerPosition = transform.position + centerOffset;
            save.checkpointPassed = true;
            save.checkpointPosition = transform.position + centerOffset;
            // TODO: collect list of collected coin IDs from manager
            SaveManager.Instance.SaveToSlot(0, save); // autosave on checkpoint
            var evt = Resources.Load<VoidEventChannel>("EventChannels/CheckpointReached");
            if (evt != null) evt.Raise();
        }
    }
}
