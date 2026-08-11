using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Coins")]
    [SerializeField] private int coins = 0;

    public int Coins => coins;

    // Observer: avisa a interface quando a quantidade de moedas muda
    public event System.Action<int> OnCoinsChanged;

    [Header("Scaling")]
    [SerializeField] private float sizePerLevel = 0.15f;
    [SerializeField] private int coinsPerLevel = 5;

    [Header("Bonuses")]
    [SerializeField] private float forceBonusPerCoin = 25f;
    [SerializeField] private float resistanceBonusPerCoin = 0.05f;
    [SerializeField] private float speedPenaltyPerCoin = 0.1f;

    private Rigidbody rb;

    // Guarda o tamanho original definido pelo BolinhaData
    private Vector3 originalScale;

    // Aumenta a força de empurrão conforme coleta moedas
    public float ForceMultiplier =>
        1f + (coins * forceBonusPerCoin / 100f);

    // Aumenta a resistência conforme coleta moedas
    public float ResistanceMultiplier =>
        1f + (coins * resistanceBonusPerCoin);

    // Diminui a velocidade conforme coleta moedas
    // Nunca fica abaixo de 40% da velocidade original
    public float SpeedMultiplier =>
        Mathf.Max(
            0.4f,
            1f - (coins * speedPenaltyPerCoin / 10f)
        );

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Guarda o tamanho original da bolinha
        originalScale = transform.localScale;
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        coins += amount;

        // Avisa o CoinScoreboard
        OnCoinsChanged?.Invoke(coins);

        // A cada 5 moedas, aumenta o tamanho
        int level = coins / coinsPerLevel;

        transform.localScale =
            originalScale *
            (1f + level * sizePerLevel);

        // Aumenta a massa
        UpdateMass();
    }

    private void UpdateMass()
    {
        if (rb != null)
        {
            rb.mass =
                1f + coins * 0.2f;
        }
    }

    public void ResetStats()
    {
        coins = 0;

        transform.localScale =
            originalScale;

        UpdateMass();

        OnCoinsChanged?.Invoke(coins);
    }
}