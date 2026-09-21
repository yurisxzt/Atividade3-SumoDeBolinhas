using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    [Header("Slot")]
    public int slotIndex = 1;

    [Header("Modo")]
    public bool isLoadMode = false;

    [Header("Referências")]
    public PauseMenuController controller;

    public Button button;

    public Text label;

    private bool configured = false;

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        Setup();
    }

    // =========================================================
    // CONFIGURAR
    // =========================================================

    public void Setup()
    {
        if (button == null)
        {
            button =
                GetComponent<Button>();
        }

        if (label == null)
        {
            label =
                GetComponentInChildren<Text>(
                    true
                );
        }

        UpdateLabel();

        if (
            button != null &&
            !configured
        )
        {
            button.onClick.AddListener(
                OnPressed
            );

            configured = true;
        }
    }

    // =========================================================
    // TEXTO
    // =========================================================

    public void UpdateLabel()
    {
        if (label == null)
        {
            return;
        }

        if (SaveManager.Instance == null)
        {
            label.text =
                "Slot " + slotIndex;

            return;
        }

        if (
            SaveManager.Instance
                .SlotExists(
                    slotIndex
                )
        )
        {
            SaveData data =
                SaveManager.Instance
                    .LoadFromSlot(
                        slotIndex
                    );

            if (data != null)
            {
                label.text =
                    "Slot "
                    + slotIndex
                    + " - "
                    + data.sceneName
                    + " - "
                    + data.coins
                    + " moedas";
            }
            else
            {
                label.text =
                    "Slot "
                    + slotIndex
                    + " - Salvo";
            }
        }
        else
        {
            label.text =
                "Slot "
                + slotIndex
                + " - Vazio";
        }
    }

    // =========================================================
    // PRESSIONAR
    // =========================================================

    public void OnPressed()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogWarning(
                "SaveSlotUI: SaveManager não encontrado."
            );

            return;
        }

        if (isLoadMode)
        {
            LoadSlot();
        }
        else
        {
            SaveSlot();
        }
    }

    // =========================================================
    // SALVAR
    // =========================================================

    private void SaveSlot()
    {
        TwoBallController player =
            FindFirstObjectByType<TwoBallController>();

        if (player == null)
        {
            Debug.LogWarning(
                "SaveSlotUI: jogador não encontrado."
            );

            return;
        }

        player.SaveManualProgress(
            slotIndex
        );

        UpdateLabel();

        if (controller != null)
        {
            controller.CloseSaveSlots();
        }
    }

    // =========================================================
    // CARREGAR
    // =========================================================

    private void LoadSlot()
    {
        if (
            !SaveManager.Instance
                .SlotExists(
                    slotIndex
                )
        )
        {
            Debug.Log(
                "Slot "
                + slotIndex
                + " está vazio."
            );

            return;
        }

        if (
            SaveRestoreManager.Instance ==
            null
        )
        {
            Debug.LogWarning(
                "SaveRestoreManager não encontrado."
            );

            return;
        }

        Time.timeScale =
            1f;

        SaveRestoreManager.Instance
            .RequestLoadSlot(
                slotIndex
            );
    }
}