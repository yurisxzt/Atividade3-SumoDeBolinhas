using UnityEngine;

/// <summary>
/// Simple UI controller for the main menu. Attach to a GameObject in the MenuPrincipal scene
/// and wire the OnStartClicked and OnQuitClicked methods to the respective UI buttons.
/// </summary>
public class MenuUiController : MonoBehaviour
{
    public void OnStartClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RequestSceneChange("SampleScene");
        }
        else
        {
            Debug.LogWarning("MenuUiController: GameManager not found. Loading SampleScene directly.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }
    }

    public void OnQuitClicked()
    {
        Debug.Log("MenuUiController: Quit requested.");
        Application.Quit();
    }
}

