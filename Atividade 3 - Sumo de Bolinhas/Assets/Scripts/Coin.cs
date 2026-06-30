
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Coin : MonoBehaviour
{
	public int value = 1;
	
	public void OnTriggerEnter(Collider other)
	{
		// Quanto mais moedas você coleta, mais difícil de ser jogada longe sua bolinha fica, também mais força ela dá na bolinha inimiga quando usa o botão de ação, porem ela fica mais lenta.
		if (other.CompareTag("Player"))
		{
			Debug.Log($"Coin: '{gameObject.name}' collected by '{other.gameObject.name}' (tag={other.gameObject.tag}) value={value}");
			var pc = other.GetComponent<TwoBallController>();
			if (pc != null)
			{
				pc.CollectCoin(value);
			}
			else
			{
				Debug.LogWarning("PlayerCoinCollector component not found on player.");
			}
			
			Destroy(gameObject);
		}
	}
	
}


