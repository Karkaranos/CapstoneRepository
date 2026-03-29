/*************************************************
Author Names : 	Tyler Bouchard
Date Created : 	3/3/2026
Date Last Modified : 3/32026
Brief Description : this MASSIVE srcipt exists so that the spell description box knows what spell a 
spell slot has when its hovering over it.	
	***************************************************/
using UnityEngine;
using UnityEngine.UI;

public class InCombatSpellSlotBehavior : MonoBehaviour
{
    public RuneData rune;

    private GameManager gm;
    private RuneEvents runeEvents;
    private ButtonManager buttonManager;

    private void Start()
    {

        gm = FindFirstObjectByType<GameManager>();
        runeEvents = FindFirstObjectByType<RuneEvents>();
        buttonManager = FindFirstObjectByType<ButtonManager>();

    }

    //im sorry
    void Update()
    {
        
        if(gm.CurrentActionPoints < rune.RuneActionPoints || runeEvents.Casting || buttonManager.Moving || runeEvents.PreviousPos.Count != 0)
        {

            gameObject.GetComponent<Image>().color = Color.gray;
            gameObject.GetComponent<Button>().interactable = false;

        }
        else
        {

            gameObject.GetComponent<Image>().color = Color.white;
            gameObject.GetComponent<Button>().interactable = true;

        }

    }

}
