/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 10/20/2025
 * Last Modified: 12/02/2025
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

    [Tooltip("Set to true if you want the ranged enemy to have to be at it's max range to attack")]
    [SerializeField] bool moveToAttackRange;

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
        targetLocations = GridManager.GetAllValidAdjacentTiles(playerPos, GetComponent<GridPathfinding>().MyPosition, false);
        List<Vector2Int> adTiles = new List<Vector2Int>();
        foreach (Vector2Int v in targetLocations)
        {
            //Can condence to 1 line, just written for testing purposes
            if (GetComponent<RangedEnemy>().minimumAttackDistance > 1)
            {
                GridManager.combatGrid[v.x, v.y].entityOnGrid = 1;
            }
            else
            {
                if (GridManager.combatGrid[v.x, v.y].entityOnGrid != -2 ||
                    GridManager.combatGrid[v.x, v.y].entityOnGrid != -4 ||
                    GridManager.combatGrid[v.x, v.y].entityOnGrid != -5)
                {
                    GridManager.combatGrid[v.x, v.y].entityOnGrid = 4;
                }
                
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
                    if (i > GetComponent<RangedEnemy>().minimumAttackDistance && HasLineOfSight(v))
                    {
                        if (GridManager.combatGrid[v.x, v.y].entityOnGrid != -2 ||
                             GridManager.combatGrid[v.x, v.y].entityOnGrid != -4 ||
                             GridManager.combatGrid[v.x, v.y].entityOnGrid != -5)
                        {
                            targetLocations.Add(v);
                            GridManager.combatGrid[v.x, v.y].entityOnGrid = 4;
                        }
                        
                    }
                    else
                    {
                        GridManager.combatGrid[v.x, v.y].entityOnGrid = 1;
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

    private bool HasLineOfSight(Vector2Int enemyTile)
    {
        float yDistance = GridManager.combatGrid[enemyTile.x, enemyTile.y].gameObject.GetComponent<BoxCollider>().bounds.size.y;
        Vector3 tilePosition = GridManager.combatGrid[enemyTile.x, enemyTile.y].gameObject.transform.position;
        Vector3 enemyPos = tilePosition + new Vector3(0, yDistance, 0);
        Vector3 endTilePos = GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y].gameObject.transform.position;
        Vector3 endPos = endTilePos - new Vector3(0, -yDistance, 0);
        RaycastHit hit;
        Physics.Linecast(enemyPos, endPos, out hit, ~doesNotBlockLOS);
        return hit.collider.tag == "Player";
    }
}
