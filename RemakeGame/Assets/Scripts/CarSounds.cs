using UnityEngine;
using System.Collections;

public class CarSounds : MonoBehaviour
{
    public float minSpeed;
    public float maxSpeed;
    private float currentSpeed;
    private Rigidbody carRb;
    private AudioSource carAudio; 
    public AudioSource brakeAudio; 
    public float minPitch;
    public float maxPitch;
    private float pitchFromCar;
    public float fadeDuration = 0.3f; 
    private bool isHandbraking = false;
    private Coroutine engineFadeCoroutine;
    private Coroutine brakeFadeCoroutine;

    void Start()
    {
        carAudio = GetComponent<AudioSource>();
        carRb = GetComponent<Rigidbody>();
        if (carAudio.isPlaying && carRb.linearVelocity.magnitude <= minSpeed)
        {
            carAudio.Stop();
        }

        if (carAudio != null)
        {
            carAudio.loop = true; 
        }
        if (brakeAudio != null)
        {
            brakeAudio.loop = true; 
            brakeAudio.Stop(); 
        }
    }

    void FixedUpdate()
    {
        currentSpeed = carRb.linearVelocity.magnitude;
        bool handbrake = Input.GetKey(KeyCode.Space);

        if (handbrake && !isHandbraking)
        {
            isHandbraking = true;
            StartBrakeAudio();
        }
        else if (!handbrake && isHandbraking)
        {
            isHandbraking = false;
            StopBrakeAudio();
        }

        if (!handbrake)
        {
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

    private void StartBrakeAudio()
    {
        if (carAudio != null && carAudio.isPlaying)
        {
            if (engineFadeCoroutine != null) StopCoroutine(engineFadeCoroutine);
            engineFadeCoroutine = StartCoroutine(FadeAudio(carAudio, false, fadeDuration));
        }
        if (brakeAudio != null)
        {
            if (brakeFadeCoroutine != null) StopCoroutine(brakeFadeCoroutine);
            brakeFadeCoroutine = StartCoroutine(FadeAudio(brakeAudio, true, fadeDuration));
        }
    }

    private void StopBrakeAudio()
    {
        if (brakeAudio != null && brakeAudio.isPlaying)
        {
            if (brakeFadeCoroutine != null) StopCoroutine(brakeFadeCoroutine);
            brakeFadeCoroutine = StartCoroutine(FadeAudio(brakeAudio, false, fadeDuration));
        }
        if (carAudio != null)
        {
            if (engineFadeCoroutine != null) StopCoroutine(engineFadeCoroutine);
            engineFadeCoroutine = StartCoroutine(FadeAudio(carAudio, true, fadeDuration));
        }
    }

    private IEnumerator FadeAudio(AudioSource audio, bool fadeIn, float duration)
    {
        float startVolume = fadeIn ? 0f : audio.volume;
        float endVolume = fadeIn ? 1f : 0f; 

        if (fadeIn && !audio.isPlaying) audio.Play();

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            audio.volume = Mathf.Lerp(startVolume, endVolume, time / duration);
            yield return null;
        }

        audio.volume = endVolume;
        if (!fadeIn) audio.Stop();
    }
}