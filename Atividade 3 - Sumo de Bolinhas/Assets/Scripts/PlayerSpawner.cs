using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject player1Prefab;

    [SerializeField]
    private GameObject player2Prefab;

    public PlayerStats Player1Stats { get; private set; }
    public PlayerStats Player2Stats { get; private set; }

    public TwoBallController Player1Controller { get; private set; }
    public TwoBallController Player2Controller { get; private set; }

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

        // Cria o placar de moedas
        if (GetComponent<CoinScoreboard>() == null)
        {
            gameObject.AddComponent<CoinScoreboard>();
        }

        // Cria a interface das barras de empurrão
        if (GetComponent<PushCooldownUI>() == null)
        {
            gameObject.AddComponent<PushCooldownUI>();
        }
    }

    private void SpawnPlayer(
        GameObject prefab,
        BolinhaData data,
        bool p1,
        Vector3 pos)
    {
        if (prefab == null)
        {
            Debug.LogError(
                "Prefab do Jogador " +
                (p1 ? "1" : "2") +
                " não foi configurado!"
            );

            return;
        }

        // Cria a bolinha
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
                "O prefab " +
                prefab.name +
                " não possui BolinhaController!"
            );
        }

        // Pega o controlador de movimento/empurrão
        TwoBallController controller =
            obj.GetComponent<TwoBallController>();

        if (controller == null)
        {
            Debug.LogError(
                "O prefab " +
                prefab.name +
                " não possui TwoBallController!"
            );

            controller =
                obj.AddComponent<TwoBallController>();
        }

        // Pega os PlayerStats
        PlayerStats playerStats =
            obj.GetComponent<PlayerStats>();

        // Se não tiver, adiciona automaticamente
        if (playerStats == null)
        {
            playerStats =
                obj.AddComponent<PlayerStats>();

            Debug.Log(
                "PlayerStats adicionado automaticamente ao " +
                (p1 ? "Jogador 1" : "Jogador 2")
            );
        }

        // Guarda as referências
        if (p1)
        {
            Player1Stats = playerStats;
            Player1Controller = controller;
        }
        else
        {
            Player2Stats = playerStats;
            Player2Controller = controller;
        }

        // Nomeia a bolinha
        obj.name = p1
            ? "Jogador 1"
            : "Jogador 2";

        Debug.Log(
            obj.name +
            " criado com sucesso."
        );
    }
}