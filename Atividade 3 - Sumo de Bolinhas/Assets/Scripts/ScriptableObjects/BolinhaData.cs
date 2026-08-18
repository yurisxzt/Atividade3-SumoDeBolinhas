using UnityEngine;

[CreateAssetMenu(
    fileName = "BolinhaData",
    menuName = "Sumo de Bolinhas/Bolinha Data")]
public class BolinhaData : ScriptableObject
{
    [Header("Visual")]
    public string ballName;

    public Sprite icon;

    public Material player1Material;
    public Material player2Material;

    [Header("Stats")]
    public float moveSpeed = 12f;

    public float basePushForce = 1f;

    public float maxPushForce = 1f;

    public float initialSize = 1f;

    public float initialMass = 1f;
}