using UnityEngine;

public class SaveSlotUI : MonoBehaviour
{
    public int slotIndex;
    public PauseMenuController controller;

    public void OnSlotPressed()
    {
        if (controller == null) return;
        // controller will handle save vs load mode; here just call SaveManager when saving
        // For now assume saving mode
        var save = new SaveData();
        save.sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        save.coins = FindObjectOfType<HUDController>()?.Coins ?? 0;
        save.playerPosition = FindObjectOfType<PlayerController>()?.transform.position ?? Vector3.zero;
        SaveManager.Instance.SaveToSlot(slotIndex, save);
    }
}
