using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuBehavior : MenuBehavior
{
    [Range(0, 100)] public int SfxVolumePercentage;
    [SerializeField] private TMP_InputField SFXInputFieldText;
    [SerializeField] private Slider SFXSlider;

    [Range(0, 100)] public int MusicVolumePercentage;
    [SerializeField] private TMP_InputField MusicInputFieldText;
    [SerializeField] private Slider MusicSlider;

    private void OnEnable()
    {
        //update volumes from fmod, then call both functions with the fmod default values

        UpdateSFXVolume("50");
        UpdateMusicVolume("50");
    }


    public void UpdateSFXVolume(string s)
    {
        if (s.EndsWith("%"))
        {
            s = s.Substring(0, s.Length - 1);
        }

        try
        {
            int percentage = int.Parse(s);

            Debug.Log(percentage);

            SfxVolumePercentage = percentage;

            SFXInputFieldText.text = percentage.ToString() + "%";
            SFXSlider.value = ((float)percentage / 100);
        }
        catch
        {
            SFXInputFieldText.text = "";

            SFXInputFieldText.text = SfxVolumePercentage.ToString() + "%";
            
            SFXSlider.value = ((float)SfxVolumePercentage / 100);
        }
        

        
    }

    public void UpdateSFXFromSlider()
    {
        int percent = Mathf.RoundToInt(SFXSlider.value * 100);

        SfxVolumePercentage = percent;

        SFXInputFieldText.text = percent.ToString() + "%";
    }


    public void UpdateMusicVolume(string s)
    {
        if (s.EndsWith("%"))
        {
            s = s.Substring(0, s.Length - 1);
        }

        try
        {
            int percentage = int.Parse(s);

            Debug.Log(percentage);

            MusicVolumePercentage = percentage;

            MusicInputFieldText.text = percentage.ToString() + "%";
            MusicSlider.value = ((float)percentage / 100);
        }
        catch
        {
            MusicInputFieldText.text = "";

            MusicInputFieldText.text = MusicVolumePercentage.ToString() + "%";

            MusicSlider.value = ((float)MusicVolumePercentage / 100);
        }



    }

    public void UpdateMusicFromSlider()
    {
        int percent = Mathf.RoundToInt(MusicSlider.value * 100);

        MusicVolumePercentage = percent;

        MusicInputFieldText.text = percent.ToString() + "%";
    }
}
