using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] Cars;
    [SerializeField] GameObject LeavePrompt;

    private void Awake()
    {
        GameObject carSpawned = Instantiate(Cars[PlayerPrefs.GetInt("CarChosen")], transform.position, transform.rotation);

        if (GetComponent<NetworkManager>())
        {
            carSpawned.AddComponent<CarNetwork>().CarId = 0;
        }

        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            LeavePrompt.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
