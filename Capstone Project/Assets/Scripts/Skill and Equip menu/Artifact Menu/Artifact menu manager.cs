/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		10/28/2025
Date Last Modified : 10/28/2025
Brief Description : Manages the artifact equipping menu
External Resources : 	
	***************************************************/

using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactMenuManager : MonoBehaviour
{
    #region VARS
    private enum Settings
    {
        Debug,
        Refs,
        None
    }

    [SerializeField] private Settings ShownSettings;

    #region DEBUG
    [HorizontalLine(4, EColor.Red)]

    [SerializeField, ShowIf(nameof(ShownSettings), Settings.Debug)] private List<ArtifactData> TEMPListOfInventoryArtifacts;
    [SerializeField, ShowIf(nameof(ShownSettings), Settings.Debug)] private List<ArtifactData> TEMPListOfEquippedArtifacts;

    #endregion DEBUG
    #region REFS
    [HorizontalLine(4, EColor.Indigo)]

    [SerializeField, ShowIf(nameof(ShownSettings), Settings.Refs)] private GameObject scrollBarContainer;
    [SerializeField, ShowIf(nameof(ShownSettings), Settings.Refs)] private GameObject InventoryButtonPrefab;

    #endregion

    private ArtifactData heldArtifact;

    #endregion VARS


    /// <summary>
    /// Initializes everything
    /// </summary>
    void Start()
    {
        PopulatePossibleEquippedArtifacts(); 
    }

    /// <summary>
    /// Populates the menu with all of the artifacts the player owns
    /// </summary>
    private void PopulatePossibleEquippedArtifacts()
    {
        foreach (ArtifactData a in TEMPListOfEquippedArtifacts)
        {

        }
    }

    /// <summary>
    /// Equips the held artifact in the right slot
    /// </summary>
    /// <param name="index"></param>
    public void EquipArtifact(int index)
    {

    }
}
