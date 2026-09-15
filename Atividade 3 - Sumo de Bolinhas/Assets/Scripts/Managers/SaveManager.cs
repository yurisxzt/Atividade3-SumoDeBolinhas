using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    private const int SlotCount = 3;
    private const string KeyString = "12345678901234567890123456789012";
    private const string IvString = "1234567890123456";

    public static SaveManager Instance { get; private set; }

    private readonly List<string> currentCollectedCoins = new List<string>();

    public IReadOnlyCollection<string> CurrentCollectedCoins => currentCollectedCoins;

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

    public string GetPath(int slot)
    {
        if (slot < 0 || slot >= SlotCount)
        {
            slot = 0;
        }

        return Path.Combine(Application.persistentDataPath, $"sumo_save_slot_{slot}.sav");
    }

    public bool SlotExists(int slot)
    {
        return File.Exists(GetPath(slot));
    }

    public void MarkCoinCollected(string coinId)
    {
        if (string.IsNullOrWhiteSpace(coinId))
        {
            return;
        }

        if (!currentCollectedCoins.Contains(coinId))
        {
            currentCollectedCoins.Add(coinId);
        }
    }

    public void SetCurrentCollectedCoins(IEnumerable<string> collectedIds)
    {
        currentCollectedCoins.Clear();

        if (collectedIds == null)
        {
            return;
        }

        foreach (string id in collectedIds)
        {
            if (!string.IsNullOrWhiteSpace(id) && !currentCollectedCoins.Contains(id))
            {
                currentCollectedCoins.Add(id);
            }
        }
    }

    public void ClearCollectedCoins()
    {
        currentCollectedCoins.Clear();
    }

    public bool IsCoinCollected(string coinId)
    {
        if (string.IsNullOrWhiteSpace(coinId))
        {
            return false;
        }

        if (currentCollectedCoins.Contains(coinId))
        {
            return true;
        }

        for (int i = 0; i < SlotCount; i++)
        {
            SaveData data = LoadFromSlot(i);
            if (data != null && data.collectedCoinIds != null && data.collectedCoinIds.Contains(coinId))
            {
                return true;
            }
        }

        return false;
    }

    public void SaveToSlot(int slot, SaveData data)
    {
        if (slot < 0 || slot >= SlotCount)
        {
            slot = 0;
        }

        SaveData save = data ?? new SaveData();
        save.sceneName = string.IsNullOrEmpty(save.sceneName) ? SceneManager.GetActiveScene().name : save.sceneName;
        save.collectedCoinIds ??= new List<string>();
        save.collectedCoinIds = NormalizeIds(save.collectedCoinIds);

        string json = JsonUtility.ToJson(save);
        string encrypted = EncryptString(json);
        File.WriteAllText(GetPath(slot), encrypted);

        if (slot != 0)
        {
            File.WriteAllText(GetPath(0), encrypted);
        }

        currentCollectedCoins.Clear();
        foreach (string id in save.collectedCoinIds)
        {
            currentCollectedCoins.Add(id);
        }

        Debug.Log($"SaveManager: slot {slot} salvo em {GetPath(slot)}");
    }

    public SaveData LoadFromSlot(int slot)
    {
        if (slot < 0 || slot >= SlotCount)
        {
            return null;
        }

        string path = GetPath(slot);
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            string encrypted = File.ReadAllText(path);
            string json = DecryptString(encrypted);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            if (data == null)
            {
                return null;
            }

            if (data.collectedCoinIds == null)
            {
                data.collectedCoinIds = new List<string>();
            }

            SetCurrentCollectedCoins(data.collectedCoinIds);
            return data;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"SaveManager: falha ao carregar slot {slot}. {ex.Message}");
            return null;
        }
    }

    public void CopySlotToAutosave(int slot)
    {
        SaveData data = LoadFromSlot(slot);
        if (data != null)
        {
            SaveToSlot(0, data);
        }
    }

    private static List<string> NormalizeIds(IEnumerable<string> ids)
    {
        List<string> normalized = new List<string>();

        if (ids == null)
        {
            return normalized;
        }

        foreach (string id in ids)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                continue;
            }

            if (!normalized.Contains(id))
            {
                normalized.Add(id);
            }
        }

        return normalized;
    }

    private static string EncryptString(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(KeyString);
        aes.IV = Encoding.UTF8.GetBytes(IvString);
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using MemoryStream ms = new MemoryStream();
        using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    private static string DecryptString(string encryptedText)
    {
        byte[] bytes = Convert.FromBase64String(encryptedText);

        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(KeyString);
        aes.IV = Encoding.UTF8.GetBytes(IvString);
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using MemoryStream ms = new MemoryStream(bytes);
        using CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using StreamReader sr = new StreamReader(cs, Encoding.UTF8);
        return sr.ReadToEnd();
    }
}
