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
using Unity.VisualScripting;
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
    [SerializeField, ShowIf(nameof(ShownSettings), Settings.Refs)] private List<EquippedArtifactButton> artifactEquippedSlotButtons;

    #endregion

    #region TEXTREFS
    [HorizontalLine(4, EColor.Blue)]

    [SerializeField, ShowIf(nameof(ShownSettings), Settings.TextRefs)] private TMP_Text artifactNameText;
    [SerializeField, ShowIf(nameof(ShownSettings), Settings.TextRefs)] private TMP_Text artifactWeightText;
    [SerializeField, ShowIf(nameof(ShownSettings), Settings.TextRefs)] private TMP_Text artifactDescriptionText;
    #endregion

    private ArtifactData heldArtifact;


    private List<InventoryButton> inventoryButtons = new List<InventoryButton>();

    private ArtifactManager AM;
    private SkillAndArtifactManager skillArtifactManager;
    private int numberOfEquippedSlotsAvailable;

    #endregion VARS


    /// <summary>
    /// Initializes everything
    /// </summary>
    void Start()
    {
        AM = FindFirstObjectByType<GameManager>().ArtifactManager;
        skillArtifactManager = FindFirstObjectByType<SkillAndArtifactManager>();
        StartCoroutine(DelayedPopulate());
    }

    private void OnEnable()
    {
        PublicEvents.TrashHeldOOCObject += ArtifactDropped;
    }

    private void OnDisable()
    {
        PublicEvents.TrashHeldOOCObject -= ArtifactDropped;
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
            CreateNewInventoryItem(a);
        }
    }

    /// <summary>
    /// Equips the held artifact in the right slot
    /// </summary>
    /// <param name="index"></param>
    public void EquipArtifact(int index)
    {
        EquippedArtifactButton buttonThatHasBeenClicked = null;

        foreach (EquippedArtifactButton button in artifactEquippedSlotButtons)
        {
            if (button.index == index)
            {
                buttonThatHasBeenClicked = button;
            }
        }

        if (buttonThatHasBeenClicked != null)
        {
            if (heldArtifact == null && buttonThatHasBeenClicked.GetArtifactData() != null)
            {
                heldArtifact = buttonThatHasBeenClicked.GetArtifactData();
                buttonThatHasBeenClicked.SetArtifactData(null);

                if (heldArtifact.ArtifactSize > 1)
                {
                    int i = 1;
                    foreach (EquippedArtifactButton eButton in artifactEquippedSlotButtons)
                    {
                        if (eButton.GetArtifactData() == null && !eButton.gameObject.activeInHierarchy)
                        {
                            eButton.gameObject.SetActive(true);
                            i++;
                            if (i >= heldArtifact.ArtifactSize)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            skillArtifactManager.SpawnCursorBox();


        }
        if (heldArtifact != null)
        {
            if (buttonThatHasBeenClicked.GetArtifactData() != null)
            {
                ArtifactData temp = buttonThatHasBeenClicked.GetArtifactData();

                ArtifactManager.RemoveArtifact(temp);

                if (ArtifactManager.ApplyArtifact(heldArtifact))
                {
                    buttonThatHasBeenClicked.SetArtifactData(heldArtifact);

                    if (heldArtifact.ArtifactSize > 1)
                    {
                        int i = 1;
                        foreach (EquippedArtifactButton eButton in artifactEquippedSlotButtons)
                        {
                            if (eButton.GetArtifactData() == null)
                            {
                                eButton.gameObject.SetActive(false);
                                i++;
                                if (i >= heldArtifact.ArtifactSize)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    heldArtifact = temp;


                }
                else
                {
                    ArtifactManager.ApplyArtifact(temp);
                }


            }
            else
            {
                buttonThatHasBeenClicked.SetArtifactData(heldArtifact);
                heldArtifact = null;
                skillArtifactManager.DeleteCursorBox();
            }
        }
    }


    public void ArtifactPickedUp(ArtifactData data, bool isInInventory)
    {
        heldArtifact = data;

        if (isInInventory)
        {
            for (int i = 0; i < inventoryButtons.Count; i++)
                if (inventoryButtons[i].GetArtifactData() == data)
                {
                    Destroy(inventoryButtons[i].gameObject);
                    inventoryButtons.RemoveAt(i);
                    break;
                }

        }

        skillArtifactManager.SpawnCursorBox();

    }

    private void ArtifactDropped()
    {

    }

    public void ButtonHovered(ArtifactData data)
    {
        artifactNameText.text = data.Name;
        artifactWeightText.text = data.ArtifactSize + " Slots";
        artifactDescriptionText.text = data.Description;
    }

    private void CreateNewInventoryItem(ArtifactData data)
    {
        InventoryButton temp = Instantiate(InventoryButtonPrefab, scrollBarContainer.transform).GetComponent<InventoryButton>();
        inventoryButtons.Add(temp);
        temp.SetArtifactData(data);
        temp.InsVars();
    }
}
