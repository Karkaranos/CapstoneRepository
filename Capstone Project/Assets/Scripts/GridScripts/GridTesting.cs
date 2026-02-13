/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 9/26/2025
 * Last Modified: 10/30/2025
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

    public int gridToLoad;
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
    /// Testing button that shows in the console which stats are being affected and by how much
    /// </summary>
    [Button("Show Terrain Affects")]
    private void DisplayTerrainAffectsInConsole()
    {
        TileStatTester[] testers = FindObjectsByType<TileStatTester>(FindObjectsSortMode.None);
        foreach(TileStatTester t in testers)
        {
            t.DisplayStatChange();
        }
    }

    /// <summary>
    /// Testing button that tells the enemies to move
    /// </summary>
    [Button("Test Pathfinding")]
     public void Pathfind()
    {
        StartCoroutine(AllEnemiesPathfind());
    }

    [Button]
    public void LoadGridAtIndex()
    {
        LoadSpecificGrid(gridToLoad);
    }
    #endregion

    /// <summary>
    /// Temporary script that allows enemies to move one at a time
    /// </summary>
    /// <returns></returns>
    IEnumerator AllEnemiesPathfind()
    {
        GridPathfinding[] enemies = FindObjectsByType<GridPathfinding>(FindObjectsSortMode.None);
        int listIndex = 0;

        while (listIndex < enemies.Length)
        {
            enemies[listIndex].SetAggroRange(10);
            enemies[listIndex].TestPathfinding();
            yield return new WaitForSeconds(5);
            ++listIndex;
        }
    }

    /// <summary>
    /// Creates an instance of the grid
    /// </summary>
    private void Start()
    {
        gridPrefabs[gridIndex].SetActive(true);
        GridManager.SetGrid(gridDimensions[gridIndex], gridPrefabs[gridIndex]);
    }

    /// <summary>
    /// Loads the next combat grid when the previous combat is over
    /// </summary>
    public void LoadNextGrid()
    {

        gridIndex = gridIndex + 1 < gridDimensions.Count ? ++gridIndex : gridIndex;
        LoadGridPrefab();
        GridManager.SetGrid(gridDimensions[gridIndex], gridPrefabs[gridIndex]);
        
    }

    public void LoadSpecificGrid(int i) 
    {
        gridIndex = i;
        LoadGridPrefab();
        GridManager.SetGrid(gridDimensions[gridIndex], gridPrefabs[gridIndex]);
       
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
