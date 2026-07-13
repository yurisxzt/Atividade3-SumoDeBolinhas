using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
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
        timer += Time.deltaTime;

        if(timer >= spawnInterval)
        {
            timer = 0f;

            SpawnCoin();
        }
    }

    void SpawnCoin()
    {
        Coin[] coins =
            FindObjectsByType<Coin>(
                FindObjectsSortMode.None);

        if(coins.Length >= maxCoins)
            return;

        Vector3 position =
            new Vector3(
                Random.Range(-arenaRadius, arenaRadius),
                0.5f,
                Random.Range(-arenaRadius, arenaRadius));

        Instantiate(
            coinPrefab,
            position,
            Quaternion.identity);
    }
}