using UnityEngine;
using UnityEngine.UI;  
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Setting")]
    public int targetBeacons = 3;
    public float fadeDuration = 3f;

    [Header("Progress UI")]
    public Text beaconCounterText;  

    [Header("Ending UI")]
    public CanvasGroup blackScreenGroup;
    public Text winText;  

    [Header("Typing")]
    public float typingSpeed = 0.1f;

    [TextArea]
    public string finalMessage = "MISSION ACCOMPLISHED";

    private int currentProgress = 0;
    private bool isGameEnding = false;

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

        UpdateBeaconUI();  
    }

    public void AddBeaconProgress()
    {
        currentProgress++;
        UpdateBeaconUI();

        Debug.Log($"Beacon Progress: {currentProgress}/{targetBeacons}");

        if (currentProgress >= targetBeacons)
        {
            StartEnding();
        }
    }

    void UpdateBeaconUI()
    {
        if (beaconCounterText != null)
        {
            beaconCounterText.text = $"Beacons: {currentProgress} / {targetBeacons}";
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

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (blackScreenGroup != null)
                blackScreenGroup.alpha = timer / fadeDuration;
            yield return null;
        }

        if (blackScreenGroup != null)
            blackScreenGroup.alpha = 1f;

        if (winText != null)
        {
            winText.gameObject.SetActive(true);
            winText.text = "";

            foreach (char letter in finalMessage)
            {
                winText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
        }
    }
}