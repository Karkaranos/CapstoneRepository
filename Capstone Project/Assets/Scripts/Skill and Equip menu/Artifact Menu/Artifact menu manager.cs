/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		10/28/2025
Date Last Modified : 10/28/2025
Brief Description : Manages the artifact equipping menu
External Resources : 	
	***************************************************/

using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactMenuManager : MonoBehaviour
{
    #region VARS
    private enum Settings
    {
        Refs,
        TextRefs,
        None
    }

    [SerializeField] private Settings ShownSettings;
    #region REFS
    [HorizontalLine(4, EColor.Indigo)]

    [SerializeField, ShowIf(nameof(ShownSettings), Settings.Refs)] private GameObject scrollBarContainer;
    [SerializeField, ShowIf(nameof(ShownSettings), Settings.Refs)] private GameObject InventoryButtonPrefab;

    #endregion

    #region TEXTREFS
    [HorizontalLine(4, EColor.Blue)]

    [SerializeField, ShowIf(nameof(ShownSettings), Settings.TextRefs)] private TMP_Text artifactNameText;
    [SerializeField, ShowIf(nameof(ShownSettings), Settings.TextRefs)] private TMP_Text artifactWeightText;
    [SerializeField, ShowIf(nameof(ShownSettings), Settings.TextRefs)] private TMP_Text artifactDescriptionText;
    #endregion

    private ArtifactData heldArtifact;

    private ArtifactManager AM;

    #endregion VARS


    /// <summary>
    /// Initializes everything
    /// </summary>
    void Start()
    {
        AM = FindFirstObjectByType<GameManager>().ArtifactManager;
        StartCoroutine(DelayedPopulate());
    }

    private IEnumerator DelayedPopulate()
    {
        yield return null;
        PopulatePossibleEquippedArtifacts();
    }

    /// <summary>
    /// Populates the menu with all of the artifacts the player owns
    /// </summary>
    private void PopulatePossibleEquippedArtifacts()
    {
        foreach (ArtifactData a in ArtifactManager.inventoryArtifacts)
        {
            InventoryButton temp = Instantiate(InventoryButtonPrefab, scrollBarContainer.transform).GetComponent<InventoryButton>();
            temp.SetArtifactData(a);
            temp.InsVars();
        }

        //LayoutRebuilder.ForceRebuildLayoutImmediate(scrollBarContainer.GetComponentInParent<ScrollRect>().content);
    }

    /// <summary>
    /// Equips the held artifact in the right slot
    /// </summary>
    /// <param name="index"></param>
    public void EquipArtifact(int index)
    {

    }

    public void ArtifactPickedUp(ArtifactData data, bool isInInventory)
    {
        heldArtifact = data;
    }

    public void ButtonHovered(ArtifactData data)
    {
        artifactNameText.text = data.Name;
        artifactWeightText.text = data.ArtifactSize + " Slots";
        artifactDescriptionText.text = data.Description;
    }

}
