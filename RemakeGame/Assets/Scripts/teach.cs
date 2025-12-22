using UnityEngine;
using System.Collections;

public class ControlHintFade : MonoBehaviour
{
    [Header("UI setting")]
    public CanvasGroup hintCanvasGroup; 
    public float fadeDuration = 1.0f;   

    private bool isFading = false;

    void Update()
    {
        if (isFading) return;

        bool inputDetected =
            Input.GetAxis("Horizontal") != 0 || 
            Input.GetAxis("Vertical") != 0 ||   
            Input.GetKeyDown(KeyCode.E) ||      
            Input.GetKeyDown(KeyCode.F) ||      
            Input.GetKeyDown(KeyCode.Space);    

        if (inputDetected)
        {
            StartCoroutine(FadeOutUI());
        }
    }

    IEnumerator FadeOutUI()
    {
        isFading = true;
        float startAlpha = hintCanvasGroup.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            hintCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0, time / fadeDuration);
            yield return null;
        }

        hintCanvasGroup.alpha = 0;
        gameObject.SetActive(false); 
    }
}