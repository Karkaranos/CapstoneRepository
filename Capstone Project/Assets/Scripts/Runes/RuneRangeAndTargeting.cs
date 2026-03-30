/*************************************************
Author Names : 	Jay Embry, Clare Grady
Date Created : 	10/07/2025
Date Last Modified : 03/12/2026
Brief Description : Determines viable targets whenever a spell is selected
External Resources : 	
	***************************************************/

using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class RuneRangeAndTargeting : MonoBehaviour
{

    #region INITIALIZATION

    //for waiting on player input
    [HideInInspector] public bool WaitingForThePlayer;
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
    //canvas for movement/end turn buttons
    GameObject confirmationMenu;

    private List<Enemy> enemiesInRange = new List<Enemy>();

    [Header("Highlight Colors")]
    public Color DefaultHighlight;
    public Color BlockedHighlight;
    public Color LightningHighlight;
    public Color LightningSecondaryHighlight;
    public Color WindHighlight;
    public Color WindSecondaryHighlight;

    /// <summary>
    /// Runs whenever this script is loaded into a scene
    /// </summary>
    private void OnEnable()
    {

        //change this line of code to something that sucks less later
        confirmationMenu = FindFirstObjectByType<ButtonManager>().confirmCanvas;

        PublicEvents.SelectTarget += TargetSelection;
        PublicEvents.RuneSelected += StoreSelectedRuneData;
        PublicEvents.CheckRange += RangeCheck;
        PublicEvents.EndCast += EndPlayerAttackPhase;
        PublicEvents.SpellConfirmed += OnSpellCastConfirm;

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
        PublicEvents.SpellConfirmed -= OnSpellCastConfirm;

    }

    #endregion INITIALIZATION



    #region INITIAL TARGETING

    /// <summary>
    /// Prepares the rune that the player chooses to attack with
    /// </summary>
    /// <param name="rd"> Rune Data </param>
    public void StoreSelectedRuneData(RuneData rd)
    {

        SetHighlight(false);

        PublicEvents.HideDamagePreview();
        FindFirstObjectByType<PlayerBehavior>().SetPlayerMovementStatus(false);

        if(!GetComponent<RuneEvents>().WaitingOnPath)
        {

            WaitingForThePlayer = true;

            if(confirmationMenu == null)
            {
                confirmationMenu = FindFirstObjectByType<ButtonManager>().confirmCanvas;
            }

            confirmationMenu.SetActive(true);
            GameObject.Find("Confirm").GetComponent<Button>().interactable = false;

            storedData = rd;

            RangeCheck(false);
            //in range check get all enemies in range via tiles entities 
            //then add to enemies in range list 
            //call show damage preview here with 
            foreach(Enemy enemy in enemiesInRange)
            {
                enemy.ShowDamagePreview(rd.RuneDamage);
                enemy.isShowingPreview = true;
            }
        }

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
            GridManager.combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -3 ||
            GridManager.combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -5;

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

        if (storedData.TypeOfRune == RuneType.Lightning && storedData.NumberOnSkillTree == 4)
        {

            tilesInRange.Add(GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y]);
            TargetCheck(tilesInRange);

            return;

        }

        if (!isRadiusCheck)
        {

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

                if (GridManager.combatGrid[tile.x, tile.y].entityOnGrid == -2)
                {
                    Enemy enemy = GridManager.combatGrid[tile.x, tile.y].gameObject.GetComponentInChildren<Enemy>();
                    if(enemy != null)
                    {
                        enemiesInRange.Add(enemy);
                    }
                }

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

    /// <summary>
    /// checks which tiles are actually targetable
    /// </summary>
    /// <param name="tilesInRange"> tiles that the player may target </param>
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

            //"targets" the player
            case (RuneType.Lightning, 4):

                viableTilesInRange = tilesInRange;

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

            //targets an empty tile
            case (RuneType.Wind, 2):

                foreach (TileBehaviour tile in tilesInRange)
                {

                    if (!tile.GetComponentInChildren<Enemy>())
                    {

                        viableTilesInRange.Add(tile);

                    }

                }

                break;

            //targets an empty tile
            case (RuneType.Wind, 3):


                foreach (TileBehaviour tile in tilesInRange)
                {

                    if (!tile.GetComponentInChildren<Enemy>())
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

        SetHighlight(true, tilesInRange);

    }

    void SetHighlight(bool runeSelected, List<TileBehaviour> tilesInRange = null)
    {

        GridManager.RemoveHighlight();

        if (tilesInRange != null)
        {

            foreach (TileBehaviour tile in tilesInRange)
            {

                tile.SetHighlightColor(BlockedHighlight);
                tile.ShowHighlight(true);

            }

        }

        if (runeSelected)

        {

            foreach (TileBehaviour tile in viableTilesInRange)
            {

                switch (storedData.TypeOfRune)
                {

                    case (RuneType.Lightning):

                        tile.SetHighlightColor(LightningHighlight);

                        tile.ShowHighlight(true);

                        break;

                    case (RuneType.Wind):

                        tile.SetHighlightColor(WindHighlight);

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

    /// <summary>
    /// adds/removes pathed tiles into list of viable targets
    /// </summary>
    /// <param name="addingToTiles"> whether a tile is being added or removed </param>
    /// <param name="newTile"> the player's selected tile </param>
    public void EditViableTiles(bool addingToTiles, TileBehaviour newTile)
    {

        if(addingToTiles)
        {

            viableTilesInRange.Add(newTile);

        }
        else
        {

            viableTilesInRange.Remove(newTile);

        }

    }

    [HideInInspector] public TileBehaviour selectedTile;
    Enemy selectedEnemy;
    PlayerBehavior selectedPlayer;

    /// <summary>
    /// stores the tile/enemy selected
    /// </summary>
    /// <param name="tile"> the tile that the player has selected </param>
    /// <param name="enemy"> the enemy that the player has selected </param>
    /// <param name="player"> for when the player is targeting themself, for whatever reason </param>
    public void TargetSelection(TileBehaviour tile, Enemy enemy, PlayerBehavior player)
    {

        if (WaitingForThePlayer && viableTilesInRange.Contains(tile))
        {

            GameObject.Find("Confirm").GetComponent<Button>().interactable = true;

            if (selectedTile != null && !this.gameObject.GetComponent<RuneEvents>().WaitingOnPath)
            {

                if(storedData.TypeOfRune == RuneType.Lightning)
                {
                    selectedTile.SetHighlightColor(LightningHighlight);
                }
                else
                {
                    selectedTile.SetHighlightColor(WindHighlight);
                }

            }

            selectedTile = tile;
            selectedEnemy = enemy;
            selectedPlayer = player;

            selectedTile.SetHighlightColor(DefaultHighlight);

            if((storedData.TypeOfRune == RuneType.Lightning && storedData.NumberOnSkillTree == 4) ||
            (storedData.TypeOfRune == RuneType.Wind))
            {

                if(!this.gameObject.GetComponent<RuneEvents>().WaitingOnPath)
                {
                    OnSpellCastConfirm();
                }

            }

        }

    }

    /// <summary>
    /// called when the confirm button is clicked
    /// </summary>
    public void OnSpellCastConfirm()
    {

        switch (storedData.TypeOfRune)
        {

            case (RuneType.Lightning):

                PublicEvents.HideEnemyStatbox.Invoke();
                PublicEvents.LightningCast.Invoke(storedData, selectedTile, selectedEnemy, selectedPlayer);
                break;

            case (RuneType.Wind):

                PublicEvents.HideEnemyStatbox.Invoke();
                PublicEvents.WindCast.Invoke(storedData, selectedTile, selectedEnemy, selectedPlayer);
                break;

            default:

                break;

        }

    }

    #endregion PLAYER TARGETING



    #region END TURN

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

        WaitingForThePlayer = false;

        confirmationMenu.SetActive(false);

        FindFirstObjectByType<PlayerBehavior>().SetPlayerMovementStatus(true);

        if (GetComponent<RuneEvents>().WaitingOnPath)
        {

            GetComponent<RuneEvents>().CancelPathing();

        }

        SetHighlight(false);

        if (castNotCanceled)
        {

            PublicEvents.RuneCast(storedData.RuneActionPoints);

            castNotCanceled = false;

        }

        if (TurnManager.currentStatus == TurnStates.PlayerTurn)
        {
            FindFirstObjectByType<ButtonManager>().ResetCanvas();
        }

        GridManager.RemoveHighlight();

        //this.gameObject.SetActive(false);

    }

    #endregion END TURN

}
