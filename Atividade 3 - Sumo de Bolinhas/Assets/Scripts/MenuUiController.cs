using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla o menu principal do jogo com início, carregar, continuar e sair.
/// </summary>
public class MenuUiController : MonoBehaviour
{
    [Header("Scenes")]
    public string firstLevelScene = "Fase1";
    public string saveSlotScene = "Menu Inicial";

    [Header("UI")]
    public GameObject continueButton;

    private void Start()
    {
        UpdateContinueButton();
    }

    private void OnEnable()
    {
        UpdateContinueButton();
    }

    public void UpdateContinueButton()
    {
        if (continueButton == null)
        {
            return;
        }

        bool hasAuto = SaveManager.Instance != null && SaveManager.Instance.SlotExists(0);
        continueButton.SetActive(hasAuto);
    }

    public void OnNewGameClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ForceSceneChange(firstLevelScene);
            return;
        }

        SceneManager.LoadScene(firstLevelScene);
    }

    public void OnContinueClicked()
    {
        if (SaveRestoreManager.Instance != null)
        {
            SaveRestoreManager.Instance.RequestLoadSlot(0);
            return;
        }

        if (SaveManager.Instance != null && SaveManager.Instance.SlotExists(0))
        {
            SaveData data = SaveManager.Instance.LoadFromSlot(0);
            if (data != null)
            {
                if (!string.IsNullOrEmpty(data.sceneName))
                {
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.ForceSceneChange(data.sceneName);
                    }
                    else
                    {
                        SceneManager.LoadScene(data.sceneName);
                    }
                }
            }
        }
    }

    public void OnLoadGameClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ForceSceneChange(saveSlotScene);
            return;
        }

        SceneManager.LoadScene(saveSlotScene);
    }

    public void OnQuitClicked()
    {
        Debug.Log("MenuUiController: Quit requested.");
        Application.Quit();
    }
}

