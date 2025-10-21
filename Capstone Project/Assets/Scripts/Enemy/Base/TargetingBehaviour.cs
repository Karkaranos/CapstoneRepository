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

    /// <summary>
    /// Public function that can be called from the statemachine, just make 
    /// sure to change the TargetingBehaviours enum first
    /// </summary>
    public void FindTarget()
    {
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
        targetLocations = GridManager.GetAllValidAdjacentTiles(playerPos);
    }

    /// <summary>
    /// Has the enemy move to a tile x spaces away where they have an 
    /// unobstructed vision line to the player
    /// </summary>
    private void RangedTargeting()
    {
        Debug.Log("HEER");
        if(attackRange <= 0)
        {
            attackRange = 1;
        }

        targetLocations = GridManager.GetAllValidAdjacentTiles(playerPos);
        List<Vector2Int> newLocations = new List<Vector2Int>();
        List<Vector2Int> temp = new List<Vector2Int>();
        for(int i = 1; i < attackRange; ++i)
        {
            foreach(Vector2Int v in targetLocations)
            {
                temp = GridManager.GetAllValidAdjacentTiles(v);
                foreach(Vector2Int location in temp)
                {
                    newLocations.Add(location);
                }
            }
            targetLocations.Clear();

            foreach(Vector2Int v in newLocations)
            {
                targetLocations.Add(v);
            }
            newLocations.Clear();
        }
    }
}
