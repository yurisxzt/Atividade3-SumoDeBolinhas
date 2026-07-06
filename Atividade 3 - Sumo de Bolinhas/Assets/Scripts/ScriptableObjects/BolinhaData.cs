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

    public float basePushForce = 800f;

    public float maxPushForce = 1800f;

    public float initialSize = 1f;

    public float initialMass = 1f;
}