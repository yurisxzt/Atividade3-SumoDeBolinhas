using UnityEngine;
using UnityEngine.SceneManagement;

public class Coin : MonoBehaviour
{
    [SerializeField]
    private int value = 1;

    [SerializeField]
    private string coinId = "";

    public string CoinId =>
        coinId;

    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (
            string.IsNullOrWhiteSpace(
                coinId
            )
        )
        {
            coinId =
                SceneManager
                    .GetActiveScene()
                    .name
                + "_"
                + gameObject.name
                + "_"
                + Mathf.RoundToInt(
                    transform.position.x
                    * 100f
                )
                + "_"
                + Mathf.RoundToInt(
                    transform.position.y
                    * 100f
                )
                + "_"
                + Mathf.RoundToInt(
                    transform.position.z
                    * 100f
                );
        }
    }

    // =========================================================
    // START / ENABLE
    // =========================================================

    private void Start()
    {
        RefreshFromCurrentSaveState();
    }

    private void OnEnable()
    {
        RefreshFromCurrentSaveState();
    }

    // =========================================================
    // ATUALIZAR ESTADO
    // =========================================================

    public void RefreshFromCurrentSaveState()
    {
        if (SaveManager.Instance == null)
        {
            return;
        }

        bool collected =
            SaveManager.Instance
                .IsCoinCollected(
                    coinId
                );

        if (
            gameObject.activeSelf ==
            collected
        )
        {
            gameObject.SetActive(
                !collected
            );
        }
    }

    // =========================================================
    // PEGAR MOEDA
    // =========================================================

    private void OnTriggerEnter(
        Collider other
    )
    {
        PlayerStats stats =
            other.GetComponentInParent<PlayerStats>();

        if (stats == null)
        {
            return;
        }

        if (
            !other.CompareTag("Player") &&
            !stats.CompareTag("Player")
        )
        {
            return;
        }

        stats.AddCoins(
            value
        );

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance
                .MarkCoinCollected(
                    coinId
                );
        }

        /*
         * NÃO fazemos SaveCurrentProgress aqui.
         *
         * O progresso é salvo no checkpoint,
         * nos slots manuais e na vitória.
         */

        gameObject.SetActive(
            false
        );
    }

    // =========================================================
    // ROTAÇÃO
    // =========================================================

    private void Update()
    {
        transform.Rotate(
            Vector3.up,
            180f * Time.deltaTime
        );
    }
}