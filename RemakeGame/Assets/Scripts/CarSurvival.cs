using UnityEngine;
using UnityEngine.UI;

public class CarSurvival : MonoBehaviour
{
    [Header("UI")]
    public Image fuelImage;      
    public Image sanityImage;

    [Header("Spawn Point")]
    public Transform spawnPoint;   

    [Header("Settings")]
    public float maxFuel = 100f;
    public float maxSanity = 100f;
    public float fuelBurnRate = 5f;       
    public float sanityDrainRate = 3f;    
    public float sanityRecoverRate = 15f; 
    public float safeZoneDelay = 2.0f;    

    private float currentFuel;
    private float currentSanity;
    private bool isInLight = false;
    private float timeSpentInLight = 0f;
    private Rigidbody rb; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ResetGame(); 
    }

    void Update()
    {
        HandleFuel();
        HandleSanity();
        UpdateUI();
        CheckDeath();
    }

    void HandleFuel()
    {
        float moveInput = Input.GetAxis("Vertical");

        if (Mathf.Abs(moveInput) > 0.1f && currentFuel > 0)
        {
            currentFuel -= fuelBurnRate * Time.deltaTime;
        }
        currentFuel = Mathf.Clamp(currentFuel, 0, maxFuel);
    }

    void HandleSanity()
    {
        if (isInLight)
        {
            timeSpentInLight += Time.deltaTime;
            if (timeSpentInLight > safeZoneDelay)
            {
                currentSanity += sanityRecoverRate * Time.deltaTime;
            }
        }
        else
        {
            timeSpentInLight = 0f;
            currentSanity -= sanityDrainRate * Time.deltaTime;
        }

        currentSanity = Mathf.Clamp(currentSanity, 0, maxSanity);
    }

    void UpdateUI()
    {
        if (fuelImage != null)
            fuelImage.fillAmount = Mathf.Lerp(fuelImage.fillAmount, currentFuel / maxFuel, Time.deltaTime * 5f);

        if (sanityImage != null)
            sanityImage.fillAmount = Mathf.Lerp(sanityImage.fillAmount, currentSanity / maxSanity, Time.deltaTime * 5f);
    }

    void CheckDeath()
    {
        if (currentFuel <= 0 || currentSanity <= 0)
        {
            Debug.Log("Game Over - Respawning...");
            ResetGame();
        }
    }

    public void ResetGame()
    {
        currentFuel = maxFuel;
        currentSanity = maxSanity;

        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void AddFuel(float amount)
    {
        currentFuel += amount;
        if (currentFuel > maxFuel) currentFuel = maxFuel;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SafeZone")) isInLight = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SafeZone")) isInLight = false;
    }
}
