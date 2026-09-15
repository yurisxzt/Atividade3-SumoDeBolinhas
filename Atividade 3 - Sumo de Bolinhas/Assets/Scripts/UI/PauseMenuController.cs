using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseRoot;
    public GameObject saveSlotsRoot;
    public GameObject saveSlotButtonPrefab;
    private bool isSavingMode = false;

    private void Update()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame))
        {
            TogglePause();
        }
#else
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
#endif
    }

    public void TogglePause()
    {
        if (pauseRoot == null)
        {
            return;
        }

        bool active = !pauseRoot.activeSelf;
        pauseRoot.SetActive(active);
        Time.timeScale = active ? 0f : 1f;
    }

    public void OnSaveGameClicked()
    {
        isSavingMode = true;
        EnsureSaveSlotsUI();
    }

    public void OnLoadGameClicked()
    {
        isSavingMode = false;
        EnsureSaveSlotsUI();
    }

    public void CloseSaveSlots()
    {
        isSavingMode = false;
        if (saveSlotsRoot != null)
        {
            saveSlotsRoot.SetActive(false);
        }
    }

    private void EnsureSaveSlotsUI()
    {
        if (saveSlotsRoot == null)
        {
            saveSlotsRoot = CreateSaveSlotsPanel();
        }

        foreach (Transform child in saveSlotsRoot.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < 3; i++)
        {
            GameObject slotObject = CreateSlotButton(i, saveSlotsRoot.transform);
            SaveSlotUI slotUi = slotObject.GetComponent<SaveSlotUI>();
            if (slotUi != null)
            {
                slotUi.isLoadMode = !isSavingMode;
                slotUi.controller = this;
                slotUi.UpdateLabel();
            }
        }

        saveSlotsRoot.SetActive(true);
    }

    private GameObject CreateSaveSlotsPanel()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        }

        GameObject panel = new GameObject("SaveSlotsRoot", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(320f, 240f);

        var image = panel.GetComponent<Image>();
        image.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);

        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 10f;
        layout.padding = new RectOffset(20, 20, 20, 20);
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;

        ContentSizeFitter fitter = panel.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        return panel;
    }

    private GameObject CreateSlotButton(int slotIndex, Transform parent)
    {
        GameObject slotObject = saveSlotButtonPrefab != null ? Instantiate(saveSlotButtonPrefab, parent) : new GameObject($"Slot{slotIndex}", typeof(RectTransform));

        if (saveSlotButtonPrefab == null)
        {
            Image image = slotObject.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 1f);

            Button btn = slotObject.AddComponent<Button>();
            btn.targetGraphic = image;

            GameObject labelObject = new GameObject("Label", typeof(RectTransform));
            labelObject.transform.SetParent(slotObject.transform, false);
            Text text = labelObject.AddComponent<Text>();
            text.alignment = TextAnchor.MiddleCenter;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.color = Color.white;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 12;
            text.resizeTextMaxSize = 24;

            var labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            SaveSlotUI slotUi = slotObject.AddComponent<SaveSlotUI>();
            slotUi.button = btn;
            slotUi.label = text;
            slotUi.slotIndex = slotIndex;
            slotUi.controller = this;
            return slotObject;
        }

        SaveSlotUI existingUi = slotObject.GetComponent<SaveSlotUI>();
        if (existingUi == null)
        {
            existingUi = slotObject.AddComponent<SaveSlotUI>();
        }

        existingUi.slotIndex = slotIndex;
        existingUi.controller = this;
        existingUi.button = slotObject.GetComponent<Button>();

        Text textComponent = slotObject.GetComponentInChildren<Text>(true);
        if (textComponent == null)
        {
            GameObject labelObject = new GameObject("Label", typeof(RectTransform));
            labelObject.transform.SetParent(slotObject.transform, false);
            textComponent = labelObject.AddComponent<Text>();
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComponent.color = Color.white;
        }

        existingUi.label = textComponent;
        return slotObject;
    }

    public void SaveAndResume()
    {
        var player = FindObjectOfType<TwoBallController>();
        if (player != null)
        {
            player.SaveCurrentProgress();
        }

        TogglePause();
    }

    public void OnReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu Inicial");
    }
}
