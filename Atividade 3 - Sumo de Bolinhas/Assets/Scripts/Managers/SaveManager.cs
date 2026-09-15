using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    // =========================================================
    // SLOTS
    // =========================================================

    public const int AutosaveSlot = 0;

    public const int FirstManualSlot = 1;

    public const int LastManualSlot = 3;

    private const int SlotCount = 4;

    // =========================================================
    // ENCRIPTAÇÃO
    // =========================================================

    private const string KeyString =
        "12345678901234567890123456789012";

    private const string IvString =
        "1234567890123456";

    // =========================================================
    // SINGLETON
    // =========================================================

    public static SaveManager Instance
    {
        get;
        private set;
    }

    // =========================================================
    // MOEDAS DA PARTIDA ATUAL
    // =========================================================

    private readonly List<string>
        currentCollectedCoins =
            new List<string>();

    public IReadOnlyCollection<string>
        CurrentCollectedCoins =>
            currentCollectedCoins;

    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(
                gameObject
            );
        }
        else
        {
            Destroy(
                gameObject
            );
        }
    }

    // =========================================================
    // CAMINHO DO SAVE
    // =========================================================

    public string GetPath(
        int slot
    )
    {
        if (!IsValidSlot(slot))
        {
            slot = AutosaveSlot;
        }

        return Path.Combine(
            Application.persistentDataPath,
            "platform_save_slot_"
            + slot
            + ".sav"
        );
    }

    // =========================================================
    // VERIFICAR SLOT
    // =========================================================

    private bool IsValidSlot(
        int slot
    )
    {
        return slot >= 0 &&
               slot < SlotCount;
    }

    public bool SlotExists(
        int slot
    )
    {
        if (!IsValidSlot(slot))
        {
            return false;
        }

        return File.Exists(
            GetPath(slot)
        );
    }

    // =========================================================
    // APAGAR SLOT
    // =========================================================

    public void DeleteSlot(
        int slot
    )
    {
        if (!IsValidSlot(slot))
        {
            return;
        }

        string path =
            GetPath(slot);

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        if (slot == AutosaveSlot)
        {
            ClearCollectedCoins();
        }
    }

    // =========================================================
    // MOEDAS
    // =========================================================

    public void MarkCoinCollected(
        string coinId
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                coinId
            )
        )
        {
            return;
        }

        if (
            !currentCollectedCoins.Contains(
                coinId
            )
        )
        {
            currentCollectedCoins.Add(
                coinId
            );
        }
    }

    public void SetCurrentCollectedCoins(
        IEnumerable<string> collectedIds
    )
    {
        currentCollectedCoins.Clear();

        if (collectedIds == null)
        {
            return;
        }

        foreach (
            string id
            in collectedIds
        )
        {
            if (
                string.IsNullOrWhiteSpace(
                    id
                )
            )
            {
                continue;
            }

            if (
                !currentCollectedCoins.Contains(
                    id
                )
            )
            {
                currentCollectedCoins.Add(
                    id
                );
            }
        }
    }

    public void ClearCollectedCoins()
    {
        currentCollectedCoins.Clear();
    }

    // =========================================================
    // VERIFICAR MOEDA
    // =========================================================

    /*
     * IMPORTANTE:
     *
     * Agora verificamos SOMENTE o estado atual.
     *
     * Não procuramos mais em todos os slots,
     * pois isso fazia moedas de um save
     * desaparecerem em outro.
     */

    public bool IsCoinCollected(
        string coinId
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                coinId
            )
        )
        {
            return false;
        }

        return currentCollectedCoins.Contains(
            coinId
        );
    }

    // =========================================================
    // SALVAR
    // =========================================================

    public void SaveToSlot(
        int slot,
        SaveData data
    )
    {
        if (!IsValidSlot(slot))
        {
            Debug.LogWarning(
                "SaveManager: slot inválido: "
                + slot
            );

            return;
        }

        SaveData save =
            data != null
                ? data.Clone()
                : new SaveData();

        if (
            string.IsNullOrEmpty(
                save.sceneName
            )
        )
        {
            save.sceneName =
                SceneManager
                    .GetActiveScene()
                    .name;
        }

        if (save.collectedCoinIds == null)
        {
            save.collectedCoinIds =
                new List<string>();
        }

        save.collectedCoinIds =
            NormalizeIds(
                save.collectedCoinIds
            );

        string json =
            JsonUtility.ToJson(
                save
            );

        string encrypted =
            EncryptString(
                json
            );

        File.WriteAllText(
            GetPath(slot),
            encrypted
        );

        // =====================================================
        // SAVE MANUAL TAMBÉM COPIA PARA AUTOSAVE
        // =====================================================

        if (slot != AutosaveSlot)
        {
            File.WriteAllText(
                GetPath(AutosaveSlot),
                encrypted
            );
        }

        Debug.Log(
            "SaveManager: slot "
            + slot
            + " salvo."
        );
    }

    // =========================================================
    // CARREGAR
    // =========================================================

    /*
     * Esse método SOMENTE lê o arquivo.
     *
     * Ele não altera o estado das moedas.
     * Isso evita que simplesmente abrir
     * o menu de slots modifique o jogo.
     */

    public SaveData LoadFromSlot(
        int slot
    )
    {
        if (!IsValidSlot(slot))
        {
            return null;
        }

        string path =
            GetPath(slot);

        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            string encrypted =
                File.ReadAllText(
                    path
                );

            string json =
                DecryptString(
                    encrypted
                );

            SaveData data =
                JsonUtility.FromJson<SaveData>(
                    json
                );

            if (data == null)
            {
                return null;
            }

            if (data.collectedCoinIds == null)
            {
                data.collectedCoinIds =
                    new List<string>();
            }

            return data;
        }
        catch (Exception ex)
        {
            Debug.LogWarning(
                "SaveManager: falha ao carregar slot "
                + slot
                + ". "
                + ex.Message
            );

            return null;
        }
    }

    // =========================================================
    // COPIAR SLOT PARA AUTOSAVE
    // =========================================================

    public void CopySlotToAutosave(
        int slot
    )
    {
        if (
            slot == AutosaveSlot ||
            !SlotExists(slot)
        )
        {
            return;
        }

        File.Copy(
            GetPath(slot),
            GetPath(AutosaveSlot),
            true
        );

        Debug.Log(
            "SaveManager: slot "
            + slot
            + " copiado para o autosave."
        );
    }

    // =========================================================
    // NORMALIZAR IDS
    // =========================================================

    private static List<string>
        NormalizeIds(
            IEnumerable<string> ids
        )
    {
        List<string> normalized =
            new List<string>();

        if (ids == null)
        {
            return normalized;
        }

        foreach (
            string id
            in ids
        )
        {
            if (
                string.IsNullOrWhiteSpace(
                    id
                )
            )
            {
                continue;
            }

            if (
                !normalized.Contains(
                    id
                )
            )
            {
                normalized.Add(
                    id
                );
            }
        }

        return normalized;
    }

    // =========================================================
    // ENCRIPTAR
    // =========================================================

    private static string EncryptString(
        string plainText
    )
    {
        using Aes aes =
            Aes.Create();

        aes.Key =
            Encoding.UTF8.GetBytes(
                KeyString
            );

        aes.IV =
            Encoding.UTF8.GetBytes(
                IvString
            );

        aes.Mode =
            CipherMode.CBC;

        aes.Padding =
            PaddingMode.PKCS7;

        using MemoryStream ms =
            new MemoryStream();

        using (
            CryptoStream cs =
                new CryptoStream(
                    ms,
                    aes.CreateEncryptor(),
                    CryptoStreamMode.Write
                )
        )
        {
            byte[] bytes =
                Encoding.UTF8.GetBytes(
                    plainText
                );

            cs.Write(
                bytes,
                0,
                bytes.Length
            );

            cs.FlushFinalBlock();
        }

        return Convert.ToBase64String(
            ms.ToArray()
        );
    }

    // =========================================================
    // DECRIPTAR
    // =========================================================

    private static string DecryptString(
        string encryptedText
    )
    {
        byte[] bytes =
            Convert.FromBase64String(
                encryptedText
            );

        using Aes aes =
            Aes.Create();

        aes.Key =
            Encoding.UTF8.GetBytes(
                KeyString
            );

        aes.IV =
            Encoding.UTF8.GetBytes(
                IvString
            );

        aes.Mode =
            CipherMode.CBC;

        aes.Padding =
            PaddingMode.PKCS7;

        using MemoryStream ms =
            new MemoryStream(
                bytes
            );

        using CryptoStream cs =
            new CryptoStream(
                ms,
                aes.CreateDecryptor(),
                CryptoStreamMode.Read
            );

        using StreamReader sr =
            new StreamReader(
                cs,
                Encoding.UTF8
            );

        return sr.ReadToEnd();
    }
}