using UnityEngine;

/// <summary>
/// Simple helper to spawn a grid of coins at scene start for testing.
/// Assign a Coin prefab and configure rows/cols and spacing.
/// </summary>
public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public int rows = 3;
    public int cols = 5;
    public Vector3 start = new Vector3(-4, 1, -2);
    public Vector3 spacing = new Vector3(1.5f, 0, 1.5f);

    private void Start()
    {
        if (coinPrefab == null) return;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Vector3 pos = start + new Vector3(spacing.x * c, spacing.y * r, spacing.z * c);
                Instantiate(coinPrefab, pos, Quaternion.identity);
            }
        }
    }
}

