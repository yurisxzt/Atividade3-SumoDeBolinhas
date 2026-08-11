using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Coins")]
    [SerializeField] private int coins = 0;

    public int Coins => coins;

    public event System.Action<int> OnCoinsChanged;

    [Header("Scaling")]
    [SerializeField] private float sizePerLevel = 0.15f;
    [SerializeField] private int coinsPerLevel = 5;

    [Header("Bonuses")]
    [SerializeField] private float forceBonusPerCoin = 25f;
    [SerializeField] private float resistanceBonusPerCoin = 0.05f;
    [SerializeField] private float speedPenaltyPerCoin = 0.1f;

    [Header("Coin Pickup Boost")]
    [SerializeField] private float coinSpeedBoost = 0.15f;
    [SerializeField] private float coinForceBoost = 0.10f;

    private Rigidbody rb;
    private Vector3 originalScale;

    // Bônus acumulado das moedas
    private float temporarySpeedBoost = 0f;
    private float temporaryForceBoost = 0f;

    public float ForceMultiplier =>
        (1f + (coins * forceBonusPerCoin / 100f))
        * (1f + temporaryForceBoost);

    public float ResistanceMultiplier =>
        1f + (coins * resistanceBonusPerCoin);

    public float SpeedMultiplier =>
        Mathf.Max(
            0.4f,
            (1f - (coins * speedPenaltyPerCoin / 10f))
            + temporarySpeedBoost
        );

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalScale = transform.localScale;
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        coins += amount;

        // Pequeno bônus instantâneo/acumulativo
        temporarySpeedBoost += coinSpeedBoost;
        temporaryForceBoost += coinForceBoost;

        // Atualiza o placar
        OnCoinsChanged?.Invoke(coins);

        // A cada 5 moedas aumenta o tamanho
        int level = coins / coinsPerLevel;

        transform.localScale =
            originalScale *
            (1f + level * sizePerLevel);

        UpdateMass();
    }

    private void UpdateMass()
    {
        if (rb != null)
        {
            rb.mass = 1f + coins * 0.2f;
        }
    }

    public void ResetStats()
    {
        coins = 0;

        temporarySpeedBoost = 0f;
        temporaryForceBoost = 0f;

        transform.localScale = originalScale;

        UpdateMass();

        OnCoinsChanged?.Invoke(coins);
    }
}