using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class EquipedRunesAndArtifacts : MonoBehaviour
{
    public static List<RuneData> runes = new List<RuneData>();
    public static List<ArtifactData> artifacts = new List<ArtifactData>();

    public static void PrintSpellsAndArtifacts() {
        print("CALLED");
        print(runes.Count +" "+ artifacts.Count);
        foreach (RuneData rune in runes) {
            print("Rune: " + rune.name);
        }
        foreach (ArtifactData artifact in artifacts)
        {
            print("Artifact: " + artifact.name);
        }
    }

    public static void EquipArtifact(ArtifactData artifact) {
        if (!(runes.Count >= 4)) {
            artifacts.Add(artifact);
        }
    }
    public static void EquipSpell(RuneData rune)
    {
        if (!(runes.Count >= 4))
        {
            runes.Add(rune);
        }
    }
    public static void UnequipArtifact(ArtifactData artifact)
    {
        if (artifacts.Contains(artifact)) { 
            artifacts.Remove(artifact);
        }
    }
    public static void UnequipSpell(RuneData rune)
    {
        if (runes.Contains(rune))
        {
            runes.Remove(rune);
        }
    }
}
