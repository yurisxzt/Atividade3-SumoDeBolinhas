using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PushCooldownUI : MonoBehaviour
{
    private PlayerSpawner spawner;

    private RectTransform player1Fill;
    private RectTransform player2Fill;

    private TMP_Text player1Text;
    private TMP_Text player2Text;

    private void Start()
    {
        // Procura o PlayerSpawner mesmo que ele esteja
        // em outra cena carregada aditivamente.
        spawner = FindFirstObjectByType<PlayerSpawner>();

        CreateUI();

        if (spawner == null)
        {
            Debug.LogWarning(
                "PushCooldownUI: PlayerSpawner não encontrado."
            );
        }
    }

    private void Update()
    {
        // Se ainda não encontrou, tenta procurar novamente.
        if (spawner == null)
        {
            spawner = FindFirstObjectByType<PlayerSpawner>();

            if (spawner == null)
                return;
        }

        // ============================
        // PLAYER 1
        // ============================

        if (spawner.Player1Controller != null)
        {
            float value =
                spawner.Player1Controller.PushCooldownNormalized;

            value = Mathf.Clamp01(value);

            UpdateBar(
                player1Fill,
                value
            );

            if (player1Text != null)
            {
                player1Text.text =
                    spawner.Player1Controller.CanPush
                    ? "P1 - PRONTO"
                    : "P1 - CARREGANDO";
            }
        }

        // ============================
        // PLAYER 2
        // ============================

        if (spawner.Player2Controller != null)
        {
            float value =
                spawner.Player2Controller.PushCooldownNormalized;

            value = Mathf.Clamp01(value);

            UpdateBar(
                player2Fill,
                value
            );

            if (player2Text != null)
            {
                player2Text.text =
                    spawner.Player2Controller.CanPush
                    ? "P2 - PRONTO"
                    : "P2 - CARREGANDO";
            }
        }
    }

    // Faz a barra crescer horizontalmente
    // de 0 até 100%.
    private void UpdateBar(
        RectTransform fill,
        float value)
    {
        if (fill == null)
            return;

        fill.anchorMax =
            new Vector2(
                value,
                1f
            );

        fill.offsetMin =
            new Vector2(4f, 4f);

        fill.offsetMax =
            new Vector2(-4f, -4f);
    }

    private void CreateUI()
    {
        // ============================
        // CANVAS
        // ============================

        GameObject canvasObject =
            new GameObject(
                "PushCooldownCanvas",
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster)
            );

        canvasObject.transform.SetParent(
            transform,
            false
        );

        Canvas canvas =
            canvasObject.GetComponent<Canvas>();

        canvas.renderMode =
            RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler =
            canvasObject.GetComponent<CanvasScaler>();

        scaler.uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;

        scaler.referenceResolution =
            new Vector2(1920f, 1080f);

        // ============================
        // PLAYER 1
        // ============================

        player1Fill = CreateBar(
            canvasObject.transform,
            "Player1PushBar",

            // Mais para cima
            new Vector2(-300f, -70f),

            new Vector2(350f, 30f)
        );

        player1Text = CreateText(
            canvasObject.transform,
            "Player1PushText",
            "P1 - PRONTO",
            new Vector2(-300f, -105f)
        );

        // ============================
        // PLAYER 2
        // ============================

        player2Fill = CreateBar(
            canvasObject.transform,
            "Player2PushBar",

            // Mais para cima
            new Vector2(300f, -70f),

            new Vector2(350f, 30f)
        );

        player2Text = CreateText(
            canvasObject.transform,
            "Player2PushText",
            "P2 - PRONTO",
            new Vector2(300f, -105f)
        );
    }

    private RectTransform CreateBar(
        Transform parent,
        string name,
        Vector2 position,
        Vector2 size)
    {
        // ============================
        // FUNDO
        // ============================

        GameObject backgroundObject =
            new GameObject(
                name + "_Background",
                typeof(Image)
            );

        backgroundObject.transform.SetParent(
            parent,
            false
        );

        RectTransform backgroundRect =
            backgroundObject.GetComponent<RectTransform>();

        backgroundRect.anchorMin =
            new Vector2(0.5f, 1f);

        backgroundRect.anchorMax =
            new Vector2(0.5f, 1f);

        backgroundRect.pivot =
            new Vector2(0.5f, 1f);

        backgroundRect.anchoredPosition =
            position;

        backgroundRect.sizeDelta =
            size;

        Image background =
            backgroundObject.GetComponent<Image>();

        background.color =
            new Color(
                0.1f,
                0.1f,
                0.1f,
                0.8f
            );

        // ============================
        // PARTE VERDE
        // ============================

        GameObject fillObject =
            new GameObject(
                name + "_Fill",
                typeof(Image)
            );

        fillObject.transform.SetParent(
            backgroundObject.transform,
            false
        );

        RectTransform fillRect =
            fillObject.GetComponent<RectTransform>();

        // Começa no lado esquerdo
        fillRect.anchorMin =
            new Vector2(0f, 0f);

        // Inicialmente cheia
        fillRect.anchorMax =
            new Vector2(1f, 1f);

        fillRect.offsetMin =
            new Vector2(4f, 4f);

        fillRect.offsetMax =
            new Vector2(-4f, -4f);

        Image fillImage =
            fillObject.GetComponent<Image>();

        fillImage.color =
            Color.green;

        return fillRect;
    }

    private TMP_Text CreateText(
        Transform parent,
        string name,
        string initialText,
        Vector2 position)
    {
        GameObject textObject =
            new GameObject(
                name,
                typeof(TextMeshProUGUI)
            );

        textObject.transform.SetParent(
            parent,
            false
        );

        RectTransform rect =
            textObject.GetComponent<RectTransform>();

        rect.anchorMin =
            new Vector2(0.5f, 1f);

        rect.anchorMax =
            new Vector2(0.5f, 1f);

        rect.pivot =
            new Vector2(0.5f, 1f);

        rect.anchoredPosition =
            position;

        rect.sizeDelta =
            new Vector2(350f, 35f);

        TMP_Text text =
            textObject.GetComponent<TextMeshProUGUI>();

        text.text =
            initialText;

        text.fontSize =
            20f;

        text.alignment =
            TextAlignmentOptions.Center;

        return text;
    }
}