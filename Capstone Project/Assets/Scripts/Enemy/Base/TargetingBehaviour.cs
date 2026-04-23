/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 10/20/2025
 * Last Modified: 4/23/2026
 * Brief: Contains the different enemy targeting behaviours that are assigned
 * to enums.
 * External Resources:
 * ***************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

public class TargetingBehaviour : MonoBehaviour
{
    /// <summary>
    /// Runs the targeting behvaiour without moving the enemy, and it populates a list with 
    /// all potential locations the enemy could make an attack from
    /// </summary>
    [Button("Test Ranged Targeting")]
    private void CallTargeting()
    {
        targetLocations.Clear();
        playerPos = GridManager.playerPosition;
        RangedTargeting();
    }

    [Button("Test Range Algorithm")]
    private void TestRange()
    {
        HasLineOfSight(GetComponent<GridPathfinding>().MyPosition, false);
    }

    [Tooltip("Layers that should be ignored for a raycast. These would be for " +
        "objects that you wouldn't want to block enemy line of sight. Ex. Hazards")]
    [SerializeField] private LayerMask doesNotBlockLOS;

    public enum TargetingBehaviours
    {
        melee,
        ranged
    }

    [HideInInspector]
    public TargetingBehaviours behaviours;
    //[HideInInspector]
    public List<Vector2Int> targetLocations = new List<Vector2Int>();
    Vector2Int playerPos;
    bool reachedTheEnd;

    [Tooltip("Set to true if you want the ranged enemy to have to be at it's max range to attack")]
    [SerializeField] bool moveToAttackRange;

    [SerializeField] List<Vector2Int> branchingPaths = new List<Vector2Int>();

    /// <summary>
    /// Public function that can be called from the statemachine, just make 
    /// sure to change the TargetingBehaviours enum first
    /// </summary>
    public void FindTarget()
    {
        targetLocations.Clear();
        playerPos = GridManager.playerPosition;
        switch(behaviours)
        {
            case TargetingBehaviours.melee:
                MeleeTargeting();
                break;
            case TargetingBehaviours.ranged:
                RangedTargeting();
                break;
            default:
                Debug.Log("ERROR");
                break;
        }
    }

    /// <summary>
    /// Has the enemy move to an adjacent tile next to the player
    /// </summary>
    private void MeleeTargeting()
    {
        targetLocations = GridManager.GetAllValidAdjacentTiles(playerPos, GetComponent<GridPathfinding>().MyPosition, false);
    }

    /// <summary>
    /// Has the enemy move to a tile x spaces away where they have an 
    /// unobstructed vision line to the player
    /// </summary>
    private void RangedTargeting()
    {
        branchingPaths.Clear();
        reachedTheEnd = false;
        targetLocations = GridManager.GetAllValidAdjacentTiles(playerPos, GetComponent<GridPathfinding>().MyPosition, false);
        List<Vector2Int> adTiles = new List<Vector2Int>();
        foreach (Vector2Int v in targetLocations)
        {
            //Can condence to 1 line, just written for testing purposes
            if (GetComponent<RangedEnemy>().minimumAttackDistance > 1)
            {
                //GridManager.combatGrid[v.x, v.y].entityOnGrid = 1;
            }
            else
            {
                //if (GridManager.combatGrid[v.x, v.y].entityOnGrid != -2 &&
                //    GridManager.combatGrid[v.x, v.y].entityOnGrid != -4 &&
                //    GridManager.combatGrid[v.x, v.y].entityOnGrid != -5)
                //{
                //    GridManager.combatGrid[v.x, v.y].entityOnGrid = 4;
                //}
                
            }
            adTiles.Add(v);
        }

        if(GetComponent<RangedEnemy>().minimumAttackDistance > 1)
        {
            targetLocations.Clear();
        }

        List<Vector2Int> newLocations = new List<Vector2Int>();
        for(int i = 1; i <= GetComponent<RangedEnemy>().maxAttackDistance; ++i)
        {
            if(moveToAttackRange)
            {
                targetLocations.Clear();
            }
            foreach(Vector2Int v in adTiles)
            {
                if (v != playerPos && !targetLocations.Contains(v))
                {
                    //Can remove the else and have line 108 be outside the if statement, just written this way for testing purposes
                    if (i > GetComponent<RangedEnemy>().minimumAttackDistance && HasLineOfSight(v, false))
                    {
                        //if (GridManager.combatGrid[v.x, v.y].entityOnGrid != -2 &&
                        //     GridManager.combatGrid[v.x, v.y].entityOnGrid != -4 &&
                        //     GridManager.combatGrid[v.x, v.y].entityOnGrid != -5)
                        //{
                            targetLocations.Add(v);
                        //    GridManager.combatGrid[v.x, v.y].entityOnGrid = 4;
                        //}
                        
                    }
                    else
                    {
                        //GridManager.combatGrid[v.x, v.y].entityOnGrid = 1;
                    }
                }
                List<Vector2Int> temp = GridManager.GetAllValidAdjacentTiles(v, GetComponent<GridPathfinding>().MyPosition, false);
                
                //Populates the next potential tiles to be chosen
                foreach(Vector2Int j in temp)
                {
                    newLocations.Add(j);
                }
            }
            adTiles.Clear();
            foreach(Vector2Int v in newLocations)
            {
                adTiles.Add(v);
            }
            newLocations.Clear();
        }

        //GridManager.DisplayGridAsText();
        GridManager.ClearPathfinding();
    }

    /// <summary>
    /// Checks if a tile has line of sight with the player. If it does, then the enemy knows that is a valid target location
    /// </summary>
    /// <param name="enemyTile"></param>
    /// <returns></returns>
    private bool HasLineOfSight(Vector2Int enemyTile, bool checkOtherPath)
    {
        //Find distance away
        int xDistance = Mathf.Abs(enemyTile.x - GridManager.playerPosition.x); 
        int yDistance = Mathf.Abs(enemyTile.y - GridManager.playerPosition.y);

        if(xDistance + yDistance == 0)
        {
            reachedTheEnd = true;
            return true;
        }

        List<Vector2Int> tilesWeCanCheck = new List<Vector2Int>();
        int availablePaths = 0;

        //Determines which direction we need to check for line of sight
        Vector2Int xTileCheck = enemyTile.x > GridManager.playerPosition.x ?
            new Vector2Int(enemyTile.x - 1, enemyTile.y) : new Vector2Int(enemyTile.x + 1, enemyTile.y);
        Vector2Int yTileCheck = enemyTile.y > GridManager.playerPosition.y ?
            new Vector2Int(enemyTile.x, enemyTile.y - 1) : new Vector2Int(enemyTile.x, enemyTile.y + 1);

        Debug.Log("X: " + xDistance);
        Debug.Log("Y: " + yDistance);

        //If the x and y distances are the same
        if (xDistance == yDistance)
        {
            //Checks how many paths we can go down
            if (GridManager.TileIsEmpty(xTileCheck) && !checkOtherPath)
            {
                ++availablePaths;
                tilesWeCanCheck.Add(xTileCheck);
            }
            if (GridManager.TileIsEmpty(yTileCheck))
            {
                ++availablePaths;
                tilesWeCanCheck.Add(yTileCheck);
            }
        }
        //If the x distance is greater than the y distance
        else if(xDistance > yDistance && (GridManager.TileIsEmpty(xTileCheck) || 
            GridManager.combatGrid[xTileCheck.x, xTileCheck.y].entityOnGrid == -3))
        {
            ++availablePaths;
            tilesWeCanCheck.Add(xTileCheck);
        }
        //If the y distance is greater than the x distance
        else if(yDistance > xDistance && (GridManager.TileIsEmpty(yTileCheck) || 
            GridManager.combatGrid[yTileCheck.x, yTileCheck.y].entityOnGrid == -3))
        {
            ++availablePaths;
            tilesWeCanCheck.Add(yTileCheck);
        }

        switch(availablePaths)
        {
            case 0:
                //Checks the second path of a branched path
                if(branchingPaths.Count > 0)
                {
                    Vector2Int tileToCheck = branchingPaths[branchingPaths.Count - 1];
                    branchingPaths.Remove(tileToCheck);
                    HasLineOfSight(tileToCheck, true);
                }
                break;
            case 1:
                HasLineOfSight(tilesWeCanCheck[0], false);
                break;
            case 2:
                branchingPaths.Add(enemyTile);
                HasLineOfSight(tilesWeCanCheck[0], false);
                break;
        }

        return reachedTheEnd;
    }
}
