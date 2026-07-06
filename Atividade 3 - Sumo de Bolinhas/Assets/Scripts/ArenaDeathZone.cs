using UnityEngine;

public class ArenaDeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        TwoBallController player =
            other.GetComponent<TwoBallController>();

        if(player == null)
            return;

        GameManager.Instance.PlayerLost(player);
    }
}