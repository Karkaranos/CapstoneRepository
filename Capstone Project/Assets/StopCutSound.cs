using FMOD.Studio;
using FMODUnity;
using EventReference = FMODUnity.EventReference;
using System.Collections.Generic;
using UnityEngine;
using static SkillTreeNode;
using UnityEngine.Playables;
public class StopCutSound : MonoBehaviour
{
    [SerializeField] private PlayableDirector timelineMainMenu;
    [SerializeField] private FMOD.Studio.Bus _masterBus;
    public void StopSounds()
    {
        timelineMainMenu.Stop();
        _masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}

