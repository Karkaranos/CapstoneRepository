using UnityEngine;

public class SlotBehavior : MonoBehaviour
{
    private enum SlotType { 
        artifact,
        spell
    }
    public RuneData rune;
    public ArtifactData artifact;
    [SerializeField] private SlotType slotType;
    
    public bool isArtifactSlot() {
        if (slotType == SlotType.artifact) { 
            return true;
        }
        return false;
    }
    
    public bool isSpellSlot()
    {
        if (slotType == SlotType.spell)
        {
            return true;
        }
        return false;
    }
}
