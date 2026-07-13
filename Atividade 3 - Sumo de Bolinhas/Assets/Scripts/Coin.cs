using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField]
    private int value = 1;

    private void OnTriggerEnter(Collider other)
    {
        PlayerStats stats =
            other.GetComponent<PlayerStats>();

        if(stats == null)
            return;

        stats.AddCoins(value);

        Destroy(gameObject);
    }

    private void Update()
    {
        transform.Rotate(
            Vector3.up,
            180f * Time.deltaTime);
    }
}