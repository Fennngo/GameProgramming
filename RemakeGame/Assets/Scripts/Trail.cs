using UnityEngine;

[DefaultExecutionOrder(100)]   
public class TrailGroundAligner : MonoBehaviour
{
    public Transform car;          
    public LayerMask ground;        
    public float rayDistance = 1.0f;
    public float yOffset = 0.005f;  

    void LateUpdate()
    {
        Vector3 origin = transform.position + Vector3.up * 0.2f;
        if (Physics.Raycast(origin, Vector3.down, out var hit, rayDistance, ground))
        {
            transform.position = hit.point + hit.normal * yOffset;

            Vector3 normal = hit.normal;
            Vector3 upRef = car ? car.forward : Vector3.forward;
            transform.rotation = Quaternion.LookRotation(normal, upRef);
        }
    }
}
