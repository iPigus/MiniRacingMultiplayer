using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PromptManager : MonoBehaviour
{
    [SerializeField] GameObject[] Arrows;

    bool isLeftSideOn = false;

    private void OnEnable()
    {
        isLeftSideOn = false;

        Arrows[0].SetActive(isLeftSideOn);
        Arrows[1].SetActive(!isLeftSideOn);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) isLeftSideOn = true;

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) isLeftSideOn = false;

        if (Input.GetMouseButtonDown(1)) UnpauseGame();

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (isLeftSideOn)
            {
                BackToMenu();
            }
            else
            {
                UnpauseGame();
            }
        }

        Arrows[0].SetActive(isLeftSideOn);
        Arrows[1].SetActive(!isLeftSideOn);
    }

    public void BackToMenu() => SceneManager.LoadScene(0);

    public void UnpauseGame()
    {
        Time.timeScale = 1f;

        this.gameObject.SetActive(false);
    }
}
