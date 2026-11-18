//Dan Ivanovas
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject optionsPanel;

    private bool isPaused;

    void Start()
    {
        //Hide the menus when starting the game
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    void Update()
    {
        //Open or close the pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else if (optionsPanel.activeSelf)
            {
                CloseOptions();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    public void PauseGame()
    {
        //Pause the game
        isPaused = true;
        pausePanel.SetActive(true);
        optionsPanel.SetActive(false);

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        //Resume the game
        isPaused = false;
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void OpenOptions()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        pausePanel.SetActive(true);

        PlayerPrefs.Save();
    }
}
