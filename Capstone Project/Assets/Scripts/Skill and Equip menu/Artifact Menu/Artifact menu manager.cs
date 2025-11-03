/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		10/28/2025
Date Last Modified : 11/02/2025
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

    //refs to objects in scene
    [SerializeField, ShowIf(nameof(ShownSettings), Settings.Refs)] private GameObject scrollBarContainer;
    [SerializeField, ShowIf(nameof(ShownSettings), Settings.Refs)] private GameObject InventoryButtonPrefab;
    [SerializeField, ShowIf(nameof(ShownSettings), Settings.Refs)] private List<EquippedArtifactButton> artifactEquippedSlotButtons;

    #endregion

    #region TEXTREFS
    [HorizontalLine(4, EColor.Blue)]

    //refs to the text objects in scene
    [SerializeField, ShowIf(nameof(ShownSettings), Settings.TextRefs)] private TMP_Text artifactNameText;
    [SerializeField, ShowIf(nameof(ShownSettings), Settings.TextRefs)] private TMP_Text artifactWeightText;
    [SerializeField, ShowIf(nameof(ShownSettings), Settings.TextRefs)] private TMP_Text artifactDescriptionText;
    #endregion

    //currently held artifact
    private ArtifactData heldArtifact;

    //list of all the buttons for the inventory
    private List<InventoryButton> inventoryButtons = new List<InventoryButton>();

    //the ref to the skillAndArtifactManager in scene
    private SkillAndArtifactManager skillArtifactManager;
    #endregion VARS

    #region Initialization
    /// <summary>
    /// Initializes everything, populates the inventory
    /// </summary>
    void Start()
    {
        skillArtifactManager = FindFirstObjectByType<SkillAndArtifactManager>();
        StartCoroutine(DelayedPopulate());
    }

    /// <summary>
    /// subscribes to the public events
    /// </summary>
    private void OnEnable()
    {
        PublicEvents.TrashHeldOOCObject += ArtifactDropped;
    }

    /// <summary>
    /// unsubscribes from the public events
    /// </summary>
    private void OnDisable()
    {
        PublicEvents.TrashHeldOOCObject -= ArtifactDropped;
    }
    #endregion

    #region PUBLIC FUNCS

    /// <summary>
    /// Equips the artifact to the held slot
    /// </summary>
    /// <param name="buttonPressed"> the button that was pressed </param>
    public void EquipArtifact(EquippedArtifactButton buttonPressed)
    {
        if (buttonPressed != null)
        {
            //if you want to pick up the object from the pressed button
            if (heldArtifact == null && buttonPressed.GetArtifactData() != null)
            {
                //grab the artifact
                heldArtifact = buttonPressed.GetArtifactData();

                //get rid of it in the artifact menu
                ArtifactManager.RemoveArtifact(heldArtifact);

                //button no longer holds an artifact
                buttonPressed.SetArtifactData(null);

                //update what buttons are showing
                UpdateNumberOfEquippedButtons();

                //spawns the box to show youre holding smthn
                skillArtifactManager.SpawnCursorBox();
            }
            else if (heldArtifact != null)
            {
                //if you're swapping artifacts
                if (buttonPressed.GetArtifactData() != null)
                {
                    //temporarily holds the data from the button
                    ArtifactData temp = buttonPressed.GetArtifactData();

                    //gets rid of the artifact from the equipped ones
                    ArtifactManager.RemoveArtifact(temp);

                    //if you can apply the new one (also just applys it if possible)
                    if (ArtifactManager.ApplyArtifact(heldArtifact))
                    {
                        //apply the new one
                        buttonPressed.SetArtifactData(heldArtifact);

                        //updates the shown equipped buttons
                        UpdateNumberOfEquippedButtons();

                        //holds the temp data
                        heldArtifact = temp;
                    }
                    else
                    {
                        //reapplys the artifact to the button
                        ArtifactManager.ApplyArtifact(temp);
                    }


                }
                else
                {
                    //tries to equip the artifact (also just applys it if possible)
                    if (ArtifactManager.ApplyArtifact(heldArtifact))
                    {
                        //sets the buttons artifact
                        buttonPressed.SetArtifactData(heldArtifact);

                        //updates the number of shown buttons
                        UpdateNumberOfEquippedButtons();

                        //stops holding the artifact
                        heldArtifact = null;
                        skillArtifactManager.DeleteCursorBox();


                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Handles picking up an artifact from the inventory
    /// </summary>
    /// <param name="data"> the artifact you picked up </param>
    public void ArtifactPickedUp(ArtifactData data)
    {
        //if you're holding something, throw it back into the inventory
        if (heldArtifact != null)
        {
            ArtifactManager.ObtainArtifact(heldArtifact);
            heldArtifact = null;
        }

        //hold the artifact
        heldArtifact = data;

        //get it out of the inventory
        ArtifactManager.RemoveArtifactFromInventory(data);

        //update the showing inventory buttons
        UpdateInventoryGameObjects();

        //spawn the box
        skillArtifactManager.SpawnCursorBox();
    }
   
    /// <summary>
    /// Updates all of the text descriptions when a button is hovered over
    /// currently broken for the inventory
    /// </summary>
    /// <param name="data"> the data to update the button for </param>
    public void ButtonHovered(ArtifactData data)
    {
        artifactNameText.text = data.Name;
        artifactWeightText.text = data.ArtifactSize + " Slots";
        artifactDescriptionText.text = data.Description;
    }

    #endregion

    #region HELPER FUNCS

    /// <summary>
    /// Delay the population of the inventory by a frame
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayedPopulate()
    {
        yield return null;
        UpdateInventoryGameObjects();
    }

    /// <summary>
    /// updates what inventory gameobjects are shown
    /// </summary>
    private void UpdateInventoryGameObjects()
    {
        //start by turning them all off then selectively turn them back on
        foreach (InventoryButton button in inventoryButtons)
        {
            button.gameObject.SetActive(false);
        }

        //turns on or creates a new button for each item in the inventory
        foreach (ArtifactData data in ArtifactManager.InventoryArtifacts)
        {
            //saves a check to see if they already have a button
            bool hasButton = false;

            //runs through all of the buttons to turn it on if its the button for the right data
            foreach (InventoryButton button in inventoryButtons)
            {
                if (button.GetArtifactData() == data)
                {
                    hasButton = true;
                    button.gameObject.SetActive(true);
                    break;
                }
            }

            //if it runs through all the buttons and doesnt have one, it makes a new button
            if (!hasButton)
            {
                CreateNewInventoryItem(data);
            }
        }
    }

    /// <summary>
    /// Makes a new button with the given data
    /// </summary>
    /// <param name="data"> the data to make the button for </param>
    private void CreateNewInventoryItem(ArtifactData data)
    {
        //makes the button
        InventoryButton temp = Instantiate(InventoryButtonPrefab, scrollBarContainer.transform).GetComponent<InventoryButton>();

        //adds it to the list
        inventoryButtons.Add(temp);

        //gives it the right data
        temp.SetArtifactData(data);

        //lets it get set up
        temp.InsVars();
    }

    /// <summary>
    /// handles whenever the player drops the artifact they're currently holding
    /// </summary>
    private void ArtifactDropped()
    {
        //adds the artifact to the inventory
        ArtifactManager.ObtainArtifact(heldArtifact);

        //deletes the object
        heldArtifact = null;
        skillArtifactManager.DeleteCursorBox();

        //updates the inventory
        UpdateInventoryGameObjects();
    }

    /// <summary>
    /// Updates all the equipped buttons to show/hide
    /// </summary>
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

        Debug.Log("Art manager has " + ArtifactManager.CurrentArtifactWeight + " weight, while currentlyEquippedButtons is " + currentlyEquippedButtons);

        //sets the right number of buttons to shown/hidden depending on the difference between
        //the number of buttons that have stuff to the number of available space
        if (ArtifactManager.CurrentArtifactWeight != currentlyEquippedButtons)
        {

            //the weight can only ever go over the current number of artifacts so we run this loop for each time it's over
            for (int i = 0; i < (ArtifactManager.CurrentArtifactWeight - currentlyEquippedButtons); i++)
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

    #endregion
}
