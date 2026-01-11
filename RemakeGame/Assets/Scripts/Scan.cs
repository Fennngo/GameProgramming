using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SonarAbility : MonoBehaviour
{
    [Header("Setting")]
    public float cooldownTime = 30f; 
    public KeyCode triggerKey = KeyCode.E;
    public float scanSpeed = 30f;    
    public float maxRange = 150f;
    public ParticleSystem sonarVFX;    
    public AudioSource audioSource;    

    [Header("Sound")]
    public AudioClip scanSound;        
    public AudioClip rechargeSound;   

    private float lastScanTime = -999f;
    private bool isReady = true;

    void Update()
    {
        if (!isReady)
        {
            if (Time.time > lastScanTime + cooldownTime)
            {
                isReady = true;
                if (audioSource && rechargeSound) audioSource.PlayOneShot(rechargeSound);
            }
        }

        if (Input.GetKeyDown(triggerKey) && isReady)
        {
            Fire();
        }
    }

    void Fire()
    {
        isReady = false;
        lastScanTime = Time.time;

        if (audioSource && scanSound) audioSource.PlayOneShot(scanSound);
        if (sonarVFX != null) sonarVFX.Play();

        StartCoroutine(ScanWaveRoutine());

        if (audioSource != null && scanSound != null)
        {
            audioSource.PlayOneShot(scanSound, 3.0f);
        }
    }

    IEnumerator ScanWaveRoutine()
    {
        float currentRadius = 0f;

        HashSet<GameObject> scannedObjects = new HashSet<GameObject>();

        while (currentRadius < maxRange)
        {
            currentRadius += scanSpeed * Time.deltaTime;
            Collider[] hits = Physics.OverlapSphere(transform.position, currentRadius);
            foreach (var hit in hits)
            {
                if (!scannedObjects.Contains(hit.gameObject))
                {
                    ScannableObject target = hit.GetComponent<ScannableObject>();
                    if (target != null)
                    {
                        target.OnScanned();
                        scannedObjects.Add(hit.gameObject);
                    }
                }
            }
            yield return null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }
}