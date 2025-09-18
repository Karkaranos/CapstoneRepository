using NaughtyAttributes;

using System.Collections.Generic;
using UnityEngine;


public class NaughtyAttributesDemo : MonoBehaviour
{
    [System.Serializable] public enum Settings 
    {
        WeaponSettings,
        PlayerSettings,
        OtherSettings
    }

    [SerializeField] private Settings currentInspectorShowing;

    [Space(4)]

    [ShowIf(nameof(currentInspectorShowing), Settings.WeaponSettings), SerializeField]
    private bool weaponPower;

    [Space(4)]

    [ShowIf(nameof(currentInspectorShowing), Settings.PlayerSettings), SerializeField]
    private float playerSpeed;

    [Space(4)]

    [ShowIf(nameof(currentInspectorShowing), Settings.PlayerSettings), SerializeField, MinMaxSlider(0, 100)]
    private Vector2Int playerMaxHealth;

    [Space(4)]

    [ShowIf(nameof(currentInspectorShowing), Settings.OtherSettings), SerializeField, InfoBox("Do Not Mess With This Please", EInfoBoxType.Warning)]
    private string errorOutput;

    [Space(4)]
    [ShowIf(nameof(currentInspectorShowing), Settings.OtherSettings), SerializeField, Dropdown(nameof(stringVals))]
    private string animalName;



    [HorizontalLine(3, EColor.Green)]
    [SerializeField, Tag] private string TagToLookFor;


    [Foldout("GeneralSettings"), SerializeField] private string objName;
    [Foldout("GeneralSettings"), SerializeField] private string objDiscription;


    private List<string> stringVals = new List<string>() { "dog", "cat", "hawk" };


}
