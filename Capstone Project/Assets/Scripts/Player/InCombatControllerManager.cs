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

    private void OpenCombat()
    {
        lastSelectedButton = firstCombatUIButton;
        lastSelectedGridTile = defaultGridSelectCoords;

        EventSystem.current.SetSelectedGameObject(firstCombatUIButton);
    }

    private void StartPlottingPath()
    {
        isPlottingPath = true;
    }

    private void EndPlottingPath()
    {
        isPlottingPath = false;
    }

    private void ToggleBetweenGridAndUI()
    {
        //dont do this if you aren't in the default ui menu
        if (!PlayerIsInUIMenu)
        {
            return;
        }

        if (controllerOnUIMenu)
        {
            lastSelectedButton = EventSystem.current.currentSelectedGameObject;
            controllerOnUIMenu = false;

            //swap over to grid and highlight it
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
            controllerOnUIMenu = true;

            //leave grid
        }
    }

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

    public void SaveCurrentTile(Vector2Int tile)
    {
        lastSelectedGridTile = tile;
    }

    #region Hover over spell funcs
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

    private IEnumerator DelayedHoverSpell()
    {
        //save the obj you're hovered over
        GameObject hoveredObj = EventSystem.current.currentSelectedGameObject;
        float timer = 0;

        //if the time to wait for is less than or equal to 0, it'll cause an infinite loop
        if (delayBeforeSpellPopup <= 0)
        {
            throw new System.Exception("DelayedHoverSpell in the InCombatControllerManager must be" +
                " greater than 0. If not, it will cause an infinite loop and crash your Unity.");
        }

        //wait for the delay
        while (timer < delayBeforeSpellPopup)
        {
            yield return null;
            timer += Time.deltaTime;
        }

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

        //TRIGGER THE SHOW HOVER BOX NOW
    }

    #endregion
}
