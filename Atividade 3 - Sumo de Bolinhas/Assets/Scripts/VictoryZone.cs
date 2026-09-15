using UnityEngine;

public class VictoryZone : MonoBehaviour
{
    public string nextLevelSceneName = "Fase2";

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleTrigger(other.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleTrigger(other.gameObject);
    }

    private void HandleTrigger(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        var player = obj.GetComponent<TwoBallController>();
        if (player == null)
        {
            return;
        }

        player.SaveCurrentProgress();

        var lm = FindFirstObjectByType<LevelManager>();
        if (lm != null)
        {
            lm.OnVictory();
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ForceSceneChange(nextLevelSceneName);
        }
    }
}
