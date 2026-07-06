using TMPro;
using UnityEngine;

public class VictoryManager : MonoBehaviour
{
    [SerializeField]
    TMP_Text winnerText;

    [SerializeField]
    TMP_Text ballText;

    private void Start()
    {
        winnerText.text =
            MatchData.WinnerName +
            " venceu!";

        ballText.text =
            "Bolinha: " +
            MatchData.WinnerBall;
    }

    public void ReturnToSelection()
    {
        GameManager.Instance
            .ForceSceneChange(
                "CharacterSelection");
    }
}