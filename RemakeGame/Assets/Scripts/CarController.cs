using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController: MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 30f;
    public float turnSpeed = 300f;
    private Rigidbody rb;

    [Header("Exhaust Settings (Low Poly Style)")]
    public ParticleSystem exhaustLeft;
    public ParticleSystem exhaustRight;
    public float exhaustRateWhileMoving = 25f;
    public float exhaustRateIdle = 0f;

    [Header("Trail Settings (Low Poly Tire Marks)")]
    public Transform rearLeftTrackPoint;
    public Transform rearRightTrackPoint;
    public LayerMask groundLayer;
    public float trailYOffset = 0.005f;

    private TrailRenderer rearLeftTrail;
    private TrailRenderer rearRightTrail;

    private ParticleSystem.EmissionModule leftEmission;
    private ParticleSystem.EmissionModule rightEmission;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;

        if (exhaustLeft != null)
        {
            leftEmission = exhaustLeft.emission;
            leftEmission.rateOverTime = exhaustRateIdle;
        }
        if (exhaustRight != null)
        {
            rightEmission = exhaustRight.emission;
            rightEmission.rateOverTime = exhaustRateIdle;
        }

        InitializeTrailTracks();

        Debug.Log(" CarController_LowPoly Initialized!");
    }

    void InitializeTrailTracks()
    {
        if (rearLeftTrackPoint != null)
        {
            rearLeftTrail = rearLeftTrackPoint.GetComponent<TrailRenderer>();
            if (rearLeftTrail != null) rearLeftTrail.emitting = false;
        }

        if (rearRightTrackPoint != null)
        {
            rearRightTrail = rearRightTrackPoint.GetComponent<TrailRenderer>();
            if (rearRightTrail != null) rearRightTrail.emitting = false;
        }
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        rb.AddForce(transform.forward * moveInput * moveSpeed, ForceMode.VelocityChange);

        float turn = turnInput * turnSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.up * turn));

        HandleExhaust(moveInput);
        HandleTrailTracks(moveInput);
    }

    private void HandleExhaust(float moveInput)
    {
        bool isMoving = Mathf.Abs(moveInput) > 0.1f;

        if (exhaustLeft != null)
        {
            leftEmission.rateOverTime = isMoving ? exhaustRateWhileMoving : exhaustRateIdle;
        }

        if (exhaustRight != null)
        {
            rightEmission.rateOverTime = isMoving ? exhaustRateWhileMoving : exhaustRateIdle;
        }
    }

    private void UpdateTrack(TrailRenderer tr, Transform trackPoint, bool isMoving)
    {
        if (tr == null || trackPoint == null) return;

        RaycastHit hit;
        bool onGround = Physics.Raycast(trackPoint.position + Vector3.up * 0.1f, Vector3.down, out hit, 0.3f, groundLayer);

        tr.emitting = isMoving && onGround;

        if (onGround)
        {
            Vector3 pos = tr.transform.position;
            pos.y = hit.point.y + trailYOffset;
            tr.transform.position = pos;
        }
    }

    private void HandleTrailTracks(float moveInput)
    {
        bool isMoving = Mathf.Abs(moveInput) > 0.1f;
        UpdateTrack(rearLeftTrail, rearLeftTrackPoint, isMoving);
        UpdateTrack(rearRightTrail, rearRightTrackPoint, isMoving);
    }
}
