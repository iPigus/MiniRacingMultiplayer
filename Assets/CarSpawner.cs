using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] Cars;

    private void Awake()
    {
        Instantiate(Cars[PlayerPrefs.GetInt("CarChosen")], transform.position, transform.rotation);
    }
}
