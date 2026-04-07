using UnityEngine;

public class EquipedRunesAndArtifacts : MonoBehaviour
{
    public static RuneData[] runes = new RuneData[4];
    public static ArtifactData[] artifacts = new ArtifactData[3];

   
    public static void EquipArtifact(ArtifactData artifact, int slotNumber) {
        artifacts[slotNumber - 1] = artifact;
    }
    public static void EquipSpell(RuneData rune, int slotNumber)
    {
        runes[slotNumber - 1] = rune;
    }
    public static void UnequipArtifact(int slotNumber)
    {
        artifacts[slotNumber - 1] = null;
    }
    public static void UnequipSpell(int slotNumber)
    {
        runes[slotNumber - 1] = null;
    }
}
