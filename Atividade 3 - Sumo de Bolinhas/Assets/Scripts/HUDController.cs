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

    [Header("Coins")]
    [SerializeField]
    private int totalCoins = 9;

    private int currentCoins = 0;

    // Quantidade de moedas coletadas
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

    // Total de moedas da fase
    public int TotalCoins
    {
        get
        {
            return totalCoins;
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

        GameObject objectCoinText =
            GameObject.Find("CoinText");

        if (objectCoinText != null)
        {
            coinText =
                objectCoinText.GetComponent<TMP_Text>();
        }

        if (coinText == null)
        {
            Debug.LogWarning(
                "HUDController: CoinText não encontrado."
            );
        }
    }

    // =========================================================
    // LOCALIZAR PLAYER
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
    // EVENTO DAS MOEDAS
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
    // DEFINIR MOEDAS ATUAIS
    // =========================================================

    public void SetCoins(
        int value
    )
    {
        value =
            Mathf.Max(
                0,
                value
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
    // DEFINIR TOTAL DE MOEDAS
    // =========================================================

    public void SetTotalCoins(
        int total
    )
    {
        if (total < 0)
        {
            total = 0;
        }

        totalCoins = total;

        UpdateUI();
    }

    // =========================================================
    // RESETAR MOEDAS
    // =========================================================

    public void ResetCoins()
    {
        if (playerStats != null)
        {
            playerStats.SetCoins(0);

            return;
        }

        currentCoins = 0;

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

        // Exemplo:
        // Moedas: 0/9
        coinText.text =
            "Moedas: "
            + Coins
            + "/"
            + totalCoins;
    }
}