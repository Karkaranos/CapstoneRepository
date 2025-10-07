/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 9/26/2025
 * Last Modified: 9/30/2025
 * Brief: Temporary script to test if the grid works. Non-temporary scripts 
 * should be added to the game manager after this is added to working
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Collections;

public class GridTesting : MonoBehaviour
{
    private enum GridSettings
    {
        GridLoading,
        GridMovement
    }

    [SerializeField] private GridSettings selectedSetting;

    #region Grid variables
    [HorizontalLine(4, EColor.Red)]

    [Tooltip("Used to determine which grid is being used")]
    [ShowIf(nameof(selectedSetting), GridSettings.GridLoading), SerializeField]
    private int gridIndex;

    [Tooltip("The list of the different combat grids")]
    [ShowIf(nameof(selectedSetting), GridSettings.GridLoading), SerializeField] 
    private List<GameObject> gridPrefabs = new List<GameObject>();

    [Tooltip("The list that contains how big each grid is")]
    [ShowIf(nameof(selectedSetting), GridSettings.GridLoading), SerializeField] 
    private List<Vector2Int> gridDimensions = new List<Vector2Int>();
    #endregion

    #region Grid movement variables
    [HorizontalLine(4, EColor.Blue)]

    //Temporary variables for testing purposes
    [Tooltip("The tile that would be where an object starts from")]
    [ShowIf(nameof(selectedSetting), GridSettings.GridMovement), SerializeField] 
    private Vector2Int currentMovementTestingTile;

    [Tooltip("The tile that on object would move to")]
    [ShowIf(nameof(selectedSetting), GridSettings.GridMovement), SerializeField] 
    private Vector2Int newMovementTestingTile;
    #endregion

    #region Buttons
    //[HorizontalLine(4, EColor.Green)]
    //[Space]
    /// <summary>
    /// Testing button that shows the grid in the console
    /// </summary>
    [Button("Show Grid In Console")]
    private void ShowGrid()
    {
        GridManager.DisplayGridAsText();
    }

    /// <summary>
    /// Testing button for loading the next grid
    /// </summary>
    [Button("Load Next Grid")]
    private void LoadGrid()
    {
        LoadNextGrid();
    }

    /// <summary>
    /// Testing button used to test movement in the grid and shows it in the console
    /// </summary>
    [Button("Tests Movement In Grid")]
    private void TestMovement()
    {
        GridManager.AddEntity(currentMovementTestingTile, -8);
        GridManager.DisplayGridAsText();
        if (GridManager.TileIsInGrid(newMovementTestingTile) && GridManager.CanMoveToTile(newMovementTestingTile))
        {
            GridManager.MoveToTile(currentMovementTestingTile, newMovementTestingTile, -8);
        }
        GridManager.DisplayGridAsText();
    }

    [Button("Show Terrain Affects")]
    private void DisplayTerrainAffectsInConsole()
    {
        TileStatTester[] testers = FindObjectsByType<TileStatTester>(FindObjectsSortMode.None);
        foreach(TileStatTester t in testers)
        {
            t.DisplayStatChange();
        }
    }

    [Button("Test Pathfinding")]
    private void Pathfind()
    {
        StartCoroutine(AllEnemiesPathfind());
        //GridPathfinding[] enemies = FindObjectsByType<GridPathfinding>(FindObjectsSortMode.None);
        //foreach(GridPathfinding e in enemies)
        //{
        //    e.TestPathfinding();
        //}
    }
    #endregion

    IEnumerator AllEnemiesPathfind()
    {
        GridPathfinding[] enemies = FindObjectsByType<GridPathfinding>(FindObjectsSortMode.None);
        int listIndex = 0;

        while(listIndex < enemies.Length)
        {
            enemies[listIndex].TestPathfinding();
            yield return new WaitForSeconds(5);
            ++listIndex;
        }
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
