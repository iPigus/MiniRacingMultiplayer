using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using static NetworkManager;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Singleton { get; private set; }

    Stopwatch Timer = new Stopwatch();

    bool isServer => NetworkManager.isServer;

    [SerializeField] TMPro.TextMeshProUGUI TimerText;
    [SerializeField] GameObject TimerCanvas;

    private void Awake()
    {
        Singleton = this;

        if (TimerCanvas == null) TimerCanvas = this.gameObject;
        if (TimerText == null) TimerText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    public void StartCountdown(int startingNumber = 3) => StartCoroutine(Countdown(startingNumber));
    IEnumerator Countdown(int startingNumber)
    {
        TimerCanvas.SetActive(true);
        TimerText.text = startingNumber.ToString();

        Time.timeScale = 0f;

        for (int i = startingNumber; i > 0; i--)
        {
            yield return new WaitForSecondsRealtime(1);

            TimerText.text = startingNumber.ToString();
        }

        TimerCanvas.SetActive(false);

        Time.timeScale = 1f;
    }


}
