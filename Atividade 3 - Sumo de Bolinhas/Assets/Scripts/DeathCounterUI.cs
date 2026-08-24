using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathCounterUI : MonoBehaviour
{
    private TMP_Text player1Text;
    private TMP_Text player2Text;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;

        CreateUI();

        UpdateCounter();
    }

    private void Update()
    {
        if (gameManager == null)
            return;

        UpdateCounter();
    }

    private void UpdateCounter()
    {
        if (player1Text != null)
        {
            player1Text.text =
                "P1 Mortes: " +
                gameManager.Player1Quedas;
        }

        if (player2Text != null)
        {
            player2Text.text =
                "P2 Mortes: " +
                gameManager.Player2Quedas;
        }
    }

    private void CreateUI()
    {
        GameObject canvasObject =
            new GameObject(
                "DeathCounterCanvas",
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(UnityEngine.UI.GraphicRaycaster)
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

        // P1
        player1Text = CreateText(
            canvasObject.transform,
            "Player1Deaths",
            "P1 Mortes: 0",
            new Vector2(30f, -140f)
        );

        // P2
        player2Text = CreateText(
            canvasObject.transform,
            "Player2Deaths",
            "P2 Mortes: 0",
            new Vector2(-30f, -140f)
        );

        // P1 no canto superior esquerdo
        RectTransform p1Rect =
            player1Text.GetComponent<RectTransform>();

        p1Rect.anchorMin =
            new Vector2(0f, 1f);

        p1Rect.anchorMax =
            new Vector2(0f, 1f);

        p1Rect.pivot =
            new Vector2(0f, 1f);

        // P2 no canto superior direito
        RectTransform p2Rect =
            player2Text.GetComponent<RectTransform>();

        p2Rect.anchorMin =
            new Vector2(1f, 1f);

        p2Rect.anchorMax =
            new Vector2(1f, 1f);

        p2Rect.pivot =
            new Vector2(1f, 1f);
    }

    private TMP_Text CreateText(
        Transform parent,
        string objectName,
        string initialText,
        Vector2 position
    )
    {
        GameObject textObject =
            new GameObject(
                objectName,
                typeof(TextMeshProUGUI)
            );

        textObject.transform.SetParent(
            parent,
            false
        );

        RectTransform rect =
            textObject.GetComponent<RectTransform>();

        rect.anchoredPosition =
            position;

        rect.sizeDelta =
            new Vector2(300f, 50f);

        TMP_Text text =
            textObject.GetComponent<TextMeshProUGUI>();

        text.text = initialText;
        text.fontSize = 28;
        text.alignment =
            TextAlignmentOptions.Center;

        return text;
    }
}