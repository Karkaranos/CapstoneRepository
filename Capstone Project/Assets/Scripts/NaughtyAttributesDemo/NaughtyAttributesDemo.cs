using UnityEngine;


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
