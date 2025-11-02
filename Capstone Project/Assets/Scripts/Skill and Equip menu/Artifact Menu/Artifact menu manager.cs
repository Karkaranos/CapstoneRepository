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
        // PopulatePossibleEquippedArtifacts();

        UpdateInventoryGameObjects();
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
    public void EquipArtifact(EquippedArtifactButton buttonPressed)
    {
        if (buttonPressed != null)
        {
            if (heldArtifact == null && buttonPressed.GetArtifactData() != null)
            {
                heldArtifact = buttonPressed.GetArtifactData();

                ArtifactManager.RemoveArtifact(heldArtifact);
                buttonPressed.SetArtifactData(null);

                /*if (heldArtifact.ArtifactSize > 1)
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
                }*/

                UpdateNumberOfEquippedButtons();

                skillArtifactManager.SpawnCursorBox();
            }
            else if (heldArtifact != null)
            {
                if (buttonPressed.GetArtifactData() != null)
                {
                    ArtifactData temp = buttonPressed.GetArtifactData();

                    ArtifactManager.RemoveArtifact(temp);

                    if (ArtifactManager.ApplyArtifact(heldArtifact))
                    {
                        buttonPressed.SetArtifactData(heldArtifact);

                        /*if (heldArtifact.ArtifactSize > 1)
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
                        }*/

                        UpdateNumberOfEquippedButtons();

                        heldArtifact = temp;


                    }
                    else
                    {
                        ArtifactManager.ApplyArtifact(temp);
                    }


                }
                else
                {
                    if (ArtifactManager.ApplyArtifact(heldArtifact))
                    {
                        Debug.Log("Artifact applied");
                        buttonPressed.SetArtifactData(heldArtifact);

                        /*if (heldArtifact.ArtifactSize > 1)
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
                        }*/

                        UpdateNumberOfEquippedButtons();


                        heldArtifact = null;
                        skillArtifactManager.DeleteCursorBox();


                    }
                    else
                    {
                        Debug.Log("Artifact could not be applied");
                    }
                }

            }

        }
    }

    private void UpdateNumberOfEquippedButtons()
    {
        //all the buttons that have stuff in them
        int currentlyEquippedButtons = 0;
        foreach (EquippedArtifactButton button in artifactEquippedSlotButtons)
        {
            if (button.GetArtifactData() != null)
            {
                currentlyEquippedButtons++;
            }
        }

        Debug.Log("Art manager has " + ArtifactManager.currentArtifactWeight + " weight, while currentlyEquippedButtons is " + currentlyEquippedButtons);

        //sets the right number of buttons to shown/hidden depending on the difference between
        //the number of buttons that have stuff to the number of available space
        if (ArtifactManager.currentArtifactWeight != currentlyEquippedButtons)
        {
            
            //the weight can only ever go over the current number of artifacts so we run this loop for each time it's over
            for (int i = 0; i < (ArtifactManager.currentArtifactWeight - currentlyEquippedButtons); i++)
            {
                //bool breakOnlyTheForeach = false;
                //goes through all the buttons and turns off the first one that's empty
                foreach (EquippedArtifactButton button in artifactEquippedSlotButtons)
                {
                    if (button.GetArtifactData() == null && button.gameObject.activeInHierarchy)
                    {
                        button.gameObject.SetActive(false);
                        //breakOnlyTheForeach = true;
                        break;
                    }
                }
            }
        }
        else
        {
            //if there are no artifacts that take up more than one slot, everything should be on.
            foreach (EquippedArtifactButton button in artifactEquippedSlotButtons)
            {
                button.gameObject.SetActive(true);
            }
        }
    }

    public void ArtifactPickedUp(ArtifactData data)
    {
        if (heldArtifact != null)
        {
            ArtifactManager.ObtainArtifact(heldArtifact);
            heldArtifact = null;
        }

        heldArtifact = data;

        ArtifactManager.RemoveArtifactFromInventory(data);


        UpdateInventoryGameObjects();

        skillArtifactManager.SpawnCursorBox();

    }

    private void ArtifactDropped()
    {
        ArtifactManager.ObtainArtifact(heldArtifact);
        heldArtifact = null;
        skillArtifactManager.DeleteCursorBox();

        UpdateInventoryGameObjects();
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

    private void UpdateInventoryGameObjects()
    {
        foreach (InventoryButton button in inventoryButtons)
        {
            button.gameObject.SetActive(false);
        }

        foreach (ArtifactData data in ArtifactManager.inventoryArtifacts)
        {
            bool hasButton = false;

            foreach (InventoryButton button in inventoryButtons)
            {
                if (button.GetArtifactData() == data)
                {
                    hasButton = true;
                    button.gameObject.SetActive(true);
                    break;
                }
            }

            if (!hasButton)
            {
                CreateNewInventoryItem(data);
            }
        }


    }
}
