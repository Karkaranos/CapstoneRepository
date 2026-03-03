/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 3/3/2026
 * Last Modified: 3/3/2026
 * Brief: Executes code when the video is done playing
 * External Resources: https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Video.VideoPlayer.html Functions and ideas come from here
 * ***************************************************************************/
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer vp;
    private ButtonManager bm;

    /// <summary>
    /// Gets a reference to the transition manager, and sets the function for when the video ends
    /// </summary>
    void Start()
    {
        bm = FindFirstObjectByType<ButtonManager>();

        vp.loopPointReached += VideoEnd;
    }

    private void VideoEnd(VideoPlayer source)
    {
        if(bm == null)
        {
            bm = FindFirstObjectByType<ButtonManager>();
        }

        bm.CutsceneEnd();
    }
}
