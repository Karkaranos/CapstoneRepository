using UnityEngine;

public class SlotBehavior : MonoBehaviour
{
    private enum SlotType { 
        artifact,
        spell
    }
    
    [SerializeField] private SlotType slotType;
}
