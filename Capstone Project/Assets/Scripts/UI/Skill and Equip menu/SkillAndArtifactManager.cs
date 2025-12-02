/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		10/2/2025
Date Last Modified : 11/24/2025
Brief Description : This manages the player's current 
                    equipped spells. Does not hold the 
                    final list of spells, however
External Resources : 	
	***************************************************/

using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class SkillAndArtifactManager : MonoBehaviour
{
    #region VARS
    private enum Settings
    {
        References,
        TestingAndDebug
    }

    [SerializeField] private Settings InspectorSettings;

    [SerializeField] private EventReference ambienceEventRefSFX;
    [SerializeField] private GameObject audioListenerObject;

    #region REFS

    [HorizontalLine(4, EColor.Red)]

    //all the containers for the different menus
    [ShowIf(nameof(InspectorSettings), Settings.References), SerializeField] private GameObject SkillTreeContainer;
    [ShowIf(nameof(InspectorSettings), Settings.References), SerializeField] private GameObject EquipMenuContainer;
    [ShowIf(nameof(InspectorSettings), Settings.References), SerializeField]
    private GameObject OutOfCombatMenuContainer;

    [HorizontalLine(4, EColor.Indigo)]

    [ShowIf(nameof(InspectorSettings), Settings.References), SerializeField] private List<GameObject> MarkContainers;
    [ShowIf(nameof(InspectorSettings), Settings.References), SerializeField] private List<GameObject> PageContainers;
    [ShowIf(nameof(InspectorSettings), Settings.References), SerializeField] private List<GameObject> SpellTabs;
    [ShowIf(nameof(InspectorSettings), Settings.References), SerializeField]
    private GameObject SpellDescription;
    [ShowIf(nameof(InspectorSettings), Settings.References), SerializeField] private GameObject PrevNextButtons;



    [SerializeField, ShowIf(nameof(InspectorSettings), Settings.References)]
    private Button continueButton;

    [SerializeField, ShowIf(nameof(InspectorSettings), Settings.References)]
    private GameObject cursorBoxPrefab;

    #endregion

    #region TESTING AND DEBUG

    //number of spell slots the player has
    [ShowIf(nameof(InspectorSettings), Settings.TestingAndDebug), SerializeField] public int NumOfSpellSlots;

    //master list of all equipped spells the player has
    [ShowIf(nameof(InspectorSettings), Settings.TestingAndDebug), SerializeField, Expandable] public List<RuneData> equippedSpells;
    [HideInInspector] public List<RuneData> UnlockedRunes;

    #endregion

    
    private GameObject spawnedCursorBox;

    #endregion

    /// <summary>
    /// Updates whether or not the player can move on from the menu
    /// </summary>
    /// <returns></returns>
    private bool UpdateContinueButton()
    {
        foreach (RuneData d in equippedSpells)
        {
            if (d != null)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// sets up the number of spell slots
    /// </summary>
    private void OnEnable()
    {
        //sets the spell menu active
        SkillTreeContainer.SetActive(false);
        EquipMenuContainer.SetActive(true);

        //sets the spell slots
        if (equippedSpells.Count < NumOfSpellSlots)
        {
            for (int i = equippedSpells.Count; i < NumOfSpellSlots; i++)
            {
                equippedSpells.Add(null);
            }
        }
    }

    /// <summary>
    /// swaps the menus between the spell and equip menu
    /// </summary>
    /// <param name="isSkill"> if youre turning on the spell menu </param>
    public void SwapMenus(bool isSkill)
    {
        if (isSkill)
        {
            SkillTreeContainer.SetActive(true);
            EquipMenuContainer.SetActive(false);
        }
        else
        {
            SkillTreeContainer.SetActive(false);
            EquipMenuContainer.SetActive(true);
        }

        if (spawnedCursorBox != null)
        {
            Destroy(spawnedCursorBox);
            spawnedCursorBox = null;
        }
    }

    /// <summary>
    /// moves on to the next level
    /// </summary>
    public void ContinueToNextLevel()
    {
        OutOfCombatMenuContainer.SetActive(false);
        FindFirstObjectByType<RuneEvents>().gameObject.SetActive(false);

        PublicEvents.StartBattle.Invoke();
        //GameObject.Find("Move Confirmation").SetActive(false);

        AudioManager.instance.CreateEventInstance(ambienceEventRefSFX);
        AudioManager.instance.PlayOneShot(ambienceEventRefSFX, audioListenerObject.transform.position);
    }

    /// <summary>
    /// sets the given spell to the equipped master list
    /// </summary>
    /// <param name="index"> index to set </param>
    /// <param name="data"> data to set to that index </param>
    public void SetIndexOfEquippedSpells(int index, RuneData data)
    {
        equippedSpells[index] = data;
        PublicEvents.EquipRunesToCombatMenu(index);
        continueButton.interactable = UpdateContinueButton();
    }
    
    /// <summary>
    /// gets the spell from the given index
    /// </summary>
    /// <param name="index"> index to get from </param>
    /// <returns></returns>
    public RuneData GetIndexOfEquippedSpells(int index)
    {
        return equippedSpells[index];
    }

    /// <summary>
    /// controls the temporary box to follow the cursor
    /// </summary>
    public void SpawnCursorBox()
    {
        if (spawnedCursorBox == null)
        {
            spawnedCursorBox = Instantiate(cursorBoxPrefab, transform);
        }
    }

    /// <summary>
    /// deletes the box
    /// </summary>
    public void DeleteCursorBox()
    {
        if (spawnedCursorBox != null)
        {
            Destroy(spawnedCursorBox);
            spawnedCursorBox = null;
        }
    }


    #region CanvasButtonFuncs

    /// <summary>
    /// Switches the tab that is showing 
    /// </summary>
    /// <param name="tabToShow"> the tab to switch over to </param>
    public void SwitchShownTab(int tabToShow)
    {
        foreach (GameObject obj in PageContainers)
        {
            obj.SetActive(false);
        }
        
        //hide the prev and next buttons when its the skill tree
        if (tabToShow == 0 || tabToShow == 1)
        {
            SpellDescription.SetActive(false);

            if (tabToShow == 0)
            {
                PrevNextButtons.SetActive(false);
            }
            else
            {
                PrevNextButtons.SetActive(true);
            }
        }
        else
        {
            SpellDescription.SetActive(true);
            PrevNextButtons.SetActive(true);
        }

        

        PageContainers[tabToShow].SetActive(true);

        PublicEvents.TrashHeldOOCObject?.Invoke();
    }

    /// <summary>
    /// Switches the mark that is showing
    /// </summary>
    /// <param name="pageToShow"></param>
    public void ShowMark(int pageToShow)
    {
        foreach (GameObject obj in MarkContainers)
        {
            obj.SetActive(false);
        }

        MarkContainers[pageToShow].SetActive(true);

        PublicEvents.TrashHeldOOCObject?.Invoke();
    }

    /// <summary>
    /// Turns on / off the spell tabs
    /// </summary>
    public void ToggleSpellTabs()
    {
        foreach (GameObject obj in SpellTabs)
        {
            if (obj.activeInHierarchy)
            {
                obj.SetActive(false);
            }
            else
            {
                obj.SetActive(true);
            }
               
        }
    }

    /// <summary>
    /// Goes over to the next page
    /// </summary>
    public void NextPage()
    {
        bool markIsShowing = false;
        int shownIndex = -1;

        foreach (GameObject obj in MarkContainers)
        {
            if (obj.activeInHierarchy)
            {
                markIsShowing = true;
                shownIndex = MarkContainers.IndexOf(obj);
            }
        }

        if (markIsShowing)
        {
            if (shownIndex + 1 >= MarkContainers.Count)
            {
                ShowMark(0);
            }
            else
            {
                ShowMark(shownIndex + 1);
            }
        }
        else
        {
            foreach (GameObject obj in PageContainers)
            {
                if (obj.activeInHierarchy)
                {
                    shownIndex = PageContainers.IndexOf(obj);
                }
            }

            //0 is skill tree and 1 is artifacts
            if (shownIndex != 0 && shownIndex != 1)
            {
                if (shownIndex + 1 >= PageContainers.Count)
                {
                    //2 will be the first spell
                    SwitchShownTab(2);
                }
                else
                {
                    SwitchShownTab(shownIndex + 1);
                }
            }
        }
    }

    /// <summary>
    /// goes back to the previous page
    /// </summary>
    public void PreviousPage()
    {
        bool markIsShowing = false;
        int shownIndex = -1;

        foreach (GameObject obj in MarkContainers)
        {
            if (obj.activeInHierarchy)
            {
                markIsShowing = true;
                shownIndex = MarkContainers.IndexOf(obj);
            }
        }

        if (markIsShowing)
        {
            if (shownIndex - 1 < 0)
            {
                ShowMark(MarkContainers.Count - 1);
            }
            else
            {
                ShowMark(shownIndex - 1);
            }
        }
        else
        {
            foreach (GameObject obj in PageContainers)
            {
                if (obj.activeInHierarchy)
                {
                    shownIndex = PageContainers.IndexOf(obj);
                }
            }

            //0 is skill tree and 1 is artifacts
            if (shownIndex != 0 && shownIndex != 1)
            {
                //2 is the replacement for 0 in the tabs
                if (shownIndex - 1 < 2)
                {
                    //2 will be the first spell
                    SwitchShownTab(PageContainers.Count - 1);
                }
                else
                {
                    SwitchShownTab(shownIndex - 1);
                }
            }
        }
    }

    #endregion
}
