using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Speed / Accel")]
    public float maxSpeed = 22f;
    public float accel = 14f;
    public float brake = 20f;
    public float dragWhenNoInput = 1.2f;
    public float dragWhenBraking = 3.0f;

    [Header("Steering")]
    public float steerAtZero = 120f;
    public float steerAtMax = 40f;
    public float steerResponse = 8f;

    [Header("Grip / Skid")]
    public float lateralFriction = 8f;
    public float skidThreshold = 3.0f;
    public float downforce = 20f;

    [Header("Effects (optional)")]
    public ParticleSystem exhaustLeft, exhaustRight;
    public float exhaustRateIdle = 0f;
    public float exhaustRateMax = 40f;
    public TrailRenderer rearLeftTrail, rearRightTrail;

    Rigidbody rb;
    float throttle;
    float steer;
    float targetSteer;
    Vector3 localVel;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    void Update()
    {
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(h) < 0.08f) h = 0f;
        throttle = Mathf.MoveTowards(throttle, v, Time.deltaTime * 4f);
        targetSteer = h;
        steer = Mathf.Lerp(steer, targetSteer, Time.deltaTime * steerResponse);

        float speed01 = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);
        float steerDegPerSec = Mathf.Lerp(steerAtZero, steerAtMax, speed01);
        float turn = steer * steerDegPerSec * Time.deltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));

        bool handbrake = Input.GetKey(KeyCode.Space);
        float exhaustRate = Mathf.Lerp(exhaustRateIdle, exhaustRateMax, Mathf.Abs(throttle)) * (handbrake ? 0f : 1f);
        if (exhaustLeft) { var em = exhaustLeft.emission; em.rateOverTime = exhaustRate; }
        if (exhaustRight) { var em = exhaustRight.emission; em.rateOverTime = exhaustRate; }
    }

    void FixedUpdate()
    {
        localVel = transform.InverseTransformVector(rb.linearVelocity);

        float forwardForce = 0f;

        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
        if (forwardSpeed > 1f && throttle < 0f)
        {
            throttle = -1f;  
        }

        if (throttle > 0f) forwardForce = accel * throttle;
        else if (throttle < 0f) forwardForce = brake * throttle;

        float drag = (Mathf.Approximately(throttle, 0f)) ? dragWhenNoInput : (throttle < 0f ? dragWhenBraking : 0f);
        Vector3 vel = rb.linearVelocity * (1f / (1f + drag * Time.fixedDeltaTime));
        rb.linearVelocity = vel;

        rb.AddForce(transform.forward * forwardForce, ForceMode.VelocityChange);

        bool handbrake = Input.GetKey(KeyCode.Space);
        float grip = handbrake ? lateralFriction * 0.35f : lateralFriction; 
        Vector3 worldRight = transform.right;
        float lateralSpeed = Vector3.Dot(rb.linearVelocity, worldRight);
        Vector3 lateralCancel = -worldRight * lateralSpeed * grip;
        rb.AddForce(lateralCancel, ForceMode.Acceleration);

        rb.AddForce(-Vector3.up * downforce * rb.linearVelocity.magnitude, ForceMode.Acceleration);

        Vector3 fwd = transform.forward * Vector3.Dot(rb.linearVelocity, transform.forward);
        fwd = Vector3.ClampMagnitude(fwd, maxSpeed);
        Vector3 side = worldRight * Vector3.Dot(rb.linearVelocity, worldRight);
        rb.linearVelocity = fwd + side;
        bool noSteer = Mathf.Abs(targetSteer) < 0.05f;
        if (noSteer && !Input.GetKey(KeyCode.Space))
        {
            Vector3 right = transform.right;
            side = Vector3.zero;
            rb.linearVelocity = fwd + side;
            var ang = rb.angularVelocity; ang.y = 0f; rb.angularVelocity = ang;
        }

        bool skidding = Mathf.Abs(lateralSpeed) > skidThreshold && rb.linearVelocity.magnitude > 5f;
        if (rearLeftTrail) rearLeftTrail.emitting = skidding;
        if (rearRightTrail) rearRightTrail.emitting = skidding;

        if (Time.frameCount % 15 == 0) Debug.Log($"spd={rb.linearVelocity.magnitude:F1}  throttle={throttle:F2}");
    }

}
