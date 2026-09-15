using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    // =========================================================
    // INTERFACE
    // =========================================================

    [Header("Interface")]
    [SerializeField]
    private TMP_Text coinText;

    // =========================================================
    // PLAYER
    // =========================================================

    [Header("Player")]
    [SerializeField]
    private PlayerStats playerStats;

    // =========================================================
    // MOEDAS
    // =========================================================

    // Fase 1 possui exatamente 9 moedas
    private const int totalCoins = 9;

    private int currentCoins = 0;

    public int Coins
    {
        get
        {
            if (playerStats != null)
            {
                return playerStats.Coins;
            }

            return currentCoins;
        }
    }

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        ResolveCoinText();

        ResolvePlayerStats();

        BindPlayerStats();

        if (playerStats != null)
        {
            currentCoins =
                playerStats.Coins;
        }

        UpdateUI();
    }

    // =========================================================
    // DESTROY
    // =========================================================

    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnCoinsChanged -=
                OnCoinsChanged;
        }
    }

    // =========================================================
    // LOCALIZAR TEXTO
    // =========================================================

    private void ResolveCoinText()
    {
        if (coinText != null)
        {
            return;
        }

        coinText =
            GetComponentInChildren<TMP_Text>(
                true
            );

        if (coinText != null)
        {
            return;
        }

        GameObject coinTextObject =
            GameObject.Find("CoinText");

        if (coinTextObject != null)
        {
            coinText =
                coinTextObject
                    .GetComponent<TMP_Text>();
        }

        if (coinText == null)
        {
            Debug.LogWarning(
                "HUDController: CoinText não encontrado."
            );
        }
    }

    // =========================================================
    // LOCALIZAR PLAYER STATS
    // =========================================================

    private void ResolvePlayerStats()
    {
        if (playerStats != null)
        {
            return;
        }

        playerStats =
            FindFirstObjectByType<PlayerStats>();

        if (playerStats == null)
        {
            Debug.LogWarning(
                "HUDController: PlayerStats não encontrado."
            );
        }
    }

    // =========================================================
    // CONECTAR EVENTO
    // =========================================================

    private void BindPlayerStats()
    {
        if (playerStats == null)
        {
            return;
        }

        playerStats.OnCoinsChanged -=
            OnCoinsChanged;

        playerStats.OnCoinsChanged +=
            OnCoinsChanged;
    }

    // =========================================================
    // QUANDO PEGAR MOEDA
    // =========================================================

    private void OnCoinsChanged(
        int value
    )
    {
        currentCoins = value;

        UpdateUI();
    }

    // =========================================================
    // ADICIONAR MOEDA
    // =========================================================

    public void AddCoin()
    {
        if (playerStats != null)
        {
            playerStats.AddCoins(1);

            return;
        }

        currentCoins++;

        UpdateUI();
    }

    // =========================================================
    // DEFINIR MOEDAS
    // =========================================================

    public void SetCoins(
        int value
    )
    {
        value =
            Mathf.Clamp(
                value,
                0,
                totalCoins
            );

        if (playerStats != null)
        {
            playerStats.SetCoins(value);

            return;
        }

        currentCoins = value;

        UpdateUI();
    }

    // =========================================================
    // ATUALIZAR INTERFACE
    // =========================================================

    private void UpdateUI()
    {
        if (coinText == null)
        {
            return;
        }

        // Texto preto
        coinText.color =
            Color.black;

        // Exemplo: Moedas: 0/9
        coinText.text =
            "Moedas: "
            + Coins
            + "/"
            + totalCoins;
    }
}