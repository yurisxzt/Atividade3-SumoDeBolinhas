using UnityEngine;

namespace ScriptableObjects
{
    public class CoinSpawner
    {
        // No cenário devem ter moedas, que quando coletadas, aumentam a pontuação do jogador e também aumentam a força de empurrão da bolinha. A cada 5 moedas coletadas, a bolinha do jogador aumenta de tamanho, ficando mais lenta, mas mais resistente a ser empurrada para longe.
        public int moedasColetadas;
        public GameObject coinPrefab;
        // Spawn de moedas: a cada 5 segundos, uma moeda é spawnada em uma posição aleatória dentro de um raio de 10 unidades do jogador. A moeda deve ter um collider e um script que detecta quando o jogador a coleta, aumentando a pontuação e a força de empurrão da bolinha.
        public void SpawnCoin(Vector3 position)
        {
            if (coinPrefab != null)
            {
                GameObject coin = GameObject.Instantiate(coinPrefab, position, Quaternion.identity);
                // Adiciona um collider e um script de coleta à moeda
                var collider = coin.AddComponent<SphereCollider>();
                collider.isTrigger = true;
                coin.AddComponent<Coin>();
            }
            else
            {
                Debug.LogWarning("CoinSpawner: coinPrefab is not assigned.");
            }
        }
        
    }
}