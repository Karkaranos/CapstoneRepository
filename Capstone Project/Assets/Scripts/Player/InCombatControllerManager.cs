/*************************************************
Author Names : 		Tyler Hayes
Date Created : 		3/3/2026
Date Last Modified : 3/3/2026 (Tyler Hayes)
Brief Description : handles the controller's connection to the in combat UI Menu
External Resources : 	
***************************************************/

using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class InCombatControllerManager : MonoBehaviour
{
    [SerializeField] private GameObject firstCombatUIButton;
    [SerializeField] private Vector2Int defaultGridSelectCoords;
    [SerializeField] private float delayBeforeSpellPopup;

    private GameObject lastSelectedButton;
    private Vector2Int lastSelectedGridTile;
    private bool controllerOnUIMenu;
    private bool PlayerIsInUIMenu;

    private bool isPlottingPath;
    private Coroutine hoverSpellCoroutine;

    //UNCOMMENT THIS WHEN YOU WANT TO ACTUALLY START TESTING
    /*private void OnEnable()
    {
        PublicEvents.ToggleGridView += ToggleBetweenGridAndUI;
        PublicEvents.ControllerMoveInGrid += MoveInGrid;
    }

    private void OnDisable()
    {
        PublicEvents.ToggleGridView -= ToggleBetweenGridAndUI;
        PublicEvents.ControllerMoveInGrid -= MoveInGrid;
    }*/

    /// <summary>
    /// triggers whenever the player goes back to the incombat menu
    /// this includes when the player is done moving and done casting a spell
    /// </summary>
    private void OpenUIMenu()
    {
        PlayerIsInUIMenu = true;
        lastSelectedButton = firstCombatUIButton;
        lastSelectedGridTile = defaultGridSelectCoords;

        EventSystem.current.SetSelectedGameObject(firstCombatUIButton);
    }

    /// <summary>
    /// Swaps between the player moving around in the grid and the UI
    /// </summary>
    private void ToggleBetweenGridAndUI()
    {
        //dont do this if you aren't in the default ui menu
        if (!PlayerIsInUIMenu)
        {
            return;
        }

        //if we are currently in the ui, go to grid
        if (controllerOnUIMenu)
        {
            lastSelectedButton = EventSystem.current.currentSelectedGameObject;
            controllerOnUIMenu = false;

            //swap over to grid and highlight it
        }
        else
        {
            //go back to ui

            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
            controllerOnUIMenu = true;

            //leave grid
        }
    }

    /// <summary>
    /// Allows the player to move around in the grid
    /// </summary>
    /// <param name="dir"> the direction the player moves in </param>
    private void MoveInGrid(Vector2 dir)
    {
        //if we aren't moving in grid, don't do this
        if (controllerOnUIMenu)
        {
            return;
        }

        //if we aren't in the default incombat menu, we don't do this
        if (!PlayerIsInUIMenu)
        {
            return;
        }

        //move in grid
        
        Debug.Log("Moving: " + dir);
    }

    #region Hover over spell funcs
    /// <summary>
    /// calls a delayed coroutine to hover over the spell
    /// </summary>
    public void DelayHoverOverSpell()
    {
        //if theres already a spell trying to hover, cancel it
        if (hoverSpellCoroutine != null)
        {
            StopCoroutine(hoverSpellCoroutine);
        }
        
        //start hovering over a new spell
        hoverSpellCoroutine = StartCoroutine(DelayedHoverSpell());
    }

    /// <summary>
    /// Actual logic to show the popup
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.Exception"></exception>
    private IEnumerator DelayedHoverSpell()
    {
        //save the obj you're hovered over
        GameObject hoveredObj = EventSystem.current.currentSelectedGameObject;

        //if the time to wait for is less than or equal to 0, it'll cause an infinite loop
        if (delayBeforeSpellPopup <= 0)
        {
            throw new System.Exception("DelayedHoverSpell in the InCombatControllerManager must be" +
                " greater than 0. If not, it will cause an infinite loop and crash your Unity.");
        }

        //wait for the delay
        yield return new WaitForSeconds(delayBeforeSpellPopup);

        //if we are no longer selecting the ui menu, we don't show the hover
        if (!controllerOnUIMenu)
        {
            yield break;
        }

        //if we aren't hovering the same object that we were at the start of the timer, don't show the popup
        if (EventSystem.current.currentSelectedGameObject != hoveredObj)
        {
            yield break;
        }

        //LOGIC TO TRIGGER THE INFO BOX GOES HERE
    }

    #endregion
}
