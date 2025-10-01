/*************************************************
Author Name(s) : 		Bouchard, Tyler
Date Created : 		    9/25/2025
Date Last Modified : 	9/30/2025
Brief Description :     This class has a few functions that test the grid manager, it isnt 
                        really meant to be used in game
External Resources : 	
	***************************************************/
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
        GridManager.ClearGrid();
    }
    [Button]
    void FindPlayerTile()
    {
        print(GridManager.GetTileWithObject(player).coordinate);
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
    [Button]
    void NextToPlayer()
    {
        List<Tile> adjacentTiles = GridManager.GetAllEmptyNeighbors(GridManager.GetTileWithObject(player));
        string s = "";

        foreach (Tile tile in adjacentTiles)
        {
            s += tile.coordinate + "  ";
        }
        print(s);
    }
    [Button]
    void DestroyGrid()
    {
        GridManager.DestroyGrid();
    }

    [Button]
    void GenerateGrid() {
        GridManager.MakeGrid(gridSize, 1, Vector3.zero);
    }


}
