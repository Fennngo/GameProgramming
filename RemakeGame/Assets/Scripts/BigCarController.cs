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
        Wheeleffects();

        float speed = carRb.linearVelocity.magnitude;

        Debug.Log("speed: " + speed.ToString("F2") + " m/s");

        if (speedLineMat != null)
        {
            float currentSpeed = carRb.linearVelocity.magnitude; 

         
            if (currentSpeed > effectStartSpeed)
            {
                speedLineMat.SetInt("_IsOpen", 1); 

                float t = Mathf.InverseLerp(effectStartSpeed, effectFullSpeed, currentSpeed);
                speedLineMat.SetFloat("_Speed", t);
            }
            else
            {
                speedLineMat.SetInt("_IsOpen", 0); 
                speedLineMat.SetFloat("_Speed", 0);
            }
        }

    }

    void LateUpdate()
    {
        Move();
        Steer();
        Barke();
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

    void Barke()
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

    void Wheeleffects()
    {
        foreach (var wheel in wheels)
        {
            if (Input.GetKey(KeyCode.Space) && wheel.axel == Axel.Rear & wheel.wheelCollider.isGrounded == true && carRb.linearVelocity.magnitude >= 10.0f)
            {
                wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = true;
                wheel.smokeParticle.Emit(1);
            }
            else
            {
                wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = false;
            }
        }
    }
    

}
