using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Vehicle")]
    public Transform target;

    [Header("Follow Settings")]
    public float followSpeed = 5f;
    public float rotationSpeed = 5f;

    void FixedUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 targetPosition = target.position;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.fixedDeltaTime);

        Quaternion targetRotation = target.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }
}