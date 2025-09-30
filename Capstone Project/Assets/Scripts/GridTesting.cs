/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 9/26/2025
 * Last Modified: 9/30/2025
 * Brief: Temporary script to test if the grid works. Non-temporary scripts 
 * should be added to the game manager after this is added to working
 * ***************************************************************************/
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

public class GridTesting : MonoBehaviour
{
    [SerializeField] private List<GameObject> gridPrefabs = new List<GameObject>();
    [SerializeField] private List<Vector2Int> gridDimensions = new List<Vector2Int>();
    [SerializeField] private int gridIndex;

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
    /// Testing button used to test movement in the grid and shows it in the console
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

    [Button("Load Next Grid")]
    private void LoadGrid()
    {
        LoadNextGrid();
    }

    /// <summary>
    /// Creates an instance of the grid
    /// </summary>
    private void Awake()
    {
        GridManager.SetGrid(gridDimensions[gridIndex]);
        gridPrefabs[gridIndex].SetActive(true);
    }

    /// <summary>
    /// Loads the next combat grid when the previous combat is over
    /// </summary>
    public void LoadNextGrid()
    {
        gridIndex = gridIndex + 1 < gridDimensions.Count ? ++gridIndex : gridIndex;
        GridManager.SetGrid(gridDimensions[gridIndex]);
        LoadGridPrefab();
    }

    /// <summary>
    /// Switches out the previous grid prefab for the new one being loaded
    /// </summary>
    private void LoadGridPrefab()
    {
        foreach(GameObject g in gridPrefabs)
        {
            g.SetActive(false);
        }
        gridPrefabs[gridIndex].SetActive(true);
    }
}
