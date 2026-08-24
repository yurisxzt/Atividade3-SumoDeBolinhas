using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Available Balls")]
    [SerializeField]
    private BolinhaData[] availableBalls;

    [Header("Player 1")]
    [SerializeField]
    private Image p1Icon;

    [SerializeField]
    private TMP_Text p1Name;

    [SerializeField]
    private TMP_Text p1Stats;

    [SerializeField]
    private Image p1ColorPreview;

    [Header("Player 2")]
    [SerializeField]
    private Image p2Icon;

    [SerializeField]
    private TMP_Text p2Name;

    [SerializeField]
    private TMP_Text p2Stats;

    [SerializeField]
    private Image p2ColorPreview;

    private int p1Index;
    private int p2Index;

    private void Start()
    {
        UpdateUI();
    }

    public void NextP1()
    {
        p1Index++;

        if (p1Index >= availableBalls.Length)
            p1Index = 0;

        UpdateUI();
    }

    public void PreviousP1()
    {
        p1Index--;

        if (p1Index < 0)
            p1Index = availableBalls.Length - 1;

        UpdateUI();
    }

    public void NextP2()
    {
        p2Index++;

        if (p2Index >= availableBalls.Length)
            p2Index = 0;

        UpdateUI();
    }

    public void PreviousP2()
    {
        p2Index--;

        if (p2Index < 0)
            p2Index = availableBalls.Length - 1;

        UpdateUI();
    }

    private void UpdateUI()
    {
        UpdatePlayerDisplay(
            availableBalls[p1Index],
            p1Icon,
            p1Name,
            p1Stats,
            p1ColorPreview,
            true
        );

        UpdatePlayerDisplay(
            availableBalls[p2Index],
            p2Icon,
            p2Name,
            p2Stats,
            p2ColorPreview,
            false
        );
    }

    private void UpdatePlayerDisplay(
        BolinhaData data,
        Image icon,
        TMP_Text nameText,
        TMP_Text statsText,
        Image colorPreview,
        bool isPlayer1)
    {
        if (data == null)
            return;

        if (icon != null)
        {
            icon.sprite = data.icon;

            // Já mostra o ícone com a cor que será usada no jogo
            if (isPlayer1 && data.player1Material != null)
            {
                icon.color = data.player1Material.color;
            }
            else if (!isPlayer1 && data.player2Material != null)
            {
                icon.color = data.player2Material.color;
            }
        }

        if (nameText != null)
        {
            nameText.text = data.ballName;
        }

        if (statsText != null)
        {
            statsText.text =
                $"Velocidade: {data.moveSpeed}\n" +
                $"Força: {data.basePushForce}\n" +
                $"Tamanho: {data.initialSize}";
        }

        // Quadradinho/círculo mostrando a cor
        if (colorPreview != null)
        {
            if (isPlayer1 && data.player1Material != null)
            {
                colorPreview.color =
                    data.player1Material.color;
            }
            else if (!isPlayer1 && data.player2Material != null)
            {
                colorPreview.color =
                    data.player2Material.color;
            }
        }
    }

    public void StartMatch()
    {
        MatchData.Player1Ball =
            availableBalls[p1Index];

        MatchData.Player2Ball =
            availableBalls[p2Index];

        GameManager.Instance
            .RequestSceneChange("SampleScene");
    }
}