using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconPosition : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float distanceZ = 3.0f;
    [SerializeField] private float distanceX = 3.0f;
    [SerializeField] private float distanceY = 3.0f;

    public float speed = 1.0f;

    /*
    bool IsCentered()
    {
        if (Vector3.Angle(cameraTransform.forward, transform.position - cameraTransform.position) > 0.1f)
            return false;

        return true;
    }*/

    private void Update()
    {
        Vector3 targetPosition = FindTargetPosition() ;
        transform.position = targetPosition;
        FaceTowardCamera();
    }

    private Vector3 FindTargetPosition()
    {
        return cameraTransform.position + (cameraTransform.forward * distanceZ) + (cameraTransform.right * distanceX) + (cameraTransform.up * distanceY);
    }

    private void FaceTowardCamera()
    {
        Vector3 targetDirection = -cameraTransform.position + transform.position;

        float singleStep = speed * Time.deltaTime;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        transform.rotation = Quaternion.LookRotation(newDirection);
    }
}
