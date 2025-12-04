using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class BeaconSystem : MonoBehaviour
{
    [Header("Settings")]
    public float detectRange = 15f;       
    public float holdDuration = 3f;      
    public Color completedColor = Color.green; 
    [Header("Audio")]
    public AudioClip progressClip; 
    public AudioClip successClip;  

    [Header("Bindings")]
    public Transform playerCar;
    public GameObject uiPrompt;
    public Slider uiProgressBar;

    private float currentTimer = 0f;
    private bool isCompleted = false;
    private MeshRenderer meshRenderer;
    private AudioSource audioSource;
    private bool isCollecting = false;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;

        if (uiPrompt != null) uiPrompt.SetActive(false);
        if (uiProgressBar != null) uiProgressBar.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isCompleted) return;

        float distance = Vector3.Distance(transform.position, playerCar.position);

        if (distance <= detectRange)
        {
            uiPrompt.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCollectingSound();
            }

            if (Input.GetKey(KeyCode.E))
            {
                uiProgressBar.gameObject.SetActive(true);

                currentTimer += Time.deltaTime;
                uiProgressBar.value = currentTimer / holdDuration;

                if (currentTimer >= holdDuration)
                {
                    CompleteBeacon();
                }
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                StopCollectingSound();
                ResetProgress();
            }
        }
        else
        {
            if (isCollecting)
            {
                StopCollectingSound();
                ResetProgress();
            }
            uiPrompt.SetActive(false);
        }
    }

    void StartCollectingSound()
    {
        isCollecting = true;

        audioSource.clip = progressClip;
        audioSource.loop = false;
        audioSource.Play();
    }

    void StopCollectingSound()
    {
        isCollecting = false;

        audioSource.Stop();
    }

    void ResetProgress()
    {
        currentTimer = 0;
        if (uiProgressBar != null)
        {
            uiProgressBar.value = 0;
            uiProgressBar.gameObject.SetActive(false);
        }
    }

    void CompleteBeacon()
    {
        isCompleted = true;
        isCollecting = false;

        audioSource.Stop();

        if (successClip != null)
        {
            audioSource.PlayOneShot(successClip);
        }

        uiPrompt.SetActive(false);
        if (uiProgressBar != null) uiProgressBar.gameObject.SetActive(false);

        if (meshRenderer != null)
        {
            meshRenderer.material.color = completedColor;
        }

    }
}