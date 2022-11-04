using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject MainScreen;
    [SerializeField] GameObject SelectionScreen;
    [SerializeField] GameObject GamemodeSelectionScreen;
    [SerializeField] GameObject MultiSelectionScreen;

    bool isMultiplayerSelected = false;
    bool isOnMainScreen => MainScreen.activeSelf;
    bool isInSelectionScreen => SelectionScreen.activeSelf;
    bool isInGamemodeSelectionScreen => GamemodeSelectionScreen.activeSelf;
    bool isInMultiSelectionScreen => MultiSelectionScreen.activeSelf;

    #region Arrows and Controllers for them

    [SerializeField] GameObject[] ArrowsInCarSelection; // the first one is back arrow 

    int _carSelected = 2;
    public int CarSelected
    {
        get => _carSelected;
        set
        {
            if(value >= 0 && value <= 6) _carSelected = value;
        }
    }

    [SerializeField] GameObject[] ArrowsInGamemodeSelection;

    int _YposGamemode = 2;
    int YposGamemode
    {
        get => _YposGamemode;
        set
        {
            if (value < 0 || ArrowsInGamemodeSelection.Length <= value) return;

            _YposGamemode = value;
        }
    }

    [SerializeField] GameObject[] ArrowsInMultiSelection;
    [SerializeField] TMP_InputField ipField;

    int _YposMultiSelection = 2;
    int YposMultiSelection
    {
        get => _YposMultiSelection;
        set
        {
            if (value < 0 || ArrowsInMultiSelection.Length <= value) return;

            _YposMultiSelection = value;
        }
    }

    #endregion

    private void Awake()
    {
        EnterMainMenu();
        CarSelectionArrowsUpdate();
        GamemodeSelectionArrowsUpdate();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            Back(); return;
        }

        if (isOnMainScreen)
        {
            if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) EnterSelectionScreen();
        }
        else if(isInSelectionScreen)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) CarSelected++;
            else
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) CarSelected--;
            else
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (CarSelected < 4) CarSelected = 0; else CarSelected -= 3;
            }
            else
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (CarSelected == 0) CarSelected++; else CarSelected += 3;
            }
            else
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) if (CarSelected == 0) Back(); else SelectCar();

            CarSelectionArrowsUpdate();
        }
        else if (isInGamemodeSelectionScreen)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) YposGamemode--;
            else
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) YposGamemode++;
            else
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                switch (YposGamemode)
                {
                    case 0: Back(); break;
                    case 1: PlayFreeplay(); break;
                    case 2: PlayTimetrail(); break;
                    case 3: EnterMultiSelection(); break;
                    case 4:
                    default: Debug.LogError("YposGamemode switch unreachable!"); break;
                }
            }

            GamemodeSelectionArrowsUpdate();
        }
        else if (isInMultiSelectionScreen)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) YposMultiSelection--;
            else
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) YposMultiSelection++;
            else
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                switch (YposGamemode)
                {
                    case 0: Back(); break;
                    case 1: 
                    case 2: PlayerPrefs.SetString("ip", ipField.text); DontDestroyOnLoad(new GameObject().AddComponent<ClientManager>()); break;
                    default: Debug.LogError("YposMulti switch unreachable!"); break;
                }
            }
        }
    }

   
    public void Back()
    {
        if (isInSelectionScreen) EnterMainMenu();
        if (isInGamemodeSelectionScreen) EnterSelectionScreen();
        if (isInMultiSelectionScreen) EnterGamemodeSelection();
    }

    #region Updating Arrows

    void CarSelectionArrowsUpdate()
    {
        for (int i = 0; i < ArrowsInCarSelection.Length; i++)
        {
            if (i == CarSelected)
            {
                if (!ArrowsInCarSelection[i].activeSelf) ArrowsInCarSelection[i].SetActive(true);
            }
            else
            {
                if (ArrowsInCarSelection[i].activeSelf) ArrowsInCarSelection[i].SetActive(false);
            }
        }        
    }
    void GamemodeSelectionArrowsUpdate()
    {
        for (int i = 0; i < ArrowsInGamemodeSelection.Length; i++)
        {
            if (i == YposGamemode)
            {
                if (!ArrowsInGamemodeSelection[i].activeSelf) ArrowsInGamemodeSelection[i].SetActive(true);
            }
            else
            {
                if (ArrowsInGamemodeSelection[i].activeSelf) ArrowsInGamemodeSelection[i].SetActive(false);
            }
        }
    }

    #endregion

    #region Entrance Menu Sections

    void EnterMenuSection(int section)
    {
        MainScreen.SetActive(section == 0);
        SelectionScreen.SetActive(section == 1);
        GamemodeSelectionScreen.SetActive(section == 2);
        MultiSelectionScreen.SetActive(section == 3);
    }
    void EnterMainMenu() => EnterMenuSection(0);
    public void EnterSelectionScreen() => EnterMenuSection(1);
    void EnterGamemodeSelection() => EnterMenuSection(2);
    void EnterMultiSelection() => EnterMenuSection(3);

    #endregion

    public void PlayFreeplay()
    {
        SceneManager.LoadScene(1);
    }
    public void PlayTimetrail()
    {
        SceneManager.LoadScene(2);
    }
    public void PlayMultiplayer()
    {
        EnterMultiSelection();
    }
    public void PlayBots()
    {

    }

    public void SelectCar(int carId)
    {
        PlayerPrefs.SetInt("CarChosen", carId - 1);

        EnterGamemodeSelection();
    }
    public void SelectCar() => SelectCar(CarSelected);
}
