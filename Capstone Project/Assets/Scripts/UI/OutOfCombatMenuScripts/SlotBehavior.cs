/*************************************************
Author Names : 		Tyler Bouchard
Date Created : 		2/2/2026
Date Last Modified : 2/10/2026
Brief Description : is put on a spell or artifact slot, will store the runeData or 
artifactData depending on which slot type it is
***************************************************/
using UnityEngine;

public class SlotBehavior : MonoBehaviour
{
    private enum SlotType { 
        artifact,
        spell
    }
    
    [HideInInspector] public RuneData rune;
    [HideInInspector] public ArtifactData artifact;
    
    [SerializeField] private SlotType slotType;

    public SpellNodeBehavior heldSpellObject;
    public ArtifactNodeBehavior heldArtifactObject;

    [SerializeField] public int slotNumber;

    
    /// <summary>
    /// this retuens true if this is an artifact slot
    /// </summary>
    /// <returns></returns>
    public bool isArtifactSlot() {
        if (slotType == SlotType.artifact) { 
            return true;
        }
        return false;
    }

    /// <summary>
    /// this returns true if its a spell slot
    /// </summary>
    /// <returns></returns>
    public bool isSpellSlot()
    {
        if (slotType == SlotType.spell)
        {
            return true;
        }
        return false;
    }
}
