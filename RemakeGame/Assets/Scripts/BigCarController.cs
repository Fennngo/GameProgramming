using UnityEngine;
using System;
using System.Collections.Generic;

public class BigCarController : MonoBehaviour
{
    public enum Axel
    {
        Front,
        Rear
    }

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public GameObject wheelEffectObj;
        public ParticleSystem smokeParticle;
        public Axel axel;
    }

    public float maxAcceleration = 30.0f;
    public float brakeAcceleration = 50.0f;
    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;
    public Vector3 _centerOfMass;
    public List<Wheel> wheels;
    [Header("Speed Effects")]
    public Material speedLineMat;
    public float effectStartSpeed = 30f;
    public float effectFullSpeed = 50f;
    [Header("Wheel Smoke Effects")]
    public float slipThreshold = 0.2f; 
    private float _currentShaderVal = 0f;

    float moveInput;
    float steerInput;
    private Rigidbody carRb;

    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = _centerOfMass;
    }
    void Update()
    {
        GetInputs();
        AnimateWheels();
        WheelEffects();
        float speed = carRb.linearVelocity.magnitude;
        HandleSpeedEffects(speed);
    }
    void LateUpdate()
    {
        Move();
        Steer();
        Brake();
    }
    void GetInputs()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }
    void Move()
    {
        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = moveInput * 600 * maxAcceleration * Time.deltaTime;
        }
    }
    void Steer()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, 0.6f);
            }
        }
    }
    void Brake()
    {
        if (Input.GetKey(KeyCode.Space) || moveInput == 0)
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 300 * brakeAcceleration * Time.deltaTime;
            }
        }
        else
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0;
            }
        }
    }
    void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rot;
            Vector3 pos;
            wheel.wheelCollider.GetWorldPose(out pos, out rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot;
        }
    }
    void WheelEffects()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Rear && wheel.wheelCollider.isGrounded)
            {
                WheelHit hit;
                if (wheel.wheelCollider.GetGroundHit(out hit))
                {
                    float forwardSlip = Mathf.Abs(hit.forwardSlip);
                    float sidewaysSlip = Mathf.Abs(hit.sidewaysSlip);
                    bool isSlipping = forwardSlip > slipThreshold || sidewaysSlip > slipThreshold;

                    bool isBraking = Input.GetKey(KeyCode.Space) && carRb.linearVelocity.magnitude >= 10.0f;

                   
                    if (isSlipping || isBraking)
                    {
                        if (wheel.wheelEffectObj != null)
                            wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = true;
                        if (wheel.smokeParticle != null)
                            wheel.smokeParticle.Emit(1);
                    }
                    else
                    {
                        if (wheel.wheelEffectObj != null)
                            wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = false;
                    }
                }
                else
                {
                    if (wheel.wheelEffectObj != null)
                        wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = false;
                }
            }
        }
    }
    void HandleSpeedEffects(float currentSpeed)
    {
        if (speedLineMat == null) return;
        float targetSpeedVal = 0f;
        int isOpen = 0;
        if (currentSpeed > effectFullSpeed * 2)
        {
            isOpen = 1;
            targetSpeedVal = 2f;
        }
        else if (currentSpeed > effectFullSpeed)
        {
            isOpen = 1;
            targetSpeedVal = 1f;
        }
        else if (currentSpeed > effectStartSpeed)
        {
            isOpen = 1;
            targetSpeedVal = 0.5f;
        }
        else
        {
            isOpen = 0;
            targetSpeedVal = 0f;
        }
        speedLineMat.SetInt("_IsOpen", isOpen);
        speedLineMat.SetFloat("_Speed", targetSpeedVal);
    }
}