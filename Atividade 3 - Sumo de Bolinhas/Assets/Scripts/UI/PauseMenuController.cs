using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseRoot;
    public GameObject saveSlotsRoot;
    bool isSavingMode = false;

    private void Update()
    {
#if ENABLE_INPUT_SYSTEM
        // Use new Input System when available
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
#else
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
#endif
    }

    public void TogglePause()
    {
        if (pauseRoot == null) return;
        bool active = !pauseRoot.activeSelf;
        pauseRoot.SetActive(active);
        Time.timeScale = active ? 0f : 1f;
    }

    public void OnSaveGameClicked()
    {
        isSavingMode = true;
        if (saveSlotsRoot != null) saveSlotsRoot.SetActive(true);
    }

    public void OnLoadGameClicked()
    {
        isSavingMode = false;
        if (saveSlotsRoot != null) saveSlotsRoot.SetActive(true);
    }

    /// <summary>
    /// Fecha/Esconde o painel de seleção de saves — ligar a um botão "Cancelar" ou "Fechar".
    /// </summary>
    public void CloseSaveSlots()
    {
        isSavingMode = false;
        if (saveSlotsRoot != null) saveSlotsRoot.SetActive(false);
    }

    public void SaveAndResume()
    {
        var player = FindObjectOfType<TwoBallController>();
        if (player != null)
            player.SaveCurrentProgress();

        TogglePause();
    }

    public void OnReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu Inicial");
    }
}
