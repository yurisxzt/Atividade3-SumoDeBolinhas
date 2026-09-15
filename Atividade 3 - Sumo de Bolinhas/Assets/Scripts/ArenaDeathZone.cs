using UnityEngine;

public class ArenaDeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        TryRespawn(other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryRespawn(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryRespawn(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryRespawn(collision.gameObject);
    }

    private void TryRespawn(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        TwoBallController player = obj.GetComponent<TwoBallController>();
        if (player == null)
        {
            player = obj.GetComponentInParent<TwoBallController>();
        }

        if (player == null)
        {
            return;
        }

        player.RespawnToCheckpointOrStart();
    }
}