using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Setting")]
    public int targetBeacons = 3;   
    public float fadeDuration = 3f; 
    [Header("UI")]
    public CanvasGroup blackScreenGroup; 
    public TextMeshProUGUI winText;
    [Header("typingspeed")]
    public float typingSpeed = 0.1f; 
    [TextArea]
    public string finalMessage = "MISSION ACCOMPLISHED"; 

    private int currentProgress = 0;
    private bool isGameEnding = false;

    public static GameManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (blackScreenGroup != null)
        {
            blackScreenGroup.alpha = 0;
            blackScreenGroup.blocksRaycasts = false;
        }

        if (winText != null)
        {
            winText.text = "";
            winText.gameObject.SetActive(false);
        }
    }

    public void AddBeaconProgress()
    {
        currentProgress++;
        Debug.Log("Progress: " + currentProgress + "/" + targetBeacons);

        if (currentProgress >= targetBeacons)
        {
            StartEnding();
        }
    }

    void StartEnding()
    {
        if (isGameEnding) return;
        isGameEnding = true;

        StartCoroutine(PlayEndingSequence());
    }
    IEnumerator PlayEndingSequence()
    {
        float timer = 0f;
        if (winText != null) winText.gameObject.SetActive(false);

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (blackScreenGroup != null)
                blackScreenGroup.alpha = Mathf.Clamp01(timer / fadeDuration);

            yield return null; 
        }

        if (blackScreenGroup != null) blackScreenGroup.alpha = 1;

        if (winText != null)
        {
            winText.gameObject.SetActive(true);
            CanvasGroup textCanvasGroup = winText.GetComponent<CanvasGroup>();

            if (textCanvasGroup != null)
            {
                textCanvasGroup.alpha = 1f;
            }
            winText.text = "";
            foreach (char letter in finalMessage.ToCharArray())
            {
                winText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
        }
    }
}