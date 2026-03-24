using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuBehavior : MenuBehavior
{
    [Range(0, 100)] public int SfxVolumePercentage;
    [SerializeField] private TMP_InputField SFXInputFieldText;
    [SerializeField] private Slider SFXSlider;

    [SerializeField, Range(0.01f, 5f)] private float SFXTestCooldown = 0.5f;
    private float currentCooldown;
    private bool canPlaySfx;

    [Range(0, 100)] public int MusicVolumePercentage;
    [SerializeField] private TMP_InputField MusicInputFieldText;
    [SerializeField] private Slider MusicSlider;

    [SerializeField] private Bus masterBus;
    [SerializeField] private Bus soundEffectsBus;
    [SerializeField] private Bus backgroundMusicBus;

    
    private void Awake()
    {
        GetBusses();
        UpdateSFXVolume("50");
        UpdateMusicVolume("50");
    }

    /// <summary>
    /// cooldown for the sfx
    /// </summary>
    private void Update()
    {
        if (!canPlaySfx)
        {

            currentCooldown += Time.deltaTime;
            if (currentCooldown >= SFXTestCooldown)
            {
                currentCooldown = 0;
                canPlaySfx = true;
            }
        }
    }

    public void GetBusses()
    {
        backgroundMusicBus = RuntimeManager.GetBus("bus:/BGM");
        soundEffectsBus = RuntimeManager.GetBus("bus:/SFX");
        masterBus = RuntimeManager.GetBus("bus:/");
    }

    /// <summary>
    /// updates the sfx volume from the input field
    /// </summary>
    /// <param name="s"></param>
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
            soundEffectsBus.setVolume(SfxVolumePercentage / 100f);
        }
        catch
        {
            SFXInputFieldText.text = "";

            SFXInputFieldText.text = SfxVolumePercentage.ToString() + "%";
            
            SFXSlider.value = ((float)SfxVolumePercentage / 100);
        }

        PlaySoundEffect();
        
    }

    /// <summary>
    /// updates the sfx volume from the slider
    /// </summary>
    public void UpdateSFXFromSlider()
    {
        int percent = Mathf.RoundToInt(SFXSlider.value * 100);

        SfxVolumePercentage = percent;

        SFXInputFieldText.text = percent.ToString() + "%";

        soundEffectsBus.setVolume(SfxVolumePercentage / 100f);

        PlaySoundEffect();
    }

    /// <summary>
    /// updates the music volume when the player inputs in the input field
    /// </summary>
    /// <param name="s"></param>
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
            backgroundMusicBus.setVolume(MusicVolumePercentage / 100f);
        }
        catch
        {
            MusicInputFieldText.text = "";

            MusicInputFieldText.text = MusicVolumePercentage.ToString() + "%";

            MusicSlider.value = ((float)MusicVolumePercentage / 100);
        }



    }

    /// <summary>
    /// Updates the music volume when the player moves the slider
    /// </summary>
    public void UpdateMusicFromSlider()
    {
        int percent = Mathf.RoundToInt(MusicSlider.value * 100);

        MusicVolumePercentage = percent;

        MusicInputFieldText.text = percent.ToString() + "%";

        backgroundMusicBus.setVolume(MusicVolumePercentage / 100f);
    }

    /// <summary>
    /// Plays a test sound effect when changing sfx volume
    /// </summary>
    private void PlaySoundEffect()
    {
        if (canPlaySfx)
        {
            //PLAY TEST SFX HERE
            Debug.Log("playing sfx");
            canPlaySfx = false;
        }
    }
}
