using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject MainScreen;
    [SerializeField] GameObject SelectionScreen;

    bool isOnMainScreen = true;

    int _carSelected = 0;
    public int CarSelected
    {
        get => _carSelected;
        set
        {
            if(value >= 0 && value < 6) _carSelected = value;
        }
    }

    private void Awake()
    {
        EnterMainMenu();
    }

    private void Update()
    {
        if (isOnMainScreen)
        {
            if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) EnterSelectionScreen();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EnterMainMenu(); return;
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) CarSelected++;
            else 
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) CarSelected--;
        }
    }

    public void EnterSelectionScreen()
    {
        isOnMainScreen = false;

        MainScreen.SetActive(isOnMainScreen);
        SelectionScreen.SetActive(!isOnMainScreen);
    }
    public void BackToMenu() => EnterMainMenu();
    void EnterMainMenu()
    {
        isOnMainScreen = true;

        MainScreen.SetActive(isOnMainScreen);
        SelectionScreen.SetActive(!isOnMainScreen);
    }

    public void SelectCar(int carId)
    {
        PlayerPrefs.SetInt("CarChosen", carId - 1);

        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
    public void SelectCar() => SelectCar(CarSelected);
}
