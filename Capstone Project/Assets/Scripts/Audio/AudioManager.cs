/*************************************************
Author Names : 		Cade Naylor
Date Created : 		10/24/2025
Date Last Modified : 10/24/2025
Brief Description : Basic AudioManager for FMOD
External Resources : 	
	***************************************************/
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
public class AudioManager : MonoBehaviour
{
    #region Variables
    public static AudioManager instance { get; private set; }

        /*[Range(0, 1)]
        public float masterVolume;
        [Range(0, 1)]
        public float sfxVolume;
        [Range(0, 1)]
        public float musicVolume;*/

    private List<EventInstance> eventInstances = new List<EventInstance>();

    #endregion

    #region Loop Help

    // Looping Sounds

    /* When creating looping sounds, you should create an event instance like the line below: 
     * playerFootstep = AudioManager.instance.CreateEventInstance(FMODEvents.instance.playerFootsteps)
     * 
     * 
     * To start looping audio that there should only be one instance of at a time, try this: 
     *      PLAYBACK_STATE playbackstate;
            playerFootstep.getPlaybackState(out playbackstate);
            if (playbackstate.Equals(PLAYBACK_STATE.STOPPED))
            {
                playerFootstep.start();
            }
     * It will only run if playerFootstep is not currently playing, ensuring audio does not get duplicated
     *  
     *  
     * To stop looping sounds, try this:
     * playerFootstep.stop(STOP_MODE.IMMEDIATE);
     * You can play with the stopmode to get things to stop at different times
     */



    #endregion

    #region Audio Setup

    /// <summary>
    /// Called on the first frame update
    /// Ensures this is a singleton
    /// </summary>
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("There is more than one AudioManager in the scene");
        }
        instance = this;

        /*masterBus = RuntimeManager.GetBus("bus:/");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        bgmBus = RuntimeManager.GetBus("bus:/BGM");*/
    }


    /// <summary>
    /// Plays a one shot audio clip at a point
    /// </summary>
    /// <param name="sound"></param>
    /// <param name="worldPos"></param>
    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    /// <summary>
    /// Creares event instances for each sound
    /// </summary>
    /// <param name="eventRef">The event reference</param>
    /// <returns>The created event instance</returns>
    public EventInstance CreateEventInstance(EventReference eventRef)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    /// <summary>
    /// Deaallocates references for events on destroy
    /// </summary>
    private void OnDestroy()
    {
        foreach (EventInstance ei in eventInstances)
        {
            ei.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            ei.release();
        }
    }

    #endregion

    #region Sound Functions
    /// <summary>
    /// Plays the appropriate Electricity Spell SFX at 0,0
    /// </summary>
    /// <param name="number">Spell index</param>
    public void PlayElectricitySpell(int number)
    {
        //PlayOneShot(FMODEventsManager.instance.TimerWarnings[personIndex], Vector3.zero);
    }

    /// <summary>
    /// Plays the appropriate Electricity Spell SFX at a specified location
    /// </summary>
    /// <param name="number">Spell Index</param>
    /// <param name="worldPos">Where the sound gets played</param>
    public void PlayElectricitySpell(int number, Vector3 worldPos)
    {
        // this is the pattern you would follow
        //PlayOneShot(FMODEventsManager.instance.TimerWarnings[personIndex], worldPos);
    }

    #endregion
}
