using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Coins")]
    public int coins;

    public int Coins => coins;

    public event System.Action<int> OnCoinsChanged;

    [Header("Scaling")]
    public float sizePerLevel = 0.15f;
    public int coinsPerLevel = 5;

    [Header("Bonuses")]
    public float forceBonusPerCoin = 25f;
    public float resistanceBonusPerCoin = 0.05f;
    public float speedPenaltyPerCoin = 0.1f;

    private Rigidbody rb;

    public float ForceMultiplier =>
        1f + (coins * forceBonusPerCoin / 100f);

    public float ResistanceMultiplier =>
        1f + (coins * resistanceBonusPerCoin);

    public float SpeedMultiplier =>
        Mathf.Max(0.4f, 1f - (coins * speedPenaltyPerCoin / 10f));

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void AddCoins(int amount)
    {
        coins += amount;

        OnCoinsChanged?.Invoke(coins);

        int level = coins / coinsPerLevel;

        transform.localScale =
            Vector3.one * (1f + level * sizePerLevel);

        UpdateMass();
    }

    void UpdateMass()
    {
        if (rb != null)
        {
            rb.mass = 1f + coins * 0.2f;
        }
    }
}