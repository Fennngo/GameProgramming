using UnityEngine;

public class CarEffectController : MonoBehaviour
{
    public Rigidbody carRigidbody;
    public Transform cameraTransform;
    public Material speedLineMaterial;

    public float shakeAmount = 0.1f;

    private float currentSpeedValue = 0f;
    private Vector3 originalCamPos;

    void Start()
    {
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
        originalCamPos = cameraTransform.localPosition;
    }

    private void LateUpdate()
    {
        if (carRigidbody == null || speedLineMaterial == null) return;

        float carVelocity = carRigidbody.linearVelocity.magnitude;

        float targetSpeedValue = 0f;
        bool shouldBeOpen = false;

        if (carVelocity > 50f)
        {
            shouldBeOpen = true;
            targetSpeedValue = 1f;
        }
        else if (carVelocity > 30f)
        {
            shouldBeOpen = true;
            targetSpeedValue = 0.5f;
        }
        else
        {
            shouldBeOpen = false;
            targetSpeedValue = 0f;
        }
        speedLineMaterial.SetInt("_IsOpen", shouldBeOpen ? 1 : 0);

        currentSpeedValue = Mathf.Lerp(currentSpeedValue, targetSpeedValue, Time.deltaTime * 5f);
        speedLineMaterial.SetFloat("_Speed", currentSpeedValue);

        if (carVelocity > 30f)
        {
            Vector3 randomShake = Random.insideUnitSphere * shakeAmount;
            if (carVelocity > 50f) randomShake *= 1.5f;
            cameraTransform.localPosition = originalCamPos + randomShake;
        }
        else
        {
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, originalCamPos, Time.deltaTime * 10f);
        }
    }
    void OnDisable()
    {
        if (speedLineMaterial != null) speedLineMaterial.SetInt("_IsOpen", 0);
    }
}