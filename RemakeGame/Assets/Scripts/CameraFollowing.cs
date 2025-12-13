using UnityEngine;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
    public float moveSmoothness;
    public float rotSmoothness;
    public Vector3 moveOffset;
    public Vector3 rotOffset;
    public Transform carTarget;
    public Camera cam;
    public float pullBackDistance = 4f;
    public float extraFOV = 15f;        
    private float defaultFOV;

    void Start()
    {
     
        if (cam != null) defaultFOV = cam.fieldOfView;
    }

    void FixedUpdate()
    {
        FollowTarget();
    }

    void FollowTarget()
    {
        HandleMovement();
        HandleRotation();
        HandleFOV();
    }

    void HandleMovement()
    {
        float input = Mathf.Clamp01(Input.GetAxisRaw("Vertical"));

        Vector3 tempOffset = moveOffset;

        tempOffset.z -= input * pullBackDistance;

        Vector3 targetPos = carTarget.TransformPoint(tempOffset);

        float currentSmooth = moveSmoothness;
        if (input > 0.1f)
        {
            currentSmooth = moveSmoothness * 0.4f;
        }
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSmoothness * Time.deltaTime);
    }

    void HandleRotation()
    {
        var direction = carTarget.position - transform.position;

        var rotation = new Quaternion();

        if (direction != Vector3.zero)
        {
            rotation = Quaternion.LookRotation(direction + rotOffset, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotSmoothness * Time.deltaTime);
        }
    }

    void HandleFOV()
    {
        if (cam == null) return;

        float input = Mathf.Clamp01(Input.GetAxis("Vertical"));

        float targetFOV = defaultFOV + (input * extraFOV);

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * 5f);
    }
}