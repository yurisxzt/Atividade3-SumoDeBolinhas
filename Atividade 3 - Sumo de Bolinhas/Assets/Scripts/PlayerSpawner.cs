using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject player1Prefab;

    [SerializeField]
    private GameObject player2Prefab;

    private void Start()
    {
        SpawnPlayer(
            player1Prefab,
            MatchData.Player1Ball,
            true,
            new Vector3(-3,0,0));

        SpawnPlayer(
            player2Prefab,
            MatchData.Player2Ball,
            false,
            new Vector3(3,0,0));
    }

    void SpawnPlayer(
        GameObject prefab,
        BolinhaData data,
        bool p1,
        Vector3 pos)
    {
        GameObject obj =
            Instantiate(
                prefab,
                pos,
                Quaternion.identity);

        BolinhaController ball =
            obj.GetComponent<BolinhaController>();

        ball.SetData(data);

        obj.name = p1
            ? "Jogador 1"
            : "Jogador 2";
    }
}