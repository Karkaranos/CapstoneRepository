/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 9/26/2025
 * Last Modified: 9/26/2025
 * Brief: Temporary script to test if the grid works. Non-temporary scripts 
 * should be added to the game manager after this is added to working
 * ***************************************************************************/
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

public class GridTesting : MonoBehaviour
{
    [SerializeField] private Vector2Int gridDimensions;

    //Temporary variables for testing purposes
    [Tooltip("The tile that would be where an object starts from")]
    [SerializeField] private Vector2Int currentMovementTestingTile;
    [Tooltip("The tile that on object would move to")]
    [SerializeField] private Vector2Int newMovementTestingTile;

    /// <summary>
    /// Shows the grid in the console on button press
    /// </summary>
    [Button("Show Grid In Console")]
    private void ShowGrid()
    {
        GridManager.DisplayGridAsText();
    }

    /// <summary>
    /// Updates movement in the grid and shows it in the console
    /// </summary>
    [Button("Tests Movement In Grid")]
    private void TestMovement()
    {
        GridManager.AddEntity(currentMovementTestingTile, -8);
        GridManager.DisplayGridAsText();
        if(GridManager.TileIsInGrid(newMovementTestingTile) && GridManager.CanMoveToTile(newMovementTestingTile))
        {
            GridManager.MoveToTile(currentMovementTestingTile, newMovementTestingTile, -8);
        }
        GridManager.DisplayGridAsText();
    }

    /// <summary>
    /// Creates an instance of the grid
    /// </summary>
    private void Awake()
    {
        GridManager.SetGrid(gridDimensions);
    }
}
