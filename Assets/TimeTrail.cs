using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.Rendering;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class TimeTrail : MonoBehaviour
{
    public static TimeTrail Singleton { get; private set; }

    Stopwatch stopwatch = new Stopwatch();

    [SerializeField] TMPro.TextMeshProUGUI CountdownText;
    [SerializeField] GameObject CountdownCanvas;

    [SerializeField] TMPro.TextMeshProUGUI LapTimeText;
    [SerializeField] GameObject LapTimeCanvas;


    [SerializeField] bool[] Zones;

    bool isAllZonesGood => !Zones.Where(x => x == false).Any();

    private void Awake()
    {
        Singleton = this;

        Time.timeScale = 0f;

        LapTimeCanvas.SetActive(false);

        StartCoroutine(StartGameCountdown());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if (Input.GetKeyDown(KeyCode.L)) PlayerPrefs.SetFloat("BestTime", 0f);
    }

    public void EnteredZone(int zoneIndex)
    {
        Zones[zoneIndex] = true;
    }

    public void FinishLine()
    {
        if (isAllZonesGood)
        {
            for (int i = 0; i < Zones.Length; i++)
            {
                Zones[i] = false;
            }

            DisplayLapTime();
        }
    }

    public void DisplayLapTime()
    {
        if (PlayerPrefs.GetFloat("BestTime") == 0f)
        {
            LapTimeText.text = stopwatch.Elapsed.TotalSeconds.ToString() + "s";
            PlayerPrefs.SetFloat("BestTime", (float)stopwatch.Elapsed.TotalSeconds);
        }
        else
        {
            if((float)stopwatch.Elapsed.TotalSeconds > PlayerPrefs.GetFloat("BestTime"))
                LapTimeText.text = string.Format("{0:#,0.000}", stopwatch.Elapsed.TotalSeconds) + "s" + " <color=red>+" + string.Format("{0:#,0.000}", ((float)stopwatch.Elapsed.TotalSeconds - PlayerPrefs.GetFloat("BestTime"))) + "</color>";
            else
            {
                LapTimeText.text = string.Format("{0:#,0.000}", stopwatch.Elapsed.TotalSeconds) + "s" + " <color=green>" + string.Format("{0:#,0.000}", ((float)stopwatch.Elapsed.TotalSeconds - PlayerPrefs.GetFloat("BestTime"))) + "</color>";
                PlayerPrefs.SetFloat("BestTime", (float)stopwatch.Elapsed.TotalSeconds);
            }
        }

        stopwatch.Restart();

        LapTimeCanvas.SetActive(true);

        StartCoroutine(UnDisplayLapTime());
    }

    IEnumerator UnDisplayLapTime()
    {
        yield return new WaitForSecondsRealtime(1.5f);   
        LapTimeCanvas.SetActive(false);
    }

    IEnumerator StartGameCountdown()
    {
        CountdownCanvas.SetActive(true);                 

        yield return new WaitForSecondsRealtime(1f);
        CountdownText.text = 3.ToString();
        yield return new WaitForSecondsRealtime(1f);
        CountdownText.text = 2.ToString();
        yield return new WaitForSecondsRealtime(1f);
        CountdownText.text = 1.ToString();
        yield return new WaitForSecondsRealtime(1f);

        CountdownCanvas.SetActive(false);
        stopwatch.Start();
        Time.timeScale = 1f;
    }
}
