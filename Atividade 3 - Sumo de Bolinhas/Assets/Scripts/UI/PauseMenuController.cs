using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("Painéis")]
    [SerializeField] private GameObject pauseRoot;
    [SerializeField] private GameObject saveSlotsRoot;

    [Header("Prefab opcional")]
    [SerializeField] private GameObject saveSlotButtonPrefab;

    [Header("Configurações de slots")]
    [SerializeField] private int slotCount = 3;
    [SerializeField] private float slotSpacing = 100f;
    [SerializeField] private Vector2 panelSize = new Vector2(420f, 500f);
    [SerializeField] private Vector2 buttonSize = new Vector2(320f, 55f);

    private bool isSavingMode = false;

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        Time.timeScale = 1f;

        if (pauseRoot != null)
        {
            pauseRoot.SetActive(false);
        }

        if (saveSlotsRoot != null)
        {
            saveSlotsRoot.SetActive(false);
        }
    }

    // =========================================================
    // INPUT
    // =========================================================

    private void Update()
    {
#if ENABLE_INPUT_SYSTEM
        if (
            Keyboard.current != null &&
            (
                Keyboard.current.escapeKey.wasPressedThisFrame ||
                Keyboard.current.pKey.wasPressedThisFrame
            )
        )
        {
            TogglePause();
        }
#else
        if (
            Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetKeyDown(KeyCode.P)
        )
        {
            TogglePause();
        }
#endif
    }

    // =========================================================
    // PAUSE
    // =========================================================

    public void TogglePause()
    {
        if (pauseRoot == null)
        {
            Debug.LogWarning(
                "PauseMenuController: PauseRoot não configurado."
            );

            return;
        }

        bool menuAberto =
            pauseRoot.activeSelf ||
            (
                saveSlotsRoot != null &&
                saveSlotsRoot.activeSelf
            );

        if (menuAberto)
        {
            pauseRoot.SetActive(false);

            if (saveSlotsRoot != null)
            {
                saveSlotsRoot.SetActive(false);
            }

            Time.timeScale = 1f;
        }
        else
        {
            pauseRoot.SetActive(true);

            if (saveSlotsRoot != null)
            {
                saveSlotsRoot.SetActive(false);
            }

            Time.timeScale = 0f;
        }
    }

    // =========================================================
    // SALVAR
    // =========================================================

    public void OnSaveGameClicked()
    {
        isSavingMode = true;

        OpenSlots();
    }

    // =========================================================
    // CARREGAR
    // =========================================================

    public void OnLoadGameClicked()
    {
        isSavingMode = false;

        OpenSlots();
    }

    // =========================================================
    // ABRIR SLOTS
    // =========================================================

    private void OpenSlots()
    {
        if (pauseRoot != null)
        {
            pauseRoot.SetActive(false);
        }

        if (saveSlotsRoot == null)
        {
            saveSlotsRoot = CreateSaveSlotsPanel();
        }

        // Activate the panel first so layout systems run correctly while adding children.
        saveSlotsRoot.SetActive(true);

        ClearOldSlotButtons();

        // IMPORTANTE:
        // Slots manuais são 1..slotCount (configurável).
        for (int slot = 1; slot <= slotCount; slot++)
        {
            CreateSlotButton(
                slot,
                saveSlotsRoot.transform
            );
        }

        CreateBackButton(
            saveSlotsRoot.transform
        );

        // Force layout rebuild so the VerticalLayoutGroup arranges the new buttons immediately.
        RectTransform rt = saveSlotsRoot.GetComponent<RectTransform>();
        if (rt != null)
        {
            // Ensure canvas/layout are updated so spacing and ContentSizeFitter take effect.
            Canvas.ForceUpdateCanvases();
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
            Canvas.ForceUpdateCanvases();
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }

        Time.timeScale = 0f;
    }

    // =========================================================
    // LIMPAR BOTÕES ANTIGOS
    // =========================================================

    private void ClearOldSlotButtons()
    {
        if (saveSlotsRoot == null)
        {
            return;
        }

        for (
            int i =
                saveSlotsRoot.transform.childCount - 1;
            i >= 0;
            i--
        )
        {
            Destroy(
                saveSlotsRoot.transform
                    .GetChild(i)
                    .gameObject
            );
        }
    }

    // =========================================================
    // FECHAR SLOTS
    // =========================================================

    public void CloseSaveSlots()
    {
        if (saveSlotsRoot != null)
        {
            saveSlotsRoot.SetActive(false);
        }

        if (pauseRoot != null)
        {
            pauseRoot.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    // =========================================================
    // CRIAR PAINEL
    // =========================================================

    private GameObject CreateSaveSlotsPanel()
    {
        Canvas canvas =
            GetComponent<Canvas>();

        if (canvas == null)
        {
            canvas =
                FindFirstObjectByType<Canvas>();
        }

        if (canvas == null)
        {
            Debug.LogError(
                "PauseMenuController: Canvas não encontrado."
            );

            return null;
        }

        GameObject panel =
            new GameObject(
                "SaveSlotsRoot",
                typeof(RectTransform),
                typeof(Image),
                typeof(VerticalLayoutGroup),
                typeof(UnityEngine.UI.ContentSizeFitter)
            );

        panel.transform.SetParent(
            canvas.transform,
            false
        );

        RectTransform rect =
            panel.GetComponent<RectTransform>();

        rect.anchorMin =
            new Vector2(0.5f, 0.5f);

        rect.anchorMax =
            new Vector2(0.5f, 0.5f);

        rect.pivot =
            new Vector2(0.5f, 0.5f);

        rect.anchoredPosition =
            Vector2.zero;

        rect.sizeDelta = panelSize;

        Image background =
            panel.GetComponent<Image>();

        background.color =
            new Color(
                0.08f,
                0.08f,
                0.08f,
                0.95f
            );

        VerticalLayoutGroup layout =
            panel.GetComponent<VerticalLayoutGroup>();

        // Do not let the layout override child's preferred sizes — use LayoutElement values.
        layout.childControlWidth = false;
        layout.childControlHeight = false;

        // Position children top-down.
        layout.childAlignment = TextAnchor.UpperCenter;

        layout.spacing = slotSpacing;

        layout.padding = new RectOffset(
            30,
            30,
            30,
            30
        );

        layout.childForceExpandWidth = false;

        layout.childForceExpandHeight = false;

        // Configure ContentSizeFitter so the panel fits its children vertically.
        UnityEngine.UI.ContentSizeFitter csf = panel.GetComponent<UnityEngine.UI.ContentSizeFitter>();
        if (csf != null)
        {
            csf.horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;
            csf.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
        }

        return panel;
    }

    // =========================================================
    // CRIAR SLOT
    // =========================================================

    private void CreateSlotButton(
        int slotIndex,
        Transform parent
    )
    {
        GameObject slotObject;

        if (saveSlotButtonPrefab != null)
        {
            slotObject =
                Instantiate(
                    saveSlotButtonPrefab,
                    parent
                );
        }
        else
        {
            slotObject =
                CreateDefaultButton(
                    "Slot " + slotIndex,
                    parent
                );
        }

        slotObject.name =
            "Slot" + slotIndex + "Button";

        SaveSlotUI slotUI =
            slotObject.GetComponent<SaveSlotUI>();

        if (slotUI == null)
        {
            slotUI =
                slotObject.AddComponent<SaveSlotUI>();
        }

        slotUI.slotIndex =
            slotIndex;

        slotUI.controller =
            this;

        slotUI.isLoadMode =
            !isSavingMode;

        Button button =
            slotObject.GetComponent<Button>();

        slotUI.button =
            button;

        Text text =
            slotObject.GetComponentInChildren<Text>(
                true
            );

        slotUI.label =
            text;

        slotUI.Setup();
    }

    // =========================================================
    // BOTÃO VOLTAR
    // =========================================================

    private void CreateBackButton(
        Transform parent
    )
    {
        GameObject backObject =
            CreateDefaultButton(
                "Voltar",
                parent
            );

        backObject.name =
            "BackButton";

        Button button =
            backObject.GetComponent<Button>();

        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(
            CloseSaveSlots
        );
    }

    // =========================================================
    // BOTÃO PADRÃO
    // =========================================================

    private GameObject CreateDefaultButton(
        string textValue,
        Transform parent
    )
    {
        GameObject buttonObject =
            new GameObject(
                textValue,
                typeof(RectTransform),
                typeof(Image),
                typeof(Button),
                typeof(LayoutElement)
            );

        buttonObject.transform.SetParent(
            parent,
            false
        );

        RectTransform rect =
            buttonObject.GetComponent<RectTransform>();

        rect.sizeDelta = buttonSize;

        LayoutElement layout =
            buttonObject.GetComponent<LayoutElement>();

        layout.preferredWidth = buttonSize.x;

        layout.preferredHeight = buttonSize.y;

        Image image =
            buttonObject.GetComponent<Image>();

        image.color =
            new Color(
                0.2f,
                0.2f,
                0.2f,
                1f
            );

        Button button =
            buttonObject.GetComponent<Button>();

        button.targetGraphic =
            image;

        GameObject labelObject =
            new GameObject(
                "Label",
                typeof(RectTransform),
                typeof(Text)
            );

        labelObject.transform.SetParent(
            buttonObject.transform,
            false
        );

        RectTransform labelRect =
            labelObject.GetComponent<RectTransform>();

        labelRect.anchorMin =
            Vector2.zero;

        labelRect.anchorMax =
            Vector2.one;

        labelRect.offsetMin =
            Vector2.zero;

        labelRect.offsetMax =
            Vector2.zero;

        Text label =
            labelObject.GetComponent<Text>();

        label.text =
            textValue;

        label.alignment =
            TextAnchor.MiddleCenter;

        label.color =
            Color.white;

        label.font =
            Resources.GetBuiltinResource<Font>(
                "LegacyRuntime.ttf"
            );

        label.fontSize =
            22;

        return buttonObject;
    }

    // =========================================================
    // VOLTAR AO MENU
    // =========================================================

    public void OnReturnToMenu()
    {
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance
                .ForceSceneChange(
                    "Menu Inicial"
                );

            return;
        }

        SceneManager.LoadScene(
            "Menu Inicial"
        );
    }
}