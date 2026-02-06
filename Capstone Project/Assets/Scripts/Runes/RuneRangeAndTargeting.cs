/*************************************************
Author Names : 	Jay Embry
Date Created : 	10/07/2025
Date Last Modified : 01/22/2026
Brief Description : Determines viable targets whenever a spell is selected
External Resources : 	
	***************************************************/

using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class RuneRangeAndTargeting : MonoBehaviour
{

    #region INITIALIZATION

    //for waiting on player input
    bool waitingForThePlayer;
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

    /// <summary>
    /// exits attack menu if waiting on a target
    /// </summary>
    public void CancelCasting()
    {

        if (waitingForThePlayer)
        {

            waitingForThePlayer = false;

        }

        GridManager.RemoveHighlight();

        foreach (TileBehaviour tile in FindFirstObjectByType<PlayerBehavior>().tilesInRange)
        {

            tile.SetHighlightColor(defaultHighlight);
            tile.ShowHighlight(true);

        }

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

            foreach(TileBehaviour tile in FindFirstObjectByType<PlayerBehavior>().tilesInRange)
            {

                tile.SetHighlightColor(defaultHighlight);

                tile.ShowHighlight(true);

            }

        }

    }

    #endregion INITIAL TARGETING



    #region PLAYER TARGETING

    /// <summary>
    /// triggers spells based on the tile or enemy that the player has selected
    /// </summary>
    /// <param name="tile"> the tile that the player has selected </param>
    /// <param name="enemy"> the enemy that the player has selected </param>
    /// <param name="player"> for when the player is targeting themself, for whatever reason </param>
    public void TargetSelection(TileBehaviour tile, Enemy enemy, PlayerBehavior player)
    {

        if (waitingForThePlayer &&
            FindFirstObjectByType<GameManager>().CurrentActionPoints >= storedData.RuneActionPoints &&
            viableTilesInRange.Contains(tile))
        {

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

        GridManager.RemoveHighlight();

        SetHighlight(false);

        if (castNotCanceled)
        {

            PublicEvents.RuneCast(storedData.RuneActionPoints);

        }

        if (TurnManager.currentStatus == TurnStates.PlayerTurn)
        {
            playerMenu.SetActive(true);
        }

        this.gameObject.SetActive(false);

        castNotCanceled = false;

    }

}
