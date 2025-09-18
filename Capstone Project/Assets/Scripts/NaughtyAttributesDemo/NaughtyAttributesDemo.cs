using UnityEngine;
using NaughtyAttributes;


public class NaughtyAttributesDemo : MonoBehaviour
{
    [System.Serializable] public enum Settings 
    {
        WeaponSettings,
        ArmorSettings,
        OtherSettings
    }

    

    [SerializeField] private Settings currentInspectorShowing;

    private bool test;


}
