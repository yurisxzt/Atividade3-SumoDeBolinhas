using UnityEngine;

[RequireComponent(typeof(TwoBallController))]
public class BolinhaController : MonoBehaviour
{
    [SerializeField]
    private BolinhaData bolinhaData;

    [SerializeField]
    private bool isPlayer1;

    private TwoBallController controller;
    private Rigidbody rb;

    public BolinhaData Data => bolinhaData;

    private void Awake()
    {
        controller = GetComponent<TwoBallController>();
        rb = GetComponent<Rigidbody>();

        ApplyData();
    }

    public void SetData(BolinhaData data)
    {
        bolinhaData = data;

        ApplyData();
    }

    void ApplyData()
    {
        if (bolinhaData == null)
            return;

        transform.localScale =
            Vector3.one * bolinhaData.initialSize;

        if (rb != null)
        {
            rb.mass = bolinhaData.initialMass;
        }

        MeshRenderer renderer =
            GetComponent<MeshRenderer>();

        if (renderer != null)
        {
            renderer.material =
                isPlayer1
                ? bolinhaData.player1Material
                : bolinhaData.player2Material;
        }

        controller.Configure(
            bolinhaData.moveSpeed,
            bolinhaData.basePushForce,
            bolinhaData.maxPushForce);
    }
}