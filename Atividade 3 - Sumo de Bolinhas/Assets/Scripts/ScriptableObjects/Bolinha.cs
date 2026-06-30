using UnityEngine;

[System.Serializable]
public class Bolinha
{
    // Tamanho da bolinha
    public static float tamanho;
    // Velocidade inicial da bolinha
    public float velocidadeInicial;
    // Força do empurrão da bolinha
    public static float forcaEmpurrar;
    // Cores da bolinha (2 cores para cada tipo de bolinha, os jogadores nunca devem ter bolinhas da mesma cor)
    public Color cor1;
    public Color cor2;
}
