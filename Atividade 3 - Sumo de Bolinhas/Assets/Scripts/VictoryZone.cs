using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryZone : MonoBehaviour
{
    // supports both 2D and 3D triggers
    void OnTriggerEnter2D(Collider2D other)
    {
        HandleTrigger(other.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        HandleTrigger(other.gameObject);
    }

    private void HandleTrigger(GameObject obj)
    {
        if (!obj.CompareTag("Player"))
            return;

        // ask player to save, then move to victory scene
        var player = obj.GetComponent<TwoBallController>();
        if (player != null)
            player.SaveCurrentProgress();

        // notify level manager if present
        var lm = FindObjectOfType<LevelManager>();
        if (lm != null)
            lm.OnVictory();

        // transition to final victory scene
        GameManager.Instance.ForceSceneChange("VictoryFinal");
    }
}
