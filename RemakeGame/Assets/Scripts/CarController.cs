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
        throttle = Mathf.MoveTowards(throttle, v, Time.deltaTime * 4f);
        targetSteer = h;
        steer = Mathf.Lerp(steer, targetSteer, Time.deltaTime * steerResponse);

        float speed01 = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);
        float steerDegPerSec = Mathf.Lerp(steerAtZero, steerAtMax, speed01);
        float turn = steer * steerDegPerSec * Time.deltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));

        float exhaustRate = Mathf.Lerp(exhaustRateIdle, exhaustRateMax, Mathf.Abs(throttle));
        if (exhaustLeft) { var em = exhaustLeft.emission; em.rateOverTime = exhaustRate; }
        if (exhaustRight) { var em = exhaustRight.emission; em.rateOverTime = exhaustRate; }
    }

    void FixedUpdate()
    {
        // 计算当前局部速度
        localVel = transform.InverseTransformVector(rb.linearVelocity);

        // 按下的油门或刹车
        float forwardForce = 0f;

        // 判断是否按下了 S 键进行刹车
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
        if (forwardSpeed > 1f && throttle < 0f)
        {
            throttle = -1f;  // 视为强力刹车
        }

        if (throttle > 0f) forwardForce = accel * throttle;
        else if (throttle < 0f) forwardForce = brake * throttle;

        // 处理阻力：松油门时增加拖拽
        float drag = (Mathf.Approximately(throttle, 0f)) ? dragWhenNoInput : (throttle < 0f ? dragWhenBraking : 0f);
        Vector3 vel = rb.linearVelocity * (1f / (1f + drag * Time.fixedDeltaTime));
        rb.linearVelocity = vel;

        // 施加前进力
        rb.AddForce(transform.forward * forwardForce, ForceMode.VelocityChange);

        // 手刹漂移：按空格时降低抓地力
        bool handbrake = Input.GetKey(KeyCode.Space);
        float grip = handbrake ? lateralFriction * 0.35f : lateralFriction;  // 按空格时减低抓地力
        Vector3 worldRight = transform.right;
        float lateralSpeed = Vector3.Dot(rb.linearVelocity, worldRight);
        Vector3 lateralCancel = -worldRight * lateralSpeed * grip;
        rb.AddForce(lateralCancel, ForceMode.Acceleration);

        // 添加下压力
        rb.AddForce(-Vector3.up * downforce * rb.linearVelocity.magnitude, ForceMode.Acceleration);

        // 限速
        Vector3 fwd = transform.forward * Vector3.Dot(rb.linearVelocity, transform.forward);
        fwd = Vector3.ClampMagnitude(fwd, maxSpeed);
        Vector3 side = worldRight * Vector3.Dot(rb.linearVelocity, worldRight);
        rb.linearVelocity = fwd + side;

        // 轮胎印迹：当车辆打滑时显示
        bool skidding = Mathf.Abs(lateralSpeed) > skidThreshold && rb.linearVelocity.magnitude > 5f;
        if (rearLeftTrail) rearLeftTrail.emitting = skidding;
        if (rearRightTrail) rearRightTrail.emitting = skidding;

        // 打印速度和油门
        if (Time.frameCount % 15 == 0) Debug.Log($"spd={rb.linearVelocity.magnitude:F1}  throttle={throttle:F2}");
    }

}
