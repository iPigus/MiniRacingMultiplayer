using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static NetworkManager;

public class CarSpawner : MonoBehaviour
{
    public static CarSpawner Singleton { get; private set; }

    [SerializeField] GameObject[] Cars;
    [SerializeField] GameObject LeavePrompt;

    private void Awake()
    {
        Singleton = this;

        if (FindObjectOfType<ClientManager>()) return;

        GameObject carSpawned = Instantiate(Cars[PlayerPrefs.GetInt("CarChosen")], transform.position, transform.rotation);

        if (FindObjectOfType<ServerManager>())
        {
            carSpawned.AddComponent<CarNetwork>().CarId = 0;
            ServerManager.SetCarInfo(0,(ushort)PlayerPrefs.GetInt("CarChosen"), carSpawned.transform.position, carSpawned.GetComponent<Rigidbody2D>().rotation);
        }
        
        if(!SceneManager.GetActiveScene().name.Contains("Time"))
            Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            LeavePrompt.SetActive(true);

            if(!FindObjectOfType<NetworkManager>())
                Time.timeScale = 0f;
        }
    }

    [MessageHandler((ushort)ServerToClientId.carInfos)]
    public static void CarSpawnFromMessage(Message message)
    {
        ushort playerId = message.GetUShort();
        ushort carId = message.GetUShort();

        GameObject carSpawned = Instantiate(Singleton.Cars[carId], message.GetVector2(), Quaternion.identity);

        carSpawned.AddComponent<CarNetwork>().CarId = playerId;
        carSpawned.GetComponent<Rigidbody2D>().rotation = message.GetFloat();
    }
}
