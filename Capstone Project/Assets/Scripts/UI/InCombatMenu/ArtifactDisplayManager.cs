using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.Unicode;

public class ArtifactDisplayManager : MonoBehaviour
{
    public GameObject[] artifactDisplays;

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
        EquipedRunesAndArtifacts.PrintSpellsAndArtifacts();

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
