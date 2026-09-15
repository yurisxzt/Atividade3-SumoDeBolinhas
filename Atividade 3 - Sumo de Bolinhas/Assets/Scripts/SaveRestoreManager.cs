using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveRestoreManager : MonoBehaviour
{
    public static SaveRestoreManager Instance
    {
        get;
        private set;
    }

    private SaveData pendingSave;

    private int pendingSlot = -1;

    private bool waitingForScene = false;

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
    // CARREGAR SLOT
    // =========================================================

    public void RequestLoadSlot(
        int slot
    )
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogWarning(
                "SaveRestoreManager: SaveManager não encontrado."
            );

            return;
        }

        SaveData data =
            SaveManager.Instance
                .LoadFromSlot(
                    slot
                );

        if (data == null)
        {
            Debug.LogWarning(
                "SaveRestoreManager: não existe save no slot "
                + slot
            );

            return;
        }

        if (
            string.IsNullOrEmpty(
                data.sceneName
            )
        )
        {
            Debug.LogWarning(
                "SaveRestoreManager: save não possui cena."
            );

            return;
        }

        pendingSave =
            data.Clone();

        pendingSlot =
            slot;

        // Deixa o estado das moedas pronto
        // ANTES da nova cena nascer.
        SaveManager.Instance
            .SetCurrentCollectedCoins(
                pendingSave.collectedCoinIds
            );

        // Caso tenha sido chamado pelo menu de pause
        Time.timeScale = 1f;

        ForceReloadSavedScene();
    }

    // =========================================================
    // RECARREGAR CENA
    // =========================================================

    private void ForceReloadSavedScene()
    {
        if (
            pendingSave == null ||
            waitingForScene
        )
        {
            return;
        }

        waitingForScene = true;

        SceneManager.sceneLoaded +=
            OnSavedSceneLoaded;

        /*
         * Mesmo se já estivermos nessa fase,
         * ForceSceneChange fará o carregamento novamente.
         */

        if (GameManager.Instance != null)
        {
            GameManager.Instance
                .ForceSceneChange(
                    pendingSave.sceneName
                );
        }
        else
        {
            SceneManager.LoadScene(
                pendingSave.sceneName,
                LoadSceneMode.Single
            );
        }
    }

    // =========================================================
    // CENA CARREGADA
    // =========================================================

    private void OnSavedSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        if (pendingSave == null)
        {
            return;
        }

        if (
            scene.name !=
            pendingSave.sceneName
        )
        {
            return;
        }

        SceneManager.sceneLoaded -=
            OnSavedSceneLoaded;

        waitingForScene = false;

        StartCoroutine(
            ApplySaveNextFrame()
        );
    }

    // =========================================================
    // APLICAR NO PRÓXIMO FRAME
    // =========================================================

    private IEnumerator ApplySaveNextFrame()
    {
        // Dá tempo para Player/HUD/etc. executarem Awake/Start
        yield return null;

        SaveData data =
            pendingSave;

        int loadedSlot =
            pendingSlot;

        ApplySaveToCurrentScene(
            data
        );

        // Carregar um slot manual
        // também atualiza o slot 0.
        if (
            SaveManager.Instance != null &&
            loadedSlot !=
            SaveManager.AutosaveSlot
        )
        {
            SaveManager.Instance
                .CopySlotToAutosave(
                    loadedSlot
                );
        }

        pendingSave = null;

        pendingSlot = -1;
    }

    // =========================================================
    // APLICAR SAVE
    // =========================================================

    private void ApplySaveToCurrentScene(
        SaveData data
    )
    {
        if (data == null)
        {
            return;
        }

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance
                .SetCurrentCollectedCoins(
                    data.collectedCoinIds
                );
        }

        TwoBallController player =
            FindFirstObjectByType<TwoBallController>();

        if (player != null)
        {
            player.ApplyLoadedData(
                data
            );
        }

        HUDController hud =
            FindFirstObjectByType<HUDController>();

        if (hud != null)
        {
            hud.SetCoins(
                data.coins
            );
        }

        RefreshCoins();

        Debug.Log(
            "SaveRestoreManager: save aplicado."
        );
    }

    // =========================================================
    // ATUALIZAR MOEDAS
    // =========================================================

    private void RefreshCoins()
    {
        Coin[] coins =
            FindObjectsByType<Coin>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (
            Coin coin
            in coins
        )
        {
            if (coin != null)
            {
                coin.RefreshFromCurrentSaveState();
            }
        }
    }
}