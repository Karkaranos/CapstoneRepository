using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class GridTesting : MonoBehaviour
{
    GameObject player;
    public Vector2Int gridSize;
    
    [Button]
    void SpawnPlayer()
    {
        player = GridManager.CreateObject(Resources.Load<GameObject>("Player"));
    }
    [Button]
    void SpawnEnemy()
    {
        GridManager.CreateObject(Resources.Load<GameObject>("Enemy"));
    }
    [Button]
    void SpawnObsticle()
    {
        GridManager.CreateObject(Resources.Load<GameObject>("Obsticle"));
    }
    [Button]
    void ClearGrid()
    {
        GridManager.currentGrid.ClearGrid();
    }
    [Button]
    void FindPlayerTile()
    {
        print(GridManager.GetObject(player).coordinate);
    }
    [Button]
    void FindEnemyTiles()
    {
        List<Tile> enemyTiles = GridManager.GetObjectsWithTag("Enemy");
        string s = "";
        foreach (Tile tile in enemyTiles) { 
            s += tile.coordinate + "  ";
        }
        print(s);
    }
    [Button]
    void RemoveEnemies()
    {
        List<Tile> enemyTiles = GridManager.GetObjectsWithTag("Enemy");
        foreach (Tile tile in enemyTiles)
        {
            GridManager.RemoveObject(tile.objectOnTile);
        }
    }


    private void Start()
    {
        GenerateGrid();
    }
    public void GenerateGrid() {
        GridManager.MakeGrid("Grid1", gridSize, 1, Vector3.zero);
    }
}
