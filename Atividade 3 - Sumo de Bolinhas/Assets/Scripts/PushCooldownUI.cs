using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PushCooldownUI : MonoBehaviour
{
    private PlayerSpawner spawner;

    private Image player1Bar;
    private Image player2Bar;

    private TMP_Text player1Text;
    private TMP_Text player2Text;

    private void Start()
    {
        spawner = GetComponent<PlayerSpawner>();

        CreateUI();
    }

    private void Update()
    {
        if (spawner == null)
            return;

        if (spawner.Player1Controller != null)
        {
            player1Bar.fillAmount =
                spawner.Player1Controller.PushCooldownNormalized;

            UpdateText(
                player1Text,
                spawner.Player1Controller.CanPush
            );
        }

        if (spawner.Player2Controller != null)
        {
            player2Bar.fillAmount =
                spawner.Player2Controller.PushCooldownNormalized;

            UpdateText(
                player2Text,
                spawner.Player2Controller.CanPush
            );
        }
    }

    private void UpdateText(TMP_Text text, bool canPush)
    {
        if (text == null)
            return;

        text.text = canPush
            ? "EMPURRÃO PRONTO"
            : "CARREGANDO...";
    }

    private void CreateUI()
    {
        GameObject canvasObject = new GameObject(
            "PushCooldownCanvas",
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );

        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler =
            canvasObject.GetComponent<CanvasScaler>();

        scaler.uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;

        scaler.referenceResolution =
            new Vector2(1920f, 1080f);

        // Jogador 1
        player1Bar = CreateBar(
            canvasObject.transform,
            "Player1PushBar",
            new Vector2(250f, -100f),
            new Vector2(400f, 35f)
        );

        player1Text = CreateText(
            canvasObject.transform,
            "Player1PushText",
            new Vector2(250f, -145f)
        );

        // Jogador 2
        player2Bar = CreateBar(
            canvasObject.transform,
            "Player2PushBar",
            new Vector2(-250f, -100f),
            new Vector2(400f, 35f)
        );

        player2Text = CreateText(
            canvasObject.transform,
            "Player2PushText",
            new Vector2(-250f, -145f)
        );
    }

    private Image CreateBar(
        Transform parent,
        string name,
        Vector2 position,
        Vector2 size
    )
    {
        GameObject barObject =
            new GameObject(name, typeof(Image));

        barObject.transform.SetParent(parent, false);

        RectTransform rect =
            barObject.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);

        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image =
            barObject.GetComponent<Image>();

        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Horizontal;
        image.fillOrigin = (int)Image.OriginHorizontal.Left;
        image.fillAmount = 1f;

        return image;
    }

    private TMP_Text CreateText(
        Transform parent,
        string name,
        Vector2 position
    )
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

        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);

        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(400f, 40f);

        TMP_Text text =
            textObject.GetComponent<TextMeshProUGUI>();

        text.fontSize = 22;
        text.alignment =
            TextAlignmentOptions.Center;

        text.text = "EMPURRÃO PRONTO";

        return text;
    }
}