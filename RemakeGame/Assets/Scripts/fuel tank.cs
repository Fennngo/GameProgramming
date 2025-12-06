using UnityEngine;

public class FuelItem : MonoBehaviour
{
    [Header("Settings")]
    public float restoreAmount = 30f; 
    public AudioClip pickupSound;     
    public GameObject pickupEffect;  

    void OnTriggerEnter(Collider other)
    {
        CarSurvival car = other.GetComponent<CarSurvival>();
        if (car == null) car = other.GetComponentInParent<CarSurvival>();

        if (car != null)
        {
            car.AddFuel(restoreAmount);

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}