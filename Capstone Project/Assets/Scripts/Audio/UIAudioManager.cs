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
using static SkillTreeNode;

public class UIAudioManager : AudioManager
{

    #region VARIABLES
    public static UIAudioManager Instance;

    [SerializeField] private EventReference uiHover;
    [SerializeField] private EventReference uiClick;
    [SerializeField] private EventReference uiSelect;
    [SerializeField] private EventReference uiConfirm;
    [SerializeField] private EventReference uiCancel;
    [SerializeField] private EventReference uiPageFlip;

    [SerializeField] private EventReference closePopUp;

    [SerializeField] private EventReference spellPickUp;
    [SerializeField] private EventReference spellDrop;

    [SerializeField] private EventReference lockedSpellCLick;

    //NodeStatus _nodeStatus = NodeStatus.Purchased;



    #endregion VARIABLES


    private void Awake()
    {
        Instance = this;
    }

    #region UI_FUNCTIONS
    public void PlayUIConfirm()
    {
        CreateEventInstance(uiConfirm);
        PlayOneShot(uiConfirm, this.transform.position);
    }
    public void PlayUICancel()
    {
        CreateEventInstance(uiCancel);
        PlayOneShot(uiCancel, this.transform.position);
    }
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

    public void PlayUILockedSpell()
    {
        CreateEventInstance(lockedSpellCLick);
        PlayOneShot(lockedSpellCLick, this.transform.position);
    }

    public void PickUpBackend()
    {
        CreateEventInstance(spellPickUp);
        PlayOneShot(spellPickUp, this.transform.position);
    }

    public void PlayClosePopUp()
    {
        CreateEventInstance(closePopUp);
        PlayOneShot(closePopUp, this.transform.position);
    }

    public void DropBackend()
    {
        CreateEventInstance(spellDrop);
        PlayOneShot(spellDrop, this.transform.position);
    }

    public void UIPickUp(Transform target)
    {
        var instance = RuntimeManager.CreateInstance(spellPickUp);

        RuntimeManager.AttachInstanceToGameObject(
            instance,
            target,
            target.GetComponent<Rigidbody>()
        );

        instance.start();
        instance.release();
    }

    public void UIDrop(Transform target)
    {
        var instance = RuntimeManager.CreateInstance(spellDrop);

        RuntimeManager.AttachInstanceToGameObject(
            instance,
            target,
            target.GetComponent<Rigidbody>()
        );

        instance.start();
        instance.release();
    }
    #endregion UI_FUNCTIONS
}