//Dan Ivanovas
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BrightnessOptions : MonoBehaviour
{
    public Slider brightnessSlider;
    public Volume globalVolume;

    private ColorAdjustments colourAdjustments;

    void Start()
    {
        //Get the Color Adjustments from the Global Volume
        globalVolume.profile.TryGet(out colourAdjustments);

        //Load the saved brightness
        float brightness = PlayerPrefs.GetFloat("Brightness", 0f);

        colourAdjustments.postExposure.overrideState = true;
        colourAdjustments.postExposure.value = brightness;

        //Set up the brightness slider
        brightnessSlider.minValue = -1f;
        brightnessSlider.maxValue = 1f;
        brightnessSlider.SetValueWithoutNotify(brightness);
        brightnessSlider.onValueChanged.AddListener(SetBrightness);
    }

    public void SetBrightness(float brightness)
    {
        //Adjust the scene brightness
        colourAdjustments.postExposure.value = brightness;

        //Save the brightness setting
        PlayerPrefs.SetFloat("Brightness", brightness);
    }

    void OnDestroy()
    {
        brightnessSlider.onValueChanged.RemoveListener(SetBrightness);
    }
}
