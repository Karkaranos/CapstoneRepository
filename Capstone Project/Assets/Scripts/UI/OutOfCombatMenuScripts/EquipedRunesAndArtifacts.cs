using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Experimental;
using UnityEngine;

public class EquipedRunesAndArtifacts : MonoBehaviour
{
    public static RuneData[] runes = new RuneData[4];
    public static ArtifactData[] artifacts = new ArtifactData[3];

    public static void PrintSpellsAndArtifacts() {
        foreach (RuneData rune in runes) {
            if (rune != null)
            {
                print("Rune: " + rune.name);
            }
            else {
                print(null);
            }
        }
        foreach (ArtifactData artifact in artifacts)
        {
            if (artifact != null)
            {
                print("Artifact: " + artifact.name);
            }
            else {
                print(null);
            }
        }
    }

    public static void EquipArtifact(ArtifactData artifact, int slotNumber) {
        artifacts[slotNumber - 1] = artifact;
    }
    public static void EquipSpell(RuneData rune, int slotNumber)
    {
        runes[slotNumber - 1] = rune;
    }
    public static void UnequipArtifact(ArtifactData artifact, int slotNumber)
    {
        artifacts[slotNumber - 1] = artifact;
    }
    public static void UnequipSpell(RuneData rune, int slotNumber)
    {
        runes[slotNumber - 1] = null;
    }
}
