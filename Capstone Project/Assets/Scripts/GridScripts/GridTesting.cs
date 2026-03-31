/******************************************************************************
 * Author: Brad Dixon, Cade Naylor
 * Creation Date: 9/26/2025
 * Last Modified: 3/10/2026 (Brad Dixon)
 * Brief: Controls grid loading and handling
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Collections;

public class GridTesting : MonoBehaviour
{
    #region Grid variables
    [HorizontalLine(4, EColor.Red)]

    [Tooltip("Used to determine which grid is being used")]
    [SerializeField] private int gridIndex;

    [Tooltip("The list of the different combat grids")]
    [SerializeField] private List<GameObject> gridPrefabs = new List<GameObject>();

    [Tooltip("The list that contains how big each grid is")]
    [SerializeField] private List<Vector2Int> gridDimensions = new List<Vector2Int>();

    private List<GameObject> entityLists = new List<GameObject>();

    public int gridToLoad;
    public PopUpScript popUp;
    #endregion

    #region Buttons
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
        LoadSpecificGrid(gridToLoad+1);
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
        PipManager.Instance.hazardTiles.Clear();
        LoadGridPrefab();
        GridManager.SetGrid(gridDimensions[gridIndex], gridPrefabs[gridIndex]);
        PublicEvents.NewLevel.Invoke();
    }

    /// <summary>
    /// Loads a specific grid
    /// </summary>
    /// <param name="i"></param>
    public void LoadSpecificGrid(int i) 
    {
        if(i > gridPrefabs.Count || i <= 0)
        {
            Logger.Error("Invalid grid index entered. Returning function", true);
            return;
        }
        gridIndex = i;
        PipManager.Instance.hazardTiles.Clear();
        LoadGridPrefab();
        GridManager.SetGrid(gridDimensions[gridIndex], gridPrefabs[gridIndex]);
        PublicEvents.NewLevel.Invoke();
    }

    /// <summary>
    /// Reloads the current grid
    /// </summary>
    public void ReloadCurrentGrid()
    {
        LoadGridPrefab();
        GridManager.SetGrid(gridDimensions[gridIndex], gridPrefabs[gridIndex]);
        PublicEvents.NewLevel.Invoke();
    }

    /// <summary>
    /// Switches out the previous grid prefab for the new one being loaded
    /// </summary>
    private void LoadGridPrefab()
    {
        ClearEntityList();

        TileBehaviour[] waterTiles = FindObjectsByType<TileBehaviour>(FindObjectsSortMode.None);
        foreach(TileBehaviour t in waterTiles)
        {
            if(t.isElectrified)
            {
                t.DelectrifyTile();
            }
        }
        foreach(GameObject g in gridPrefabs)
        {
            g.SetActive(false);
        }
        gridPrefabs[gridIndex].SetActive(true);
        PublicEvents.LoadingGrid.Invoke(gridIndex);
        popUp.Flip();
    }

    /// <summary>
    /// Adds entities to a list that is used for deleting them on level loading
    /// </summary>
    /// <param name="g"></param>
    public void AddEntityToList(GameObject g)
    {
        entityLists.Add(g);
    }

    /// <summary>
    /// Deletes all entities than resets the list
    /// </summary>
    private void ClearEntityList()
    {
        foreach(GameObject g in entityLists)
        {
            Destroy(g);
        }
        entityLists.Clear();
    }
}
