using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveRestoreManager : MonoBehaviour
{
    public static SaveRestoreManager Instance { get; private set; }

    private SaveData pendingSave;
    private int pendingSlot = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RequestLoadSlot(int slot)
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("SaveManager not found");
            return;
        }

        SaveData data = SaveManager.Instance.LoadFromSlot(slot);
        if (data == null)
        {
            Debug.LogWarning($"No save in slot {slot}");
            return;
        }

        pendingSave = data;
        pendingSlot = slot;

        if (!string.IsNullOrEmpty(data.sceneName) && data.sceneName != SceneManager.GetActiveScene().name)
        {
            StartCoroutine(LoadSceneAndApplySave(data.sceneName));
        }
        else
        {
            ApplySaveToCurrentScene(data);
        }
    }

    private IEnumerator LoadSceneAndApplySave(string sceneName)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ForceSceneChange(sceneName);
            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }

        ApplySaveToCurrentScene(pendingSave);
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveToSlot(0, pendingSave);
        }
    }

    private void ApplySaveToCurrentScene(SaveData data)
    {
        if (data == null)
        {
            return;
        }

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SetCurrentCollectedCoins(data.collectedCoinIds);
        }

        var player = FindObjectOfType<TwoBallController>();
        if (player != null)
        {
            player.ApplyLoadedData(data);
        }

        var hud = FindObjectOfType<HUDController>();
        if (hud != null)
        {
            hud.SetCoins(data.coins);
        }

        foreach (Coin coin in FindObjectsByType<Coin>(FindObjectsSortMode.None))
        {
            if (coin == null)
            {
                continue;
            }

            coin.gameObject.SetActive(!SaveManager.Instance.IsCoinCollected(coin.CoinId));
        }

        pendingSave = null;
        pendingSlot = -1;
    }
}