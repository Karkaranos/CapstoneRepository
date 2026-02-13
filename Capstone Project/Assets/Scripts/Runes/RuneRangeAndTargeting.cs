/*************************************************
Author Names : 	Jay Embry
Date Created : 	10/07/2025
Date Last Modified : 01/22/2026
Brief Description : Determines viable targets whenever a spell is selected
External Resources : 	
	***************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class RuneRangeAndTargeting : MonoBehaviour
{

    #region INITIALIZATION

    //for waiting on player input
    bool waitingForThePlayer;
    //for waiting after the player selects a wind spell
    bool waitingOnTheSecondSelection;
    //Stores the currently using rune
    private RuneData storedData;
    //updated everytime the player selects a spell
    //List<TileBehaviour> tilesInRange = new List<TileBehaviour>();
    //updated based on the map
    List<TileBehaviour> viableTilesInRange = new List<TileBehaviour>();
    //for swapping menus
    [SerializeField] GameObject playerMenu;
    //whether or not the cast was canceled
    bool castNotCanceled = false;

    [Header("Highlight Colors")]
    [SerializeField] Color defaultHighlight;
    [SerializeField] Color lightningHighlight;
    [SerializeField] Color lightningSecondaryHighlight;
    [SerializeField] Color windHighlight;
    [SerializeField] Color windSecondaryHighlight;

    /// <summary>
    /// Runs whenever this script is loaded into a scene
    /// </summary>
    private void OnEnable()
    {

        PublicEvents.SelectTarget += TargetSelection;
        PublicEvents.RuneSelected += StoreSelectedRuneData;
        PublicEvents.CheckRange += RangeCheck;
        PublicEvents.EndCast += EndPlayerAttackPhase;

    }

    /// <summary>
    /// Runs whenever this script is destroyed
    /// </summary>
    private void OnDisable()
    {

        PublicEvents.SelectTarget -= TargetSelection;
        PublicEvents.RuneSelected -= StoreSelectedRuneData;
        PublicEvents.CheckRange -= RangeCheck;
        PublicEvents.EndCast -= EndPlayerAttackPhase;

    }

    #endregion INITIALIZATION



    #region INITIAL TARGETING

    /// <summary>
    /// Prepares the rune that the player chooses to attack with
    /// </summary>
    /// <param name="rd"> Rune Data </param>
    public void StoreSelectedRuneData(RuneData rd)
    {

        waitingForThePlayer = true;

        storedData = rd;

        RangeCheck(false);

    }

    /// <summary>
    /// checks to see if a tile has literally anything other than an obstacle
    /// thanks brad
    /// </summary>
    /// <param name="tileCoordinates"> the tile that the player has selected </param>
    /// <returns> whether or not the tile can be targeted </returns>
    public static bool CanAttackTile(Vector2Int tileCoordinates)
    {

        if (tileCoordinates == GridManager.playerPosition)
        {
            return true;
        }

        return GridManager.combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -1 ||
            GridManager.combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -2 ||
            GridManager.combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -3;

    }

    /// <summary>
    /// creates a list of all targetable tiles upon selecting a spell
    /// </summary>
    /// <param name="currentTile"> the tile that the player has selected </param>
    /// <returns> list of targetable tiles </returns>
    public static List<Vector2Int> GetAllValidTargetableTiles(Vector2Int currentTile)
    {
        List<Vector2Int> validTiles = new List<Vector2Int>();

        if (GridManager.TileIsInGrid(new Vector2Int(currentTile.x + 1, currentTile.y)) && CanAttackTile(new Vector2Int(currentTile.x + 1, currentTile.y)))
        {
            validTiles.Add(new Vector2Int(currentTile.x + 1, currentTile.y));
        }
        if (GridManager.TileIsInGrid(new Vector2Int(currentTile.x - 1, currentTile.y)) && CanAttackTile(new Vector2Int(currentTile.x - 1, currentTile.y)))
        {
            validTiles.Add(new Vector2Int(currentTile.x - 1, currentTile.y));
        }
        if (GridManager.TileIsInGrid(new Vector2Int(currentTile.x, currentTile.y + 1)) && CanAttackTile(new Vector2Int(currentTile.x, currentTile.y + 1)))
        {
            validTiles.Add(new Vector2Int(currentTile.x, currentTile.y + 1));
        }
        if (GridManager.TileIsInGrid(new Vector2Int(currentTile.x, currentTile.y - 1)) && CanAttackTile(new Vector2Int(currentTile.x, currentTile.y - 1)))
        {
            validTiles.Add(new Vector2Int(currentTile.x, currentTile.y - 1));
        }

        return validTiles;
    }

    /// <summary>
    /// checks to see if a tile or enemy is in range before executing a spell
    /// this could be cleaner but i can worry about that later
    /// </summary>
    /// <param name="isRadiusCheck"> determines where the function is checking adjacent tiles from </param>
    /// <param name="radius"> a spell's potential radius/aoe </param>
    /// <param name="target"> the initial tile hit if isRadiusCheck </param>
    public void RangeCheck(bool isRadiusCheck, int radius = 0, TileBehaviour target = null)
    {

        List<TileBehaviour> tilesInRange = new List<TileBehaviour>();

        List<Vector2Int> validTiles = new List<Vector2Int>();

        if (!isRadiusCheck)
        {

            if (storedData.TypeOfRune == RuneType.Wind && storedData.NumberOnSkillTree == 3)
            {

                tilesInRange.Add(GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y]);

            }

            for (int i = 0; i < storedData.RuneRange + 1; i++)
            {

                if (i == 1)
                {

                    List<Vector2Int> initialAdjacentTiles = GetAllValidTargetableTiles(GridManager.playerPosition);

                    foreach (Vector2Int tile in initialAdjacentTiles.ToList())
                    {

                        if (tile != GridManager.playerPosition)
                        {

                            validTiles.Add(tile);

                        }

                    }

                    continue;

                }

                foreach (Vector2Int validTile in validTiles.ToList())
                {

                    List<Vector2Int> adjacentTiles = new List<Vector2Int>();

                    adjacentTiles = GetAllValidTargetableTiles(validTile);

                    foreach (Vector2Int tile in adjacentTiles.ToList())
                    {

                        if (validTiles.Contains(tile))
                        {

                            continue;

                        }
                        else if (tile != GridManager.playerPosition)
                        {

                            validTiles.Add(tile);

                        }

                    }

                }

            }

            foreach (Vector2Int tile in validTiles.ToList())
            {

                tilesInRange.Add(GridManager.combatGrid[tile.x, tile.y]);

            }

            validTiles.Clear();

            TargetCheck(tilesInRange);

        }
        else
        {

            Vector2Int targetPos = target.IndexInGrid;

            for (int i = 0; i < radius + 1; i++)
            {

                if (i == 1)
                {

                    List<Vector2Int> initialAdjacentTiles = GetAllValidTargetableTiles(targetPos);

                    foreach (Vector2Int tile in initialAdjacentTiles.ToList())
                    {

                        validTiles.Add(tile);

                    }

                    continue;

                }

                foreach (Vector2Int validTile in validTiles.ToList())
                {

                    List<Vector2Int> adjacentTiles = new List<Vector2Int>();

                    adjacentTiles = GetAllValidTargetableTiles(validTile);

                    foreach (Vector2Int tile in adjacentTiles.ToList())
                    {

                        if (validTiles.Contains(tile))
                        {

                            continue;

                        }
                        else
                        {

                            validTiles.Add(tile);

                        }

                    }

                }

            }

            foreach (Vector2Int tile in validTiles.ToList())
            {

                tilesInRange.Add(GridManager.combatGrid[tile.x, tile.y]);

            }

            validTiles.Clear();

        }

        FindFirstObjectByType<RuneEvents>().GetTargets(tilesInRange);

    }

    void TargetCheck(List<TileBehaviour> tilesInRange)
    {

        viableTilesInRange.Clear();

        switch (storedData.TypeOfRune, storedData.NumberOnSkillTree)
        {

            //targets any tile
            case (RuneType.Lightning, 1):

                viableTilesInRange = tilesInRange;

                break;

            //targets an enemy
            case (RuneType.Lightning, 2):

                foreach (TileBehaviour tile in tilesInRange)
                {

                    if (tile.GetComponentInChildren<Enemy>())
                    {

                        viableTilesInRange.Add(tile);

                    }

                }

                break;

            //targets an empty tile
            case (RuneType.Lightning, 3):

                foreach (TileBehaviour tile in tilesInRange)
                {

                    if (!tile.GetComponentInChildren<Enemy>())
                    {

                        viableTilesInRange.Add(tile);

                    }

                }

                break;

            //targets an empty tile
            case (RuneType.Lightning, 4):

                foreach (TileBehaviour tile in tilesInRange)
                {

                    if (!tile.GetComponentInChildren<Enemy>())
                    {

                        viableTilesInRange.Add(tile);

                    }

                }

                break;

            //targets an enemy
            case (RuneType.Wind, 1):

                foreach (TileBehaviour tile in tilesInRange)
                {

                    if (tile.GetComponentInChildren<Enemy>())
                    {

                        viableTilesInRange.Add(tile);

                    }

                }

                break;

            //targets an enemy
            case (RuneType.Wind, 2):

                foreach (TileBehaviour tile in tilesInRange)
                {

                    if (tile.GetComponentInChildren<Enemy>())
                    {

                        viableTilesInRange.Add(tile);

                    }

                }

                break;

            //targets a player
            case (RuneType.Wind, 3):


                foreach (TileBehaviour tile in tilesInRange)
                {

                    if (tile.GetComponentInChildren<PlayerBehavior>())
                    {

                        viableTilesInRange.Add(tile);

                    }

                }

                break;

            //targets any tile
            case (RuneType.Wind, 4):

                viableTilesInRange = tilesInRange;

                break;

        }

        SetHighlight(true);

    }

    void SetHighlight(bool runeSelected)
    {

        GridManager.RemoveHighlight();

        if(runeSelected)

        {

            foreach (TileBehaviour tile in viableTilesInRange)
            {

                switch (storedData.TypeOfRune)
                {

                    case (RuneType.Lightning):

                        tile.SetHighlightColor(lightningHighlight);

                        tile.ShowHighlight(true);

                        break;

                    case (RuneType.Wind):

                        tile.SetHighlightColor(windHighlight);

                        tile.ShowHighlight(true);

                        break;

                    default:

                        break;

                }

            }

        }

        else
        {

            foreach(TileBehaviour tile in viableTilesInRange)
            {

                tile.SetHighlightColor(Color.blue);

            }

        }


    }

    #endregion INITIAL TARGETING



    #region PLAYER TARGETING

    TileBehaviour targetedTile;
    Enemy targetedEnemy;

    int movementLeft;

    /// <summary>
    /// triggers spells based on the tile or enemy that the player has selected
    /// </summary>
    /// <param name="tile"> the tile that the player has selected </param>
    /// <param name="enemy"> the enemy that the player has selected </param>
    /// <param name="player"> for when the player is targeting themself, for whatever reason </param>
    public void TargetSelection(TileBehaviour tile, Enemy enemy, PlayerBehavior player)
    {

        if (waitingForThePlayer &&
        FindFirstObjectByType<GameManager>().CurrentActionPoints >= storedData.RuneActionPoints && viableTilesInRange.Contains(tile))

        {

            targetedTile = tile;
            targetedEnemy = enemy;

            ghostPos = tile.transform.position;

            movementLeft = storedData.RuneRange;

            switch (storedData.TypeOfRune)
            {

                case (RuneType.Lightning):

                    PublicEvents.LightningCast.Invoke(storedData, tile, enemy, player);
                    break;

                case (RuneType.Wind):

                    PublicEvents.WindCast.Invoke(storedData, tile, enemy, player);
                    break;

                default:

                    break;

            }

        }

    }

    #endregion PLAYER TARGETING



    #region SECONDARY TARGETING

    List<Vector2Int> tilesInPath = new List<Vector2Int>();
    List<Vector3> pathPos = new List<Vector3>();
    Vector3 ghostPos;

    /// <summary>
    /// determines which direction the player is pathing out their spell
    /// </summary>
    /// <param name="dir"> input </param>
    private void PathingDirection (Vector2 dir)
    {

        if(waitingOnTheSecondSelection)
        {

            if (dir.y >= .5f)
            {

                Vector2Int v = new Vector2Int(GridManager.playerPosition.x, GridManager.playerPosition.y + 1);

                if (GridManager.TileIsInGrid(v) && CanAttackTile(v) && (!tilesInPath.Contains(v) || v == tilesInPath[tilesInPath.Count - 2]) &&
                Mathf.Abs(GridManager.playerPosition.y - targetedTile.IndexInGrid.y) <= storedData.RuneRange)
                {

                    Vector3 newPos = new Vector3(ghostPos.x, ghostPos.y, ghostPos.z + GridManager.MoveDistances.y);
                    UpdatePath(v, newPos);

                }

            }
            else if (dir.y <= -.5f)
            {

                Vector2Int v = new Vector2Int(GridManager.playerPosition.x, GridManager.playerPosition.y - 1);

                if (GridManager.TileIsInGrid(v) && CanAttackTile(v) && (!tilesInPath.Contains(v) || v == tilesInPath[tilesInPath.Count - 2]) &&
                Mathf.Abs(GridManager.playerPosition.y - targetedTile.IndexInGrid.y) <= storedData.RuneRange)
                {

                    Vector3 newPos = new Vector3(ghostPos.x, ghostPos.y, ghostPos.z - GridManager.MoveDistances.y);
                    UpdatePath(v, newPos);

                }

            }
            else if (dir.x > .5f)
            {

                Vector2Int v = new Vector2Int(GridManager.playerPosition.x + 1, GridManager.playerPosition.y);

                if (GridManager.TileIsInGrid(v) && CanAttackTile(v) && (!tilesInPath.Contains(v) || v == tilesInPath[tilesInPath.Count - 2]) &&
                Mathf.Abs(GridManager.playerPosition.y - targetedTile.IndexInGrid.y) <= storedData.RuneRange)
                {

                    Vector3 newPos = new Vector3(ghostPos.x + GridManager.MoveDistances.x, ghostPos.y, ghostPos.z);
                    UpdatePath(v, newPos);

                }

            }
            else if (dir.x < -.5f)
            {

                Vector2Int v = new Vector2Int(GridManager.playerPosition.x - 1, GridManager.playerPosition.y);

                if (GridManager.TileIsInGrid(v) && CanAttackTile(v) && (!tilesInPath.Contains(v) || v == tilesInPath[tilesInPath.Count - 2]) &&
                Mathf.Abs(GridManager.playerPosition.y - targetedTile.IndexInGrid.y) <= storedData.RuneRange)
                {

                    Vector3 newPos = new Vector3(ghostPos.x - GridManager.MoveDistances.x, ghostPos.y, ghostPos.z);
                    UpdatePath(v, newPos);

                }

            }

        }

    }

    private void UpdatePath(Vector2Int v, Vector3 newPos)
    {

        waitingOnTheSecondSelection = false;

        if (tilesInPath.Contains(v))
        {

            pathPos.RemoveAt(pathPos.Count - 1);
            ++movementLeft;

        }
        else
        {

            if (movementLeft > 0)
            {

                switch (storedData.TypeOfRune, storedData.NumberOnSkillTree)
                {

                    case (RuneType.Wind, 1):

                        if ((GridManager.playerPosition.x < targetedTile.IndexInGrid.x && v.x > GridManager.playerPosition.x) ||
                        (GridManager.playerPosition.x > targetedTile.IndexInGrid.x && v.x < GridManager.playerPosition.x) ||
                        (GridManager.playerPosition.y < targetedTile.IndexInGrid.y && v.y > GridManager.playerPosition.y) ||
                        (GridManager.playerPosition.y > targetedTile.IndexInGrid.y && v.y < GridManager.playerPosition.y))
                        {

                            GridManager.combatGrid[v.x, v.y].SetHighlightColor(windSecondaryHighlight);
                            GridManager.combatGrid[v.x, v.y].ShowHighlight(true);

                            tilesInPath.Add(v);
                            pathPos.Add(newPos);

                            --movementLeft;

                        }

                        break;

                    default:

                        GridManager.combatGrid[v.x, v.y].SetHighlightColor(windSecondaryHighlight);
                        GridManager.combatGrid[v.x, v.y].ShowHighlight(true);

                        tilesInPath.Add(v);
                        pathPos.Add(newPos);

                        --movementLeft;

                        break;

                }

                if (movementLeft == 0)
                {

                    ghostPos = newPos;

                }

            }

        }

        if (movementLeft > 0)
        {

            ghostPos = newPos;

        }

        StartCoroutine(PathingDelay());

    }

    IEnumerator PathingDelay()
    {

        yield return new WaitForSeconds(0.5f);
        waitingOnTheSecondSelection = true;

    }

    IEnumerator ConfirmPathing()
    {

        waitingOnTheSecondSelection = false;

        switch(storedData.TypeOfRune, storedData.NumberOnSkillTree)
        {

            case (RuneType.Wind, 1):



                break;

        }

        //temp
        yield return null;

    }    

    #endregion SECONDARY TARGETING


    public void SetCastStatus(bool werePointsSpent)
    {

        castNotCanceled = werePointsSpent;

    }

    /// <summary>
    /// runs whenever an enemy is successfully targeted
    /// made into a function to prevent SOME clutter
    /// </summary>
    void EndPlayerAttackPhase()
    {

        waitingForThePlayer = false;

        SetHighlight(false);

        if (castNotCanceled)
        {

            PublicEvents.RuneCast(storedData.RuneActionPoints);

        }

        if (TurnManager.currentStatus == TurnStates.PlayerTurn)
        {
            playerMenu.SetActive(true);
        }
        else
        {
            GridManager.RemoveHighlight();
        }

        this.gameObject.SetActive(false);

        castNotCanceled = false;

    }

}
