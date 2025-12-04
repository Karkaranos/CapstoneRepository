/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		09/28/2025
Date Last Modified : 11/24/2025
Brief Description : This manages the player's skill points
                    and stores the data of their unlocked nodes
External Resources : 	
	***************************************************/

using NaughtyAttributes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    #region VARS
    private enum Settings
    {
        SkillSettings,
        Refs
    }

    
    [SerializeField] private Settings showingSettings;

    #region SKILL SETTINGS

    [HorizontalLine(4, EColor.Red)]
    //how many skill points the player has
    [ShowIf(nameof(showingSettings), Settings.SkillSettings)] public int ExperiencePoints;

    #endregion

    #region REFS

    [HorizontalLine(4, EColor.Indigo)]

    
    
    //prefab for the box that follows the cursor
    [SerializeField, ShowIf(nameof(showingSettings), Settings.Refs)] private GameObject FollowCursorPrefab;

    //this is all the text that changes
    [SerializeField, ShowIf(nameof(showingSettings), Settings.Refs)] private TMP_Text titleText;
    [SerializeField, ShowIf(nameof(showingSettings), Settings.Refs)] private TMP_Text costText;
    [SerializeField, ShowIf(nameof(showingSettings), Settings.Refs)] private TMP_Text descriptionText;
    [SerializeField, ShowIf(nameof(showingSettings), Settings.Refs)] private TMP_Text currentSkillPointsText;

    [SerializeField, ShowIf(nameof(showingSettings), Settings.Refs)] private bool isInEquipMenu;

    //list of containers for each skill tree element
    [SerializeField, ShowIf(nameof(showingSettings), Settings.Refs)] private List<GameObject> DifferentElementSkillTreeContainers;

    #endregion
    #region NONINSPECTOR VARS

    //master list of runes the player has unlocked
    [HideInInspector]
    public List<RuneData> unlockedRunes;

    //whatever runedata the player is currently holding to equip
   public RuneData currentlySelected;

    //holds a ref to the box it spawns when trying to equip
    private GameObject spawnedCursorFollowBox;

    private SkillAndArtifactManager skillAndArtifactManager;

    private SkillTreeNode nodeSelected;

    #endregion
    #endregion

    /// <summary>
    /// Initializes refs
    /// </summary>
    private void Start()
    {
        skillAndArtifactManager = FindFirstObjectByType<SkillAndArtifactManager>();
    }

    /// <summary>
    /// subscribes to needed public events
    /// </summary>
    private void OnEnable()
    {
        PublicEvents.TrashHeldOOCObject += ConfirmEquipSpell;
    }

    /// <summary>
    /// unsubscribes from public events
    /// </summary>
    private void OnDisable()
    {
        PublicEvents.TrashHeldOOCObject -= ConfirmEquipSpell;
    }


    //add in the data the node is storing as a parameter here
    //so we can store all the nodes the player's unlocked in a list
    /// <summary>
    /// Checks to see if the node can be unlocked and unlocks it if it can be
    /// will also store the data when we create the class for the data
    /// </summary>
    /// <param name="cost"></param>
    /// <returns></returns>
    public bool CanPurchaseNode(int cost)
    {
        //checks to see if you can purchase the node
        if (cost <= ExperiencePoints)
        {
            //purchases it
            ExperiencePoints -= cost;

            //updates the skill points text to the new value
            currentSkillPointsText.text = ExperiencePoints + " EXP";

            //tells the node to buy itself
            return true;
        }

        //tells the node you cant buy it
        return false;
    }

    /// <summary>
    /// This is where we'll put the saving of the data in the 
    /// skill tree node. Currently blank because character team
    /// has not talked about how we are doing this
    /// </summary>
    public void UpdatePurchasedNodes(RuneData runePurchased)
    {
        //Makes sure that theres only ever one copy of each rune data in the list
        if (!unlockedRunes.Contains(runePurchased))
        {
            unlockedRunes.Add(runePurchased);
            skillAndArtifactManager.UnlockedRunes.Add(runePurchased);
            if (runePurchased.NumberOnSkillTree == 4)
            {
                PublicEvents.MasteryRunePurchased?.Invoke(runePurchased.TypeOfRune);
            }
        }
        else
        {
            //idk what would trigger this but its important to have just in case
            //maybe messing around in the inspector?
            throw new System.Exception("Player already owns the rune " + runePurchased.RuneName + " but tried to add it to the runes purchased");
        }
    }

    /// <summary>
    /// Changes what skill tree is shown 
    /// </summary>
    /// <param name="TypeToChangeTo"> which skill tree to show, is indexed </param>
    public void ChangeRuneType(int TypeToChangeTo)
    {
        //goes through to find the element to change into
        for (int i = 0; i < DifferentElementSkillTreeContainers.Count; i++)
        {
            //change into it
            if (i == TypeToChangeTo)
            {
                DifferentElementSkillTreeContainers[i].SetActive(true);
            }
            else
            {
                //if its not the right one and this is the one currently showing, hide it
                if (DifferentElementSkillTreeContainers[i].activeInHierarchy)
                {
                    DifferentElementSkillTreeContainers[i].SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Holds the data the player is currently trying to equip
    /// </summary>
    /// <param name="data"></param>
    public void SelectNode(RuneData data)
    {
        currentlySelected = data;
        //Debug.Log("now currently holding " + currentlySelected.name);

        skillAndArtifactManager.SpawnCursorBox();
    }

    /// <summary>
    /// When the player equips it, it sets the held data to null and deletes the box
    /// </summary>
    public void ConfirmEquipSpell()
    {
        currentlySelected = null;

        skillAndArtifactManager.DeleteCursorBox();
    }

    /// <summary>
    /// Updates the text in the scene
    /// </summary>
    /// <param name="dataToDisplay"> the data to display now, whatever rune should be shown </param>
    /// <param name="cost"> how much the rune costs to purchase, -1 if already owned </param>
    public void UpdateSpellDescriptionText(SkillTreeNode dataToDisplay, int cost)
    {
        
        //shows cost if it has one
        if (cost > 0)
        {
            if (isInEquipMenu)
            {
                costText.text = "Unowned";
            }
            else
            {
                costText.text = cost + " EXP";
            }
                
        }
        else
        {
            costText.text = "Owned";
        }

        //updates the rest of the text
        titleText.text = dataToDisplay.NodeRuneData.RuneName;
        descriptionText.text = dataToDisplay.NodeRuneData.RuneDescription;

        nodeSelected = dataToDisplay;
    }

    /// <summary>
    /// tells the node to purchase itself
    /// </summary>
    public void PurchaseSpell()
    {
        if (nodeSelected != null)
        {
            nodeSelected.PurchaseNode();
        }
        
    }
}
