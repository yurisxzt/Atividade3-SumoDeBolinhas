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

    private Rigidbody rb;
    private Vector3 originalScale;

    public float ForceMultiplier =>
        1f + (coins * forceBonusPerCoin / 100f);

    public float ResistanceMultiplier =>
        1f + (coins * resistanceBonusPerCoin);

    public float SpeedMultiplier =>
        Mathf.Max(0.4f, 1f - (coins * speedPenaltyPerCoin / 10f));

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

        // Atualiza a interface através do Observer
        OnCoinsChanged?.Invoke(coins);

        // A cada 5 moedas, aumenta o tamanho
        int level = coins / coinsPerLevel;

        transform.localScale =
            originalScale * (1f + level * sizePerLevel);

        // Aumenta a massa conforme as moedas
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

        transform.localScale = originalScale;

        UpdateMass();

        OnCoinsChanged?.Invoke(coins);
    }
}