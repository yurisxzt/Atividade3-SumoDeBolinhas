using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Coins")]
    [SerializeField] private int coins = 0;

    public int Coins => coins;

    public event System.Action<int> OnCoinsChanged;

    private Rigidbody rb;
    private Vector3 originalScale;

    public float ForceMultiplier => 1f;
    public float ResistanceMultiplier => 1f;
    public float SpeedMultiplier => 1f;

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
        OnCoinsChanged?.Invoke(coins);
    }

    public void SetCoins(int value)
    {
        coins = Mathf.Max(0, value);
        OnCoinsChanged?.Invoke(coins);
    }

    public void ResetStats()
    {
        coins = 0;
        transform.localScale = originalScale;
        if (rb != null)
        {
            rb.mass = 1f;
        }

        OnCoinsChanged?.Invoke(coins);
    }
}