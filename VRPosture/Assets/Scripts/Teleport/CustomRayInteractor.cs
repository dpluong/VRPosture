using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomRayInteractor : XRRayInteractor
{
    public Transform playerTransform; // Reference to your XR Rig's transform

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (args.interactableObject is BaseTeleportationInteractable teleportInteractable)
        {
            // Get the hit point with the same y-value as the player's current position
            Vector3 teleportPosition = new Vector3(args.interactableObject.transform.position.x, playerTransform.position.y, args.interactableObject.transform.position.z);
            // Teleport the player to the adjusted position
            playerTransform.position = teleportPosition;
        }
    }
}