using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public Text coinText;
    public int Coins { get; private set; }

    void Start() { UpdateUI(); }
    public void AddCoin() { Coins++; UpdateUI(); }
    public void SetCoins(int v) { Coins = v; UpdateUI(); }
    void UpdateUI() { if (coinText) coinText.text = Coins.ToString(); }
}
