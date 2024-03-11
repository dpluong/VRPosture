using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using Unity.XR.CoreUtils;

public class CharacterMovementHelper : MonoBehaviour
{
    private XROrigin XROrigin;
    private CharacterController CharacterController;
    private CharacterControllerDriver Driver;

   
    void Update()
    {
        UpdateCharacterController();
    }

    /// <summary>
    /// Updates the <see cref="CharacterController.height"/> and <see cref="CharacterController.center"/>
    /// based on the camera's position.
    /// </summary>
    protected virtual void UpdateCharacterController()
    {
        if (XROrigin == null || CharacterController == null)
            return;

        var height = Mathf.Clamp(XROrigin.CameraInOriginSpaceHeight, Driver.minHeight, Driver.maxHeight);

        Vector3 center = XROrigin.CameraInOriginSpacePos;
        center.y = height / 2f + CharacterController.skinWidth;

        CharacterController.height = height;
        CharacterController.center = center;
        Debug.Log(XROrigin.CameraInOriginSpaceHeight);
    }


}
