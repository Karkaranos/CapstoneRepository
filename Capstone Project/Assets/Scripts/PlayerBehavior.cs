using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class PlayerBehavior : MonoBehaviour
{
    public GameObject player;
    public GameObject gridTile;
    public List<Vector2Int> gridPoints = new List<Vector2Int>();
    public List<Vector2Int> playergridPoints = new List<Vector2Int>();
    private Vector2Int playerPosition;
    private Vector2 tilePosition;
    private TileBehavior tileBehavior;
    private GridManager gridManager;
    public bool PlayerCanMove;
    public bool PlayerHasMoved;
    //public bool PlayerCanAttack;
    //public bool PlayerHasAttacked;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManager = gridManager.gameObject.GetComponent<GridManager>();
        player = player.gameObject.GetComponent<GameObject>();
        tilePosition = tileBehavior.TileIntPosition;
    }


    public void GetTilePositions()
    {
        gridPoints.Add(new Vector2Int(0, 0));
        gridPoints.Add(new Vector2Int(1, 0));
        gridPoints.Add(new Vector2Int(2, 0));
        gridPoints.Add(new Vector2Int(1, 1));
        gridPoints.Add(new Vector2Int(2, 1));
        gridPoints.Add(new Vector2Int(3, 1));
        gridPoints.Add(new Vector2Int(0, 2));
        gridPoints.Add(new Vector2Int(1, 2));
        gridPoints.Add(new Vector2Int(2, 2));
        gridPoints.Add(new Vector2Int(1, 3));
        gridPoints.Add(new Vector2Int(2, 3));
        gridPoints.Add(new Vector2Int(3, 3));
    }

    public void GetPlayerGridTilePosition()
    {
        playergridPoints.Add(new Vector2Int(1, 0));
        playergridPoints.Add(new Vector2Int(2, 0));
        playergridPoints.Add(new Vector2Int(1, 1));
        playergridPoints.Add(new Vector2Int(2, 1));
        playergridPoints.Add(new Vector2Int(3, 1));
        playergridPoints.Add(new Vector2Int(0, 2));
        playergridPoints.Add(new Vector2Int(1, 2));
        playergridPoints.Add(new Vector2Int(1, 3));
    }
}
