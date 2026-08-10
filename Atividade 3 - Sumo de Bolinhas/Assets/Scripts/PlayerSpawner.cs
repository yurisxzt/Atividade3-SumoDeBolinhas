using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject player1Prefab;

    [SerializeField]
    private GameObject player2Prefab;

    public PlayerStats Player1Stats { get; private set; }

    public PlayerStats Player2Stats { get; private set; }

    private void Start()
    {
        SpawnPlayer(
            player1Prefab,
            MatchData.Player1Ball,
            true,
            new Vector3(-3, 1, 0)
        );

        SpawnPlayer(
            player2Prefab,
            MatchData.Player2Ball,
            false,
            new Vector3(3, 1, 0)
        );

        gameObject.AddComponent<CoinScoreboard>();
    }

    private void SpawnPlayer(
        GameObject prefab,
        BolinhaData data,
        bool p1,
        Vector3 pos)
    {
        GameObject obj = Instantiate(
            prefab,
            pos,
            Quaternion.identity
        );

        // Pega o controlador da bolinha
        BolinhaController ball =
            obj.GetComponent<BolinhaController>();

        if (ball != null)
        {
            ball.SetData(data);
        }
        else
        {
            Debug.LogError(
                "O prefab " + prefab.name +
                " não possui BolinhaController!"
            );
        }

        // Procura o PlayerStats
        PlayerStats playerStats =
            obj.GetComponent<PlayerStats>();

        // Se não existir, adiciona automaticamente
        if (playerStats == null)
        {
            Debug.LogWarning(
                "PlayerStats não encontrado em " +
                obj.name +
                ". Adicionando automaticamente."
            );

            playerStats =
                obj.AddComponent<PlayerStats>();
        }

        // Guarda os stats do jogador correto
        if (p1)
        {
            Player1Stats = playerStats;
        }
        else
        {
            Player2Stats = playerStats;
        }

        obj.name = p1
            ? "Jogador 1"
            : "Jogador 2";

        Debug.Log(
            obj.name +
            " criado com PlayerStats corretamente."
        );
    }
}