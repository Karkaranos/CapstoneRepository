/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		10/2/2025
Date Last Modified : 10/6/2025
Brief Description : This manages the player's current 
                    equipped spells. Does not hold the 
                    final list of spells, however
External Resources : 	
	***************************************************/

using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquippedSpellNode : MonoBehaviour
{
    #region VARS
    
    //inspector settings enum
    private enum Settings
    {
        SpellSettings,
        References
    }

    //inspector settings var
    [SerializeField] private Settings currentlyShowingSettings;

    #region SPELL SETTINGS

    [HorizontalLine(4, EColor.Red)]

    //the spell thats equipped in this node
    [SerializeField, ShowIf(nameof(currentlyShowingSettings), Settings.SpellSettings), Expandable] 
    private RuneData heldSpell;

    #endregion

    #region REFERENCES

    [HorizontalLine(4, EColor.Indigo)]

    //the index of this node - same as the index in the list of spells in SkillAndEquipManager
    [SerializeField, ShowIf(nameof(currentlyShowingSettings), Settings.References)] 
    private int index;

    [SerializeField, ShowIf(nameof(currentlyShowingSettings), Settings.References)] private TMP_Text buttonText;

    [SerializeField, ShowIf(nameof(currentlyShowingSettings), Settings.References)] private Color EquippedColor;

    #endregion

    #region NONINSPECTOR

    //refs to needed managers in scene
    private SkillAndArtifactManager skillAndEquipManager;
    [SerializeField] private SkillTreeManager skillTreeManager;

    #endregion
    #endregion

    /// <summary>
    /// Instantiates everything needed
    /// </summary>
    private void Start()
    {
        //finds the managers
        skillAndEquipManager = FindFirstObjectByType<SkillAndArtifactManager>();
        skillTreeManager = FindFirstObjectByType<SkillTreeManager>();

        //populates this node with the equipped spell at this node's index
        //in the master list of spells
        if (skillAndEquipManager.equippedSpells[index] != null)
        {
            heldSpell = skillAndEquipManager.equippedSpells[index];
            GetComponent<Image>().color = EquippedColor;
        }
    }

    /// <summary>
    /// Triggers when the button is pressed
    /// either picks up the spell in the node or puts down the spell currently held
    /// </summary>
    public void OnClick()
    {
        //checks to see if the player is holding any spell
        if (skillTreeManager.currentlySelected == null)
        {

            //if this has nothing in it
            if (heldSpell != null)
            {
                //sends the spell the player is holding to this node's spell
                skillTreeManager.SelectNode(heldSpell);
                heldSpell = null;
                skillAndEquipManager.SetIndexOfEquippedSpells(index, null);
                //GetComponent<Image>().color = Color.white;
                //buttonText.text = "Spell Slot";
            }
            else
            {
                return;
            }
        }
        else
        {
            if (heldSpell != null)
            {
                //if this has a spell, it swaps the spell this has with the spell you're holding
                RuneData temp = skillTreeManager.currentlySelected;
                skillTreeManager.SelectNode(heldSpell);
                heldSpell = temp;
                skillAndEquipManager.SetIndexOfEquippedSpells(index, heldSpell);
                //GetComponent<Image>().color = EquippedColor;
                //buttonText.text = heldSpell.RuneName;
            }
            else
            {
                //if this doesn't have a spell, sets the spell in this to the spell you're holding
                heldSpell = skillTreeManager.currentlySelected;
                skillAndEquipManager.SetIndexOfEquippedSpells(index, heldSpell);
                skillTreeManager.ConfirmEquipSpell();
                //GetComponent<Image>().color = EquippedColor;
                //buttonText.text = heldSpell.RuneName;
            }
            
        }
    }

    /// <summary>
    /// triggers when this element is hovered over
    /// tells the description to update
    /// </summary>
    public void OnHover()
    {
        if (heldSpell != null)
        {
            //tells the skill tree manager to update the text
            //skillTreeManager.UpdateSpellDescriptionText(heldSpell, -1);
        }
    }
}
