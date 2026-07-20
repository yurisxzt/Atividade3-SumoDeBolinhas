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
        if (spawner != null)
        {
            if (spawner.Player1Stats != null)
            {
                spawner.Player1Stats.OnCoinsChanged -= UpdatePlayer1Score;
            }

            if (spawner.Player2Stats != null)
            {
                spawner.Player2Stats.OnCoinsChanged -= UpdatePlayer2Score;
            }
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
        if (player1Text != null && spawner?.Player1Stats != null)
        {
            player1Text.text = $"Jogador 1: {spawner.Player1Stats.Coins}";
        }

        if (player2Text != null && spawner?.Player2Stats != null)
        {
            player2Text.text = $"Jogador 2: {spawner.Player2Stats.Coins}";
        }
    }

    private void UpdatePlayer1Score(int value)
    {
        if (player1Text != null)
        {
            player1Text.text = $"Jogador 1: {value}";
        }
    }

    private void UpdatePlayer2Score(int value)
    {
        if (player2Text != null)
        {
            player2Text.text = $"Jogador 2: {value}";
        }
    }

    private void CreateScoreboardUi()
    {
        if (player1Text != null && player2Text != null)
            return;

        GameObject canvasObject = new GameObject("CoinScoreboardCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        player1Text = CreateText(canvasObject.transform, "Player1CoinText", new Vector2(30f, -30f), new Vector2(0f, 1f), new Vector2(0f, 1f));
        player2Text = CreateText(canvasObject.transform, "Player2CoinText", new Vector2(30f, -80f), new Vector2(0f, 1f), new Vector2(0f, 1f));

        player1Text.text = "Jogador 1: 0";
        player2Text.text = "Jogador 2: 0";
    }

    private TMP_Text CreateText(Transform parent, string name, Vector2 anchorPosition, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject textObject = new GameObject(name, typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = anchorPosition;
        rectTransform.sizeDelta = new Vector2(500f, 40f);

        TMP_Text text = textObject.GetComponent<TextMeshProUGUI>();
        text.fontSize = 28;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.enableVertexGradient = true;

        return text;
    }
}
