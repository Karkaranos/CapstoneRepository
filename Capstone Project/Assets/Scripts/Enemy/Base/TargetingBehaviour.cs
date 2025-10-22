/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 10/20/2025
 * Last Modified: 10/21/2025
 * Brief: Contains the different enemy targeting behaviours that are assigned
 * to enums.
 * External Resources:
 * ***************************************************************************/
using UnityEngine;
using System.Collections.Generic;

public class TargetingBehaviour : MonoBehaviour
{
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
    [Tooltip("The attack range of the ranged enemy. Does nothing for enemies without a ranged attack")]
    [SerializeField] int attackRange;

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
        targetLocations = GridManager.GetAllValidAdjacentTiles(playerPos, GetComponent<GridPathfinding>().MyPosition);
    }

    /// <summary>
    /// Has the enemy move to a tile x spaces away where they have an 
    /// unobstructed vision line to the player
    /// </summary>
    private void RangedTargeting()
    {
        Debug.Log("AR = " + attackRange);
        if(attackRange <= 0)
        {
            attackRange = 1;
        }

        targetLocations = GridManager.GetAllValidAdjacentTiles(playerPos, GetComponent<GridPathfinding>().MyPosition);
        List<Vector2Int> adTiles = new List<Vector2Int>();
        foreach (Vector2Int v in targetLocations)
        {
            GridManager.combatGrid[v.x, v.y] = 4;
            adTiles.Add(v);
        }
        List<Vector2Int> newLocations = new List<Vector2Int>();
        for(int i = 1; i <= attackRange; ++i)
        {
            if(moveToAttackRange)
            {
                targetLocations.Clear();
            }
            foreach(Vector2Int v in adTiles)
            {
                if (FindIndexDistance(v) >= i && !targetLocations.Contains(v))
                {
                    targetLocations.Add(v);
                }
                List<Vector2Int> temp = GridManager.GetAllValidAdjacentTiles(v, GetComponent<GridPathfinding>().MyPosition);
                
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

        GridManager.ClearPathfinding();
    }

    private int FindIndexDistance(Vector2Int testedTile)
    {
        if(testedTile.x <= 0)
        {
            Debug.Log("HEHEHEHEHHEHHEHEHE");
            return (Mathf.Abs(playerPos.x - testedTile.x)) + (Mathf.Abs(playerPos.y - testedTile.y));
        }
        return (Mathf.Abs(playerPos.x - testedTile.x)) + (Mathf.Abs(playerPos.y - (Mathf.CeilToInt((float) testedTile.y / 2))));
    }
}
