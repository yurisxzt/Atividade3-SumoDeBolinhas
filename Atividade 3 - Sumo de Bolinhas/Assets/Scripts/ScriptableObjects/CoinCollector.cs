using System;
using UnityEngine;

namespace ScriptableObjects
{
    public class CoinCollector : MonoBehaviour
    {
        // No cenário devem ter moedas, que quando coletadas, aumentam a pontuação do jogador e também aumentam a força de empurrão da bolinha. A cada 5 moedas coletadas, a bolinha do jogador aumenta de tamanho, ficando mais lenta, mas mais resistente a ser empurrada para longe.
        public int moedasColetadas;

        private void Start()
        {
            moedasColetadas = 0;
        }

        public void CollectCoin(int value)
        {
            moedasColetadas += value;
            // Aqui você pode adicionar lógica adicional, como aumentar a força de empurrão da bolinha ou aumentar o tamanho da bolinha a cada 5 moedas coletadas.
            for (int i = 0; i < moedasColetadas / 5; i++)
            {
                // Aumenta o tamanho da bolinha e ajusta a força de empurrão
                Bolinha.tamanho += 0.1f;
                Bolinha.forcaEmpurrar += 0.5f;
            }
            
        }
    }
}