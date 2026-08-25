using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseRoot;
    public GameObject saveSlotsRoot;
    bool isSavingMode = false;

    public void TogglePause()
    {
        bool active = !pauseRoot.activeSelf;
        pauseRoot.SetActive(active);
        Time.timeScale = active ? 0f : 1f;
    }

    public void OnSaveGameClicked()
    {
        isSavingMode = true;
        saveSlotsRoot.SetActive(true);
    }
    public void OnLoadGameClicked()
    {
        isSavingMode = false;
        saveSlotsRoot.SetActive(true);
    }
    public void OnReturnToMenu() { Time.timeScale = 1f; UnityEngine.SceneManagement.SceneManager.LoadScene("MenuInitial"); }
}
