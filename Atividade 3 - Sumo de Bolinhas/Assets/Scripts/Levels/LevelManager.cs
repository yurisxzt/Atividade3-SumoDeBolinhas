using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    // =========================================================
    // PRÓXIMA FASE
    // =========================================================

    [Header("Fase seguinte")]
    [SerializeField]
    private string nextLevelSceneName = "Fase2";

    // =========================================================
    // MOEDAS
    // =========================================================

    [Header("Moedas da fase")]
    [SerializeField]
    private int totalCoinsInLevel = 9;

    // =========================================================
    // VITÓRIA
    // =========================================================

    [Header("Vitória")]
    [SerializeField]
    private string victoryMessage =
        "Vitória! Pressione Espaço para continuar.";

    [SerializeField]
    private GameObject victoryPanel;

    [SerializeField]
    private Text victoryTitleText;

    [SerializeField]
    private Text victoryCoinsText;

    [SerializeField]
    private Button nextLevelButton;

    private bool waitingForNextLevel = false;

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // Atualiza o HUD com o total de moedas da fase
        HUDController hud =
            FindFirstObjectByType<HUDController>();

        if (hud != null)
        {
            hud.SetTotalCoins(
                totalCoinsInLevel
            );
        }

        // Esconde painel de vitória no início
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(false);
        }
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (!waitingForNextLevel)
        {
            return;
        }

        // Espaço continua para próxima fase
        if (
            Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame
        )
        {
            GoToNextLevel();
        }
    }

    // =========================================================
    // VITÓRIA
    // =========================================================

    public void OnVictory()
    {
        if (waitingForNextLevel)
        {
            return;
        }

        waitingForNextLevel = true;

        HUDController hud =
            FindFirstObjectByType<HUDController>();

        int collectedCoins = 0;

        if (hud != null)
        {
            collectedCoins =
                hud.Coins;
        }

        ShowVictoryPanel(
            collectedCoins,
            totalCoinsInLevel
        );

        Debug.Log(
            victoryMessage
            + " Moedas: "
            + collectedCoins
            + "/"
            + totalCoinsInLevel
        );
    }

    // =========================================================
    // MOSTRAR PAINEL
    // =========================================================

    private void ShowVictoryPanel(
        int collectedCoins,
        int totalCoins
    )
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        if (victoryTitleText != null)
        {
            victoryTitleText.text =
                "Vitória!";
        }

        if (victoryCoinsText != null)
        {
            victoryCoinsText.text =
                "Moedas: "
                + collectedCoins
                + "/"
                + totalCoins;
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(true);

            nextLevelButton.onClick.RemoveAllListeners();

            nextLevelButton.onClick.AddListener(
                GoToNextLevel
            );
        }
    }

    // =========================================================
    // PRÓXIMA FASE
    // =========================================================

    public void GoToNextLevel()
    {
        if (
            string.IsNullOrEmpty(
                nextLevelSceneName
            )
        )
        {
            Debug.LogWarning(
                "LevelManager: próxima fase não configurada."
            );

            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ForceSceneChange(
                nextLevelSceneName
            );

            return;
        }

        SceneManager.LoadScene(
            nextLevelSceneName
        );
    }
}