using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileDisplayBehavior : MonoBehaviour
{
    public Image[] artifactDisplays;
    public TextMeshProUGUI rangeText;
    public Slider healthBar;
    public PipDisplayBehavior pipDisplay;

    private GameManager gameManager;

    /// <summary>
    /// it shows the artifacts that are equipped
    /// </summary>
    public void SetUpArtifactDisplay(){

        //setting up the range text
        PlayerBehavior playerScript = FindFirstObjectByType<PlayerBehavior>();
        rangeText.text = "" + playerScript.movementLeft;

        //setting up the artifact displays
        foreach(Image artifact in artifactDisplays) {
            artifact.gameObject.SetActive(false);
        }
        for (int i = 0; i < ArtifactManager.CurrentArtifacts.Count; i++) {
            artifactDisplays[i].gameObject.SetActive(true);
            artifactDisplays[i].sprite = ArtifactManager.CurrentArtifacts[i].ArtifactSprite;
        }
    }

    /// <summary>
    /// should really be called in an event but this works too, if it aint brok dont fix it or somthing like that.
    /// </summary>
    private void Update()
    {
        SetUpArtifactDisplay();
    }

}
