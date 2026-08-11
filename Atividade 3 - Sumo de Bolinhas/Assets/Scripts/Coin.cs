using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField]
    private int value = 1;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(
            "Objeto entrou na moeda: " +
            other.name
        );

        PlayerStats stats =
            other.GetComponentInParent<PlayerStats>();

        if (stats == null)
        {
            Debug.Log(
                "Não encontrou PlayerStats em: " +
                other.name
            );

            return;
        }

        Debug.Log(
            "Moeda coletada por: " +
            other.name
        );

        stats.AddCoins(value);

        Destroy(gameObject);
    }

    private void Update()
    {
        transform.Rotate(
            Vector3.up,
            180f * Time.deltaTime
        );
    }
}