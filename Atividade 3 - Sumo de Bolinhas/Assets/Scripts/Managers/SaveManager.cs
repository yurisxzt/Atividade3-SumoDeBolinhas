using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    private string GetPath(int slot) => Path.Combine(Application.persistentDataPath, $"save{slot}.dat");
    private readonly byte[] key = Encoding.UTF8.GetBytes("a1b2c3d4e5f6g7h8"); // 16 bytes (example)
    private readonly byte[] iv = Encoding.UTF8.GetBytes("1a2b3c4d5e6f7g8h");

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); } else Destroy(gameObject);
    }

    public void SaveToSlot(int slot, SaveData data)
    {
        try
        {
            var json = JsonUtility.ToJson(data);
            var bytes = EncryptStringToBytes_Aes(json, key, iv);
            File.WriteAllBytes(GetPath(slot), bytes);
            // mirror to autosave slot 0 if not saving slot 0 itself
            if (slot != 0)
                File.WriteAllBytes(GetPath(0), bytes);
        }
        catch (Exception e)
        {
            Debug.LogError("Save failed: " + e);
        }
    }

    public SaveData LoadFromSlot(int slot)
    {
        try
        {
            var path = GetPath(slot);
            if (!File.Exists(path)) return null;
            var bytes = File.ReadAllBytes(path);
            var json = DecryptStringFromBytes_Aes(bytes, key, iv);
            var data = JsonUtility.FromJson<SaveData>(json);
            // mirror load into slot 0
            if (slot != 0)
                File.WriteAllBytes(GetPath(0), bytes);
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError("Load failed: " + e);
            return null;
        }
    }

    static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
    {
        if (plainText == null || plainText.Length <= 0) throw new ArgumentNullException(nameof(plainText));
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key; aesAlg.IV = IV;
            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            
                return ms.ToArray();
            }
        }
    }

    static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
    {
        if (cipherText == null || cipherText.Length <= 0) throw new ArgumentNullException(nameof(cipherText));
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key; aesAlg.IV = IV;
            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using (var ms = new MemoryStream(cipherText))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
