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
using UnityEngine.Rendering.Universal;

public class InCombatControllerManager : MonoBehaviour
{
    [SerializeField] private GameObject firstCombatUIButton;
    [SerializeField] private Vector2Int defaultGridSelectCoords;
    [SerializeField] private float delayBeforeSpellPopup = 0.5f;
    [SerializeField] private Color highlightColor;

    private GameObject lastSelectedButton;
    private Vector2Int currentSelectedGridTile;
    private Vector2Int prevSelectedGridTile;
    private bool controllerOnUIMenu;
    private bool PlayerIsInUIMenu;

    private bool isPlottingPath;
    private Coroutine hoverSpellCoroutine;
    private GridManager gridMan;

    //UNCOMMENT THIS WHEN YOU WANT TO ACTUALLY START TESTING
    private void OnEnable()
    {
        PublicEvents.ToggleGridView += ToggleBetweenGridAndUI;
        PublicEvents.ControllerMoveInGrid += PlayerMovedController;
        PublicEvents.NewLevel += OpenUIMenu;
    }

    private void OnDisable()
    {
        PublicEvents.ToggleGridView -= ToggleBetweenGridAndUI;
        PublicEvents.ControllerMoveInGrid -= PlayerMovedController;
        PublicEvents.NewLevel -= OpenUIMenu;
    }

    #region Toggles

    /// <summary>
    /// triggers whenever the player goes back to the incombat menu
    /// this includes when the player is done moving and done casting a spell
    /// </summary>
    private void OpenUIMenu()
    {
        PlayerIsInUIMenu = true;
        lastSelectedButton = firstCombatUIButton;
        currentSelectedGridTile = defaultGridSelectCoords;

        EventSystem.current.SetSelectedGameObject(firstCombatUIButton);
    }

    private void CloseUIMenu()
    {
        PlayerIsInUIMenu = false;
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
            EventSystem.current.SetSelectedGameObject(null);

            //swap over to grid and highlight it
            MoveToTile(currentSelectedGridTile);
        }
        else
        {
            //go back to ui

            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
            controllerOnUIMenu = true;

            //leave grid

            TileBehaviour prevTile = null;

            if (GridManager.TileIsInGrid(prevSelectedGridTile))
            {
                prevTile = GridManager.combatGrid[prevSelectedGridTile.x, prevSelectedGridTile.y];
                prevTile.ShowHighlight(false);
            }
        }
    }

    #endregion

    /// <summary>
    /// Allows the player to move around in the grid
    /// </summary>
    /// <param name="dir"> the direction the player moves in </param>
    private void PlayerMovedController(Vector2 dir)
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
        Vector2Int newDir = new Vector2Int(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y));

        //only moves if the tile is in the grid
        if (GridManager.TileIsInGrid(currentSelectedGridTile + newDir))
        {
            currentSelectedGridTile += newDir;
            MoveToTile(currentSelectedGridTile);
        }

       // Debug.Log("Moving: " + dir);
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

    /// <summary>
    /// The logic that changes the tile's highlights when you move to a new one
    /// </summary>
    /// <param name="tile"></param>
    private void MoveToTile(Vector2Int tile)
    {
        //make sure we r actually in the combat menu and that we r properly moving
        if (controllerOnUIMenu)
        {
            return;
        }

        Debug.Log("char says hi");

        if (!PlayerIsInUIMenu)
        {
            return;
        }

        TileBehaviour prevTile = null;
        TileBehaviour currentTile = null;

        if (GridManager.TileIsInGrid(prevSelectedGridTile))
        {
            prevTile = GridManager.combatGrid[prevSelectedGridTile.x, prevSelectedGridTile.y];
        }

        if (GridManager.TileIsInGrid(tile))
        {
            currentTile = GridManager.combatGrid[tile.x, tile.y];
        }

        if (currentTile == null)
        {
            return;
        }

        if (prevTile != null)
        {
            prevTile.ShowHighlight(false);
        }

        currentTile.SetHighlightColor(highlightColor);
        currentTile.ShowHighlight(true);
    }

    #endregion
}
