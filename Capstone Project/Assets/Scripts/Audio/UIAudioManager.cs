/*************************************************
Author Names : 		Gabe Holmes
Date Created : 		01/27/2026
Date Last Modified : 01/27/2026
Brief Description : Basic AudioManager for UI
External Resources : 	
	***************************************************/
using FMOD.Studio;
using FMODUnity;
using EventReference = FMODUnity.EventReference;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioManager : AudioManager
{

    #region VARIABLES
    public static UIAudioManager Instance;

    [SerializeField] private EventReference uiHover;
    [SerializeField] private EventReference uiClick;
    [SerializeField] private EventReference uiSelect;
    [SerializeField] private EventReference uiPageFlip;

    private GameObject audioListenerObject;

    #endregion VARIABLES


    private void Awake()
    {
        Instance = this;
    }

    #region UI_FUNCTIONS
    public void PlayUIClick()
    {
        CreateEventInstance(uiClick);
        PlayOneShot(uiClick, this.transform.position);
    }
    public void PlayUISelect()
    {
        CreateEventInstance(uiSelect);
        PlayOneShot(uiSelect, this.transform.position);
    }
    public void PlayUIHover()
    {
        CreateEventInstance(uiHover);
        PlayOneShot(uiHover, this.transform.position);
    }

    public void PlayUIPageFlip()
    {
        CreateEventInstance(uiPageFlip);
        PlayOneShot(uiPageFlip, this.transform.position);
    }
    #endregion UI_FUNCTIONS
}