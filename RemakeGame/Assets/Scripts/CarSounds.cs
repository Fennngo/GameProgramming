using UnityEngine;
using System.Collections.Generic;
using UnityEngine;

public class CarSounds : MonoBehaviour
{
    public float minSpeed;
    public float maxSpeed;
    private float currentSpeed;

    private Rigidbody carRb;
    private AudioSource carAudio;

    public float minPitch;
    public float maxPitch;
    private float pitchFromCar;

    void Start()
    {
        carAudio = GetComponent<AudioSource>();
        carRb = GetComponent<Rigidbody>();

        if (carAudio.isPlaying && carRb.linearVelocity.magnitude <= minSpeed)
        {
            carAudio.Stop();
        }
    }

    void FixedUpdate()
    {
        float currentSpeed = carRb.linearVelocity.magnitude;
        if (currentSpeed <= minSpeed)
        {
            if (carAudio.isPlaying)
                carAudio.Stop();

            carAudio.pitch = minPitch;
            return;
        }
        if (!carAudio.isPlaying)
        {
            if (carAudio.clip != null)
                carAudio.Play();
        }
        float t = Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed);
        carAudio.pitch = Mathf.Lerp(minPitch, maxPitch, t);

    }
}
