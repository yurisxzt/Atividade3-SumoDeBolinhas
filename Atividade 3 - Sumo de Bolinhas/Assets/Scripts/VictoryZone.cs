using UnityEngine;

public class VictoryZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var lm = FindObjectOfType<LevelManager>();
            if (lm != null) lm.OnVictory();
        }
    }
}
