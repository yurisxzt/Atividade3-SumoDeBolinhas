using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Spawn manual de moedas")]
    [SerializeField]
    private bool allowRandomSpawning = false;

    [SerializeField]
    private GameObject coinPrefab;

    [SerializeField]
    private float spawnInterval = 3f;

    [SerializeField]
    private int maxCoins = 10;

    [SerializeField]
    private float arenaRadius = 8f;

    private float timer;

    private void Update()
    {
        if (!allowRandomSpawning)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnCoin();
        }
    }

    private void SpawnCoin()
    {
        if (coinPrefab == null)
        {
            return;
        }

        Coin[] coins = FindObjectsByType<Coin>(FindObjectsSortMode.None);

        if (coins.Length >= maxCoins)
        {
            return;
        }

        Vector3 position = new Vector3(
            Random.Range(-arenaRadius, arenaRadius),
            0.5f,
            Random.Range(-arenaRadius, arenaRadius));

        Instantiate(coinPrefab, position, Quaternion.identity);
    }
}