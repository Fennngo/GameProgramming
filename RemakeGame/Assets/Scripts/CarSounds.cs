using UnityEngine;
using System.Collections;

public class CarSounds : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource accelAudio;   
    public AudioSource decelAudio;    
    public AudioSource brakeAudio;    

    public float minSpeed = 1f;
    public float maxSpeed = 50f;
    public float brakeSpeedThreshold = 5f;
    public float inputThreshold = 0.1f;
    public float fadeDuration = 0.3f;
    public float minPitch = 0.8f;
    public float maxPitch = 2.0f;

    [Header("Engine Max Volume (Important!)")]
    public float engineMaxVolume = 1f;  

    private float currentSpeed;
    private Rigidbody carRb;

    private enum EngineState { Idle, Accelerating, Decelerating }
    private EngineState currentEngineState = EngineState.Idle;

    private Coroutine accelFadeCoroutine;
    private Coroutine decelFadeCoroutine;

    private bool isBraking;
    private bool brakePressedLastFrame = false;
    private float moveInput;

    void Start()
    {
        carRb = GetComponent<Rigidbody>();

        AudioSource[] sources = GetComponents<AudioSource>();
        if (sources.Length >= 3)
        {
            accelAudio = sources[0];
            decelAudio = sources[1];
            brakeAudio = sources[2];
        }
        else
        {
            Debug.LogError("BigCar need 3  AudioSource ");
        }

        if (accelAudio) accelAudio.loop = true;
        if (decelAudio) decelAudio.loop = true;
        if (brakeAudio) brakeAudio.loop = false;

        StopEngineAudioImmediate();
    }

    void Update()
    {
        isBraking = Input.GetKey(KeyCode.Space);
        moveInput = Input.GetAxis("Vertical");

        if (isBraking && !brakePressedLastFrame && currentSpeed > brakeSpeedThreshold)
        {
            PlayBrakeSound();
        }
        brakePressedLastFrame = isBraking;
    }

    void FixedUpdate()
    {
        currentSpeed = carRb.linearVelocity.magnitude; 

        EngineState newState = EngineState.Idle;

        if (moveInput > inputThreshold && currentSpeed > minSpeed)
        {
            newState = EngineState.Accelerating;
        }
        else if (Mathf.Abs(moveInput) < inputThreshold && currentSpeed > minSpeed)
        {
            newState = EngineState.Decelerating;
        }

        if (newState != currentEngineState)
        {
            SwitchEngineState(newState);
            currentEngineState = newState;
        }
        if (currentEngineState == EngineState.Accelerating && accelAudio && accelAudio.isPlaying)
        {
            float t = Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed);
            accelAudio.pitch = Mathf.Lerp(minPitch, maxPitch, t);
        }
    }

    private void SwitchEngineState(EngineState newState)
    {
        switch (currentEngineState)
        {
            case EngineState.Accelerating:
                if (accelFadeCoroutine != null) StopCoroutine(accelFadeCoroutine);
                accelFadeCoroutine = StartCoroutine(FadeEngineAudio(accelAudio, false));
                break;
            case EngineState.Decelerating:
                if (decelFadeCoroutine != null) StopCoroutine(decelFadeCoroutine);
                decelFadeCoroutine = StartCoroutine(FadeEngineAudio(decelAudio, false));
                break;
        }

        switch (newState)
        {
            case EngineState.Accelerating:
                if (accelFadeCoroutine != null) StopCoroutine(accelFadeCoroutine);
                accelFadeCoroutine = StartCoroutine(FadeEngineAudio(accelAudio, true));
                break;
            case EngineState.Decelerating:
                if (decelFadeCoroutine != null) StopCoroutine(decelFadeCoroutine);
                decelFadeCoroutine = StartCoroutine(FadeEngineAudio(decelAudio, true));
                break;
        }
    }

    private void PlayBrakeSound()
    {
        if (brakeAudio != null && brakeAudio.clip != null)
        {
            brakeAudio.volume = 1f;
            brakeAudio.pitch = Random.Range(0.9f, 1.1f); 
            brakeAudio.PlayOneShot(brakeAudio.clip);
        }
    }

    private IEnumerator FadeEngineAudio(AudioSource audio, bool fadeIn)
    {
        if (audio == null) yield break;

        float startVolume = fadeIn ? 0f : engineMaxVolume;
        float endVolume = fadeIn ? engineMaxVolume : 0f;

        if (fadeIn && !audio.isPlaying)
            audio.Play();

        audio.volume = startVolume;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            audio.volume = Mathf.Lerp(startVolume, endVolume, timer / fadeDuration);
            yield return null;
        }

        audio.volume = endVolume;

        if (!fadeIn)
            audio.Stop();
    }

    private void StopEngineAudioImmediate()
    {
        if (accelAudio) { accelAudio.Stop(); accelAudio.volume = 0f; }
        if (decelAudio) { decelAudio.Stop(); decelAudio.volume = 0f; }
    }
}