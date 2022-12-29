using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject MainScreen;
    [SerializeField] GameObject SelectionScreen;
    [SerializeField] GameObject GamemodeSelectionScreen;
    [SerializeField] GameObject MultiSelectionScreen;
    [SerializeField] GameObject CustomizeCarScreen;

    bool isMultiplayerSelected = false;
    bool isOnMainScreen => MainScreen.activeSelf;
    bool isInSelectionScreen => SelectionScreen.activeSelf;
    bool isInGamemodeSelectionScreen => GamemodeSelectionScreen.activeSelf;
    bool isInMultiSelectionScreen => MultiSelectionScreen.activeSelf;
    bool isInCustomizeCarScreenScreen => CustomizeCarScreen.activeSelf;

    #region Arrows and Controllers for them

    [SerializeField] GameObject[] ArrowsInCarSelection; // the first one is back arrow 

    int _carSelected = 2;
    public int CarSelected
    {
        get => _carSelected;
        set
        {
            if (value >= 0 && value <= 6) _carSelected = value;
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

    [SerializeField] GameObject[] ArrowsInCustomizeCar;
    int _SelectedCustomizeRow = 1;
    int SelectedCustomizeRow
    {
        get => _SelectedCustomizeRow;
        set
        {
            if (value < 0 || value > 5) return; _SelectedCustomizeRow = value;
        }
    }

    #endregion

    private void Awake()
    {
        Application.runInBackground = true;

        CheckForNetworkManagers();
        CheckForBuildVersion();

        EnterMainMenu();
        CarSelectionArrowsUpdate();
        GamemodeSelectionArrowsUpdate();
        MultiSelectionArrowsUpdate();
        CustomizeCarArrowsUpdate();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            Back(); return;
        }

        if (isOnMainScreen)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) EnterSelectionScreen();
        }
        else if (isInSelectionScreen)
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
                switch (YposMultiSelection)
                {
                    case 0: Back(); break;
                    case 1: TryToHost(); break;
                    case 2: TryToConnect(); break;

                    default: Debug.LogError("YposMulti switch unreachable!"); break;
                }
            }

            MultiSelectionArrowsUpdate();
        }
        else if (isInCustomizeCarScreenScreen)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) SelectedCustomizeRow--;
            else
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) SelectedCustomizeRow++;
            else
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) MoveLeft();
            else
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) MoveRight();
            else
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                switch (SelectedCustomizeRow)
                {
                    case 0: Back(); break;
                    case 5: SelectCustomCar(); break;
                }
            }

            CustomizeCarArrowsUpdate();
        }
    }


    public void Back()
    {
        if (isInSelectionScreen) EnterMainMenu();
        if (isInGamemodeSelectionScreen) EnterSelectionScreen();
        if (isInMultiSelectionScreen) EnterGamemodeSelection();
    }

    #region Hosting and Connecting
    public void TryToHost()
    {
        SceneManager.LoadScene(3);
    }

    public void TryToConnect()
    {
        if (FindObjectOfType<ClientManager>()) return;
        DontDestroyOnLoad(new GameObject().AddComponent<ClientManager>());
        ClientManager.Singleton.Connect();
    }
    #endregion

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
    void MultiSelectionArrowsUpdate()
    {
        for (int i = 0; i < ArrowsInMultiSelection.Length; i++)
        {
            if (i == YposMultiSelection)
            {
                if (!ArrowsInMultiSelection[i].activeSelf) ArrowsInMultiSelection[i].SetActive(true);
            }
            else
            {
                if (ArrowsInMultiSelection[i].activeSelf) ArrowsInMultiSelection[i].SetActive(false);
            }
        }
    }

    void CustomizeCarArrowsUpdate()
    {
        for (int i = 0; i < ArrowsInCustomizeCar.Length; i++)
        {
            if (i == SelectedCustomizeRow)
            {
                if (!ArrowsInCustomizeCar[i].activeSelf) ArrowsInCustomizeCar[i].SetActive(true);
            }
            else
            {
                if (ArrowsInCustomizeCar[i].activeSelf) ArrowsInCustomizeCar[i].SetActive(false);
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

    #region NetworkManager Preventions

    void CheckForNetworkManagers()
    {
        if (FindObjectOfType<NetworkManager>())
        {
            NetworkManager[] objectsToDestroy = FindObjectsOfType<NetworkManager>();

            for (int i = 0; i < objectsToDestroy.Length; i++)
            {
                Destroy(objectsToDestroy[i]);
            }
        }
    }

    #endregion

    #region Play Modes Handling
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

    #endregion

    #region Customize Car Handling

    [Header("Custom Car Things")]
    [SerializeField] Image CarImage;
    [SerializeField] TextMeshProUGUI SpeedText;
    [SerializeField] TextMeshProUGUI AccerelationText;
    [SerializeField] TextMeshProUGUI DriftnessText;

    [SerializeField] Sprite[] SelectableCars;
    [SerializeField] int minSpeed = 8;
    [SerializeField] int maxSpeed = 60;
    [SerializeField] int minAccerelation = 5;
    [SerializeField] int maxAccerelation = 20;
    [SerializeField] int minDriftness = 5;
    [SerializeField] int maxDriftness = 100;


    int actualSprite
    {
        get => PlayerPrefs.GetInt("CustomSprite");
        set
        {
            if (value >= SelectableCars.Length) PlayerPrefs.SetInt("CustomSprite", 0);
            if ( value < 0) PlayerPrefs.SetInt("CustomSprite", SelectableCars.Length - 1);
            else PlayerPrefs.SetInt("CustomSprite", value);
        }
    }
    int actualSpeed
    {
        get => PlayerPrefs.GetInt("CustomSpeed");
        set
        {
            if (value > maxSpeed || value < minSpeed || Mathf.Sqrt((float)(value * actualAccerelation)) > 28f) return;

            PlayerPrefs.SetInt("CustomSpeed", value);
        }
    }
    int actualAccerelation
    {
        get => PlayerPrefs.GetInt("CustomAccerelation");
        set
        {
            if (value > minAccerelation || value < maxAccerelation || Mathf.Sqrt((float)(value * actualAccerelation)) > 28f) return;

            PlayerPrefs.SetInt("CustomAccerelation", value);
        }
    }
    int actualDriftness
    {
        get => PlayerPrefs.GetInt("CustomDriftness");
        set
        {
            if (value > minDriftness || value < maxDriftness) return;

            PlayerPrefs.SetInt("CustomDriftness", value);
        }
    }

    void MoveLeft()
    {
        switch (SelectedCustomizeRow)
        {
            case 1: actualSprite--; break;
            case 2: actualSpeed--;break;
            case 3: actualAccerelation--; break;
            case 4: actualDriftness--; break;
            default: break;
        }

        UpdateCustomTextsAndImageInCarCustomize();
    }

    void MoveRight()
    {
        switch (SelectedCustomizeRow)
        {
            case 1: actualSprite++; break;
            case 2: actualSpeed++; break;
            case 3: actualAccerelation++; break;
            case 4: actualDriftness++; break;
            default: break;
        }

        UpdateCustomTextsAndImageInCarCustomize();
    }

    void UpdateCustomTextsAndImageInCarCustomize()
    {
        CarImage.sprite = SelectableCars[actualSprite];
        SpeedText.text = "Speed " + actualSpeed.ToString(); 
        AccerelationText.text = "Accerelation " + actualSpeed.ToString(); 
        DriftnessText.text = "Driftness " + actualSpeed.ToString(); 
    }

    #endregion

    #region Reset Stats For Legacy Builds

    void CheckForBuildVersion()
    {
        int version = PlayerPrefs.GetInt("essunia127");

        if (version == 0)
        {
            PlayerPrefs.SetFloat("BestTime", 0f);
            PlayerPrefs.SetInt("CustomDriftness", 80);
            PlayerPrefs.SetInt("CustomSprite", 0);
            PlayerPrefs.SetInt("CustomSpeed", 30);
            PlayerPrefs.SetInt("CustomAccerelation", 15);


            PlayerPrefs.SetInt("essunia127", 1);
        }
    }

    #endregion

    public void SelectCar(int carId)
    {
        PlayerPrefs.SetInt("CarChosen", carId - 1);
        PlayerPrefs.SetInt("isCarCustom", 0);

        EnterGamemodeSelection();
    }
    public void SelectCar() => SelectCar(CarSelected);

    public void SelectCustomCar()
    {
        PlayerPrefs.SetInt("isCarCustom", 1);

        EnterGamemodeSelection();
    }
}
