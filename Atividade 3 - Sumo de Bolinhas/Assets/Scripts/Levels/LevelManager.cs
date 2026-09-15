using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("Fase seguinte")]
    public string nextLevelSceneName = "Fase2";

    [Header("Mensagem")]
    public string victoryMessage = "Vitória! Pressione Espaço para continuar.";

    [Header("UI de Vitória")]
    public GameObject victoryPanel;
    public Text victoryTitleText;
    public Text victoryCoinsText;
    public Button nextLevelButton;

    private bool waitingForNextLevel;
    private int totalCoinsInLevel;

    private void Start()
    {
        totalCoinsInLevel = FindObjectsByType<Coin>(FindObjectsSortMode.None).Length;
        EnsureUiReferences();
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (nextLevelButton != null) nextLevelButton.gameObject.SetActive(false);

        HUDController hud = FindFirstObjectByType<HUDController>();
        if (hud != null)
        {
            hud.SetTotalCoins(totalCoinsInLevel);
        }
    }

    private void Update()
    {
        if (!waitingForNextLevel)
        {
            return;
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            GoToNextLevel();
        }
    }

    public void OnVictory()
    {
        if (waitingForNextLevel)
        {
            return;
        }

        waitingForNextLevel = true;

        HUDController hud = FindFirstObjectByType<HUDController>();
        int collectedCoins = hud != null ? hud.Coins : 0;
        ShowVictoryPanel(collectedCoins, totalCoinsInLevel);
        Debug.Log(victoryMessage + " Total: " + collectedCoins + " / " + totalCoinsInLevel);

        if (SaveManager.Instance != null)
        {
            var save = new SaveData
            {
                sceneName = SceneManager.GetActiveScene().name,
                coins = collectedCoins,
                checkpointPassed = true,
                collectedCoinIds = new System.Collections.Generic.List<string>(SaveManager.Instance.CurrentCollectedCoins)
            };

            SaveManager.Instance.SaveToSlot(0, save);
        }
    }

    private void ShowVictoryPanel(int collectedCoins, int totalCoins)
    {
        EnsureUiReferences();

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        if (victoryTitleText != null)
        {
            victoryTitleText.text = "Vitória!";
        }

        if (victoryCoinsText != null)
        {
            victoryCoinsText.text = $"Você coletou {collectedCoins} de {totalCoins} moedas.";
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(true);
            nextLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.AddListener(GoToNextLevel);
        }
    }

    public void GoToNextLevel()
    {
        if (string.IsNullOrEmpty(nextLevelSceneName))
        {
            Debug.LogWarning("LevelManager: nextLevelSceneName está vazio.");
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ForceSceneChange(nextLevelSceneName);
            return;
        }

        SceneManager.LoadScene(nextLevelSceneName);
    }

    private void EnsureUiReferences()
    {
        if (victoryPanel == null)
        {
            victoryPanel = GameObject.Find("VictoryPanel");
        }

        if (victoryTitleText == null)
        {
            Text titleText = GameObject.Find("VictoryTitleText")?.GetComponent<Text>();
            if (titleText != null)
            {
                victoryTitleText = titleText;
            }
        }

        if (victoryCoinsText == null)
        {
            Text coinsText = GameObject.Find("VictoryCoinsText")?.GetComponent<Text>();
            if (coinsText != null)
            {
                victoryCoinsText = coinsText;
            }
        }

        if (nextLevelButton == null)
        {
            Button button = GameObject.Find("NextLevelButton")?.GetComponent<Button>();
            if (button != null)
            {
                nextLevelButton = button;
            }
        }
    }
}
