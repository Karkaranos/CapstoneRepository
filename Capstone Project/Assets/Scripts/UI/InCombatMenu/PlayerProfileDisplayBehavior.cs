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

    public void SetUpArtifactDisplay(){

        foreach(Image artifact in artifactDisplays) {
            artifact.gameObject.SetActive(false);
        }
        for (int i = 0; i < ArtifactManager.CurrentArtifacts.Count; i++) {
            artifactDisplays[i].gameObject.SetActive(true);
            artifactDisplays[i].sprite = ArtifactManager.CurrentArtifacts[i].ArtifactSprite;
        }
    }

    private void Update()
    {
        SetUpArtifactDisplay();
    }

}
