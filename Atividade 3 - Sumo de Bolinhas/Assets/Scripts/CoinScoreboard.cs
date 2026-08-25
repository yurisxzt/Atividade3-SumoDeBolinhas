using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinScoreboard : MonoBehaviour
{
    private PlayerSpawner spawner;

    private TMP_Text player1Text;
    private TMP_Text player2Text;

    private void Start()
    {
        spawner = GetComponent<PlayerSpawner>();

        CreateScoreboardUi();

        BindStats();

        RefreshScoreboard();
    }

    private void OnDestroy()
    {
        if (spawner == null)
            return;

        if (spawner.Player1Stats != null)
        {
            spawner.Player1Stats.OnCoinsChanged -= UpdatePlayer1Score;
        }

        if (spawner.Player2Stats != null)
        {
            spawner.Player2Stats.OnCoinsChanged -= UpdatePlayer2Score;
        }
    }

    private void BindStats()
    {
        if (spawner == null)
            return;

        if (spawner.Player1Stats != null)
        {
            spawner.Player1Stats.OnCoinsChanged += UpdatePlayer1Score;
        }

        if (spawner.Player2Stats != null)
        {
            spawner.Player2Stats.OnCoinsChanged += UpdatePlayer2Score;
        }
    }

    private void RefreshScoreboard()
    {
        if (player1Text != null &&
            spawner != null &&
            spawner.Player1Stats != null)
        {
            player1Text.text =
                "P1 Moedas: " +
                spawner.Player1Stats.Coins;
        }

        if (player2Text != null &&
            spawner != null &&
            spawner.Player2Stats != null)
        {
            player2Text.text =
                "P2 Moedas: " +
                spawner.Player2Stats.Coins;
        }
    }

    private void UpdatePlayer1Score(int value)
    {
        if (player1Text != null)
        {
            player1Text.text =
                "P1 Moedas: " + value;
        }
    }

    private void UpdatePlayer2Score(int value)
    {
        if (player2Text != null)
        {
            player2Text.text =
                "P2 Moedas: " + value;
        }
    }

    private void CreateScoreboardUi()
    {
        GameObject canvasObject =
            new GameObject(
                "CoinScoreboardCanvas",
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

        // PLAYER 1 - LADO ESQUERDO
        player1Text = CreateText(
            canvasObject.transform,
            "Player1CoinText",
            "P1 Moedas: 0",
            new Vector2(30f, -30f),
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            TextAlignmentOptions.TopLeft
        );

        // PLAYER 2 - LADO DIREITO
        player2Text = CreateText(
            canvasObject.transform,
            "Player2CoinText",
            "P2 Moedas: 0",
            new Vector2(-30f, -30f),
            new Vector2(1f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 1f),
            TextAlignmentOptions.TopRight
        );
    }

    private TMP_Text CreateText(
        Transform parent,
        string objectName,
        string initialText,
        Vector2 position,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        TextAlignmentOptions alignment)
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

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;

        rect.anchoredPosition =
            position;

        rect.sizeDelta =
            new Vector2(350f, 50f);

        TMP_Text text =
            textObject.GetComponent<TextMeshProUGUI>();

        text.text = initialText;
        text.fontSize = 28f;
        text.alignment = alignment;

        return text;
    }
}