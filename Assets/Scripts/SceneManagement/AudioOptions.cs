
using UnityEngine;
using UnityEngine.UI;

public class AudioOptions : MonoBehaviour
{
    [Header("Menu")]
    public bool isMainMenu = true;
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;

    [Header("Audio")]
    public Slider volumeSlider;

    void Start()
    {
        float volume = PlayerPrefs.GetFloat("Volume", 1f);

        volumeSlider.value = volume;
        AudioListener.volume = volume;

        volumeSlider.onValueChanged.AddListener(SetVolume);

        if (isMainMenu)
        {
            mainMenuPanel.SetActive(true);
            optionsPanel.SetActive(false);
        }
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
    }

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        PlayerPrefs.Save();
    }
}
