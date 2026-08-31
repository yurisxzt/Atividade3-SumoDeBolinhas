using UnityEngine;

/// <summary>
/// Controlador simples para o Menu Principal.
/// Configure os nomes das cenas no Inspector e ligue os botões às funções abaixo.
/// </summary>
public class MenuPrincipalController : MonoBehaviour
{
    [Header("Cenas")]
    public string selecaoBolinhasScene = "SeleçãoBolinhas";
    public string jogoComSaveScene = "SampleScene"; // cena que usa SaveManager

    /// <summary>
    /// Botão: Seleção de Bolinhas
    /// </summary>
    public void OnSelecaoBolinhasClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RequestSceneChange(selecaoBolinhasScene);
    }

    /// <summary>
    /// Botão: Iniciar Jogo (usando sistema de save)
    /// </summary>
    public void OnJogoComSaveClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ForceSceneChange(jogoComSaveScene);
    }

    /// <summary>
    /// Botão: Sair do jogo
    /// </summary>
    public void OnSairClicked()
    {
        Debug.Log("MenuPrincipal: Sair solicitado.");
        Application.Quit();
    }
}