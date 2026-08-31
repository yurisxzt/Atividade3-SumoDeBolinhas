using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour {
    public int slotIndex;
    public PauseMenuController controller; // opcional, para fechar UI
    public Button button;
    public Text label;
    [Tooltip("If true this button loads the slot instead of saving it")]
    public bool isLoadMode = false;

    void Start() {
        if (button == null) button = GetComponent<Button>();
        UpdateLabel();
        if (button != null) button.onClick.AddListener(OnPressed);
    }

    public void UpdateLabel() {
        if (label == null) return;
        if (SaveManager.Instance == null) { label.text = $"Slot {slotIndex}"; return; }
        if (SaveManager.Instance.SlotExists(slotIndex))
        {
            var data = SaveManager.Instance.LoadFromSlot(slotIndex);
            label.text = data != null ? $"Slot {slotIndex} - {data.sceneName}" : $"Slot {slotIndex} (Saved)";
        }
        else
        {
            label.text = $"Slot {slotIndex} (Empty)";
        }
    }

    public void OnPressed() {
        if (isLoadMode)
        {
            if (SaveManager.Instance == null) return;
            if (!SaveManager.Instance.SlotExists(slotIndex)) return;
            SaveRestoreManager.Instance.RequestLoadSlot(slotIndex);
            return;
        }

        // salvar jogador
        var player = FindObjectOfType<TwoBallController>();
        var save = new SaveData();
        save.sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        save.playerPosition = player != null ? player.transform.position : Vector3.zero;
        var hud = FindObjectOfType<HUDController>();
        save.coins = hud != null ? hud.Coins : 0;

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveToSlot(slotIndex, save);
            UpdateLabel();
        }

        controller?.TogglePause();
    }
}
