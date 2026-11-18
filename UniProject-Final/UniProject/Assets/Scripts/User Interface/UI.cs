//Dan Ivanovas
using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{


    public TextMeshProUGUI waveText;
    public static int waveValue;

    public TextMeshProUGUI healthText;

    //Reference variable from other scripts like this makes sure to name something clear
    public PlayerHealth playerHealth;

    public TextMeshProUGUI currencyText;
    public CurrencyManager currencyManager;

    public GameObject pausePanel;
    public GameObject optionsPanel;

    private bool isPaused;

    private int previousWave = -1;
    private int previousHealth = -1;
    private int previousCurrency = -1;

    void Start()
    {
        //TimeScale is the speed at which the game is being run at
        //We can make game objects active or turned off using set active great for pauses / jumpscares (not for this game)
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
        Time.timeScale = 1f;

        //Safety Checks
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }

        if (currencyManager == null)
        {
            currencyManager = FindFirstObjectByType<CurrencyManager>();
        }
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

        //Null checks can be used to debug code easier because one of the common beginner mistakes is a null reference exception

        //Though if you dont like them you can write the code like this

        //waveText.text = "Round " + (waveValue + 1);

        //healthText.text = playerHealth.CurrentHealth.ToString();

        //currencyText.text = currencyManager.CurrentCurrency.ToString();

        //And It will work perfectly fine

        //Only update the wave text when the wave changes
        if (waveValue != previousWave)
        {
            waveText.text = "Round " + (waveValue + 1);
            previousWave = waveValue;
        }

        //Only update health when the player takes damage
        if (playerHealth.CurrentHealth != previousHealth)
        {
            healthText.text = playerHealth.CurrentHealth.ToString();
            previousHealth = playerHealth.CurrentHealth;
        }

        //Only update currency when the amount changes
        if (currencyManager.CurrentCurrency != previousCurrency)
        {
            currencyText.text = currencyManager.CurrentCurrency.ToString();
            previousCurrency = currencyManager.CurrentCurrency;
        }
    }

    //Pause Functions
    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
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
