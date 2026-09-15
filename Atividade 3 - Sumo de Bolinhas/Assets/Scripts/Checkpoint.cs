using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint")]
    public Vector3 centerOffset =
        Vector3.zero;

    public bool activated =
        false;

    public string checkpointId =
        "checkpoint";

    // =========================================================
    // TRIGGER 2D
    // =========================================================

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        HandleTrigger(
            other.gameObject
        );
    }

    // =========================================================
    // TRIGGER 3D
    // =========================================================

    private void OnTriggerEnter(
        Collider other
    )
    {
        HandleTrigger(
            other.gameObject
        );
    }

    // =========================================================
    // ATIVAR
    // =========================================================

    private void HandleTrigger(
        GameObject obj
    )
    {
        if (
            activated ||
            obj == null
        )
        {
            return;
        }

        TwoBallController player =
            obj.GetComponentInParent<TwoBallController>();

        if (player == null)
        {
            return;
        }

        if (
            !player.CompareTag(
                "Player"
            )
        )
        {
            return;
        }

        activated =
            true;

        Vector3 centerPosition =
            transform.position
            + centerOffset;

        string id =
            string.IsNullOrEmpty(
                checkpointId
            )
                ? gameObject.name
                : checkpointId;

        // =====================================================
        // AUTOSAVE DO CHECKPOINT
        // =====================================================

        player.SaveCheckpointProgress(
            id,
            centerPosition
        );

        // =====================================================
        // STATIC EVENT CHANNEL
        // =====================================================

        VoidEventChannel evt =
            Resources.Load<VoidEventChannel>(
                "EventChannels/CheckpointReached"
            );

        if (evt != null)
        {
            evt.Raise();
        }

        Debug.Log(
            "Checkpoint ativado: "
            + id
        );
    }
}