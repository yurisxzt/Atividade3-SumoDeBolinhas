using UnityEngine;
using UnityEngine.SceneManagement;

public class Coin : MonoBehaviour
{
    [SerializeField]
    private int value = 1;

    [SerializeField]
    private string coinId = "";

    public string CoinId => coinId;

    private void Awake()
    {
        if (string.IsNullOrWhiteSpace(coinId))
        {
            coinId = $"{SceneManager.GetActiveScene().name}_{gameObject.name}_{Mathf.RoundToInt(transform.position.x * 100f)}_{Mathf.RoundToInt(transform.position.z * 100f)}";
        }
    }

    private void Start()
    {
        ApplySaveState();
    }

    private void OnEnable()
    {
        ApplySaveState();
    }

    private void ApplySaveState()
    {
        if (SaveManager.Instance == null)
        {
            gameObject.SetActive(true);
            return;
        }

        SaveData current = SaveManager.Instance.LoadFromSlot(0);
        bool alreadyCollected = false;

        if (current != null && current.sceneName == SceneManager.GetActiveScene().name && current.collectedCoinIds != null)
        {
            alreadyCollected = ContainsCoinId(current.collectedCoinIds, coinId);
        }

        if (!alreadyCollected)
        {
            alreadyCollected = ContainsCoinId(SaveManager.Instance.CurrentCollectedCoins, coinId);
        }

        if (!alreadyCollected)
        {
            alreadyCollected = SaveManager.Instance.IsCoinCollected(coinId);
        }

        gameObject.SetActive(!alreadyCollected);
    }

    private static bool ContainsCoinId(System.Collections.Generic.IEnumerable<string> ids, string targetId)
    {
        if (string.IsNullOrWhiteSpace(targetId) || ids == null)
        {
            return false;
        }

        foreach (string id in ids)
        {
            if (string.Equals(id, targetId))
            {
                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (!gameObject.activeSelf)
        {
            return;
        }

        PlayerStats stats = other.GetComponentInParent<PlayerStats>();
        if (stats == null)
        {
            return;
        }

        stats.AddCoins(value);

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.MarkCoinCollected(coinId);
        }

        var player = other.GetComponentInParent<TwoBallController>();
        if (player != null)
        {
            player.SaveCurrentProgress();
        }

        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, 180f * Time.deltaTime);
    }
}