using UnityEngine;

public class SlotBehavior : MonoBehaviour
{
    private enum SlotType { 
        artifact,
        spell
    }
    public RuneData rune;
    
    [SerializeField] private SlotType slotType;
}
