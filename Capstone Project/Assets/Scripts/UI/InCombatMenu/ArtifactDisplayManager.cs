/*************************************************
Author Names : 		Tyler Bouchard
Date Created : 		2/10/2026
Date Last Modified : 2/19/2026
Brief Description : This gets put on the artifact display n the in combat menu
it shows the artifacts that are equiped
***************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactDisplayManager : MonoBehaviour
{
    public GameObject[] artifactDisplays;

    /// <summary>
    /// Sets up the artifact displays
    /// </summary>
    public void SetUpArtifactDisplays()
    {
        List<ArtifactData> artifacts = new List<ArtifactData>();
        
        //finds all the artifact datas that are equiped
        foreach (ArtifactData artifact in EquipedRunesAndArtifacts.artifacts) {
            if (artifact != null)
            {
                artifacts.Add(artifact);
            }
        }
        //EquipedRunesAndArtifacts.PrintSpellsAndArtifacts();

        //resets all of the spell tabs so there is no overlap in setup
        foreach (GameObject artifactDisplay in artifactDisplays)
        {
            artifactDisplay.SetActive(false);
        }

        //activates the artifact display
        for (int i = 0; i < artifacts.Count; i++) {
            artifactDisplays[i].SetActive(true);
            artifactDisplays[i].GetComponent<Image>().sprite = artifacts[i].ArtifactSprite;
        }
    }
}
