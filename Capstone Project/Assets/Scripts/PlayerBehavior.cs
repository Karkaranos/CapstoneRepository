using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class PlayerBehavior : MonoBehaviour
{
    public GameObject player;
    public GameObject gridTile;
    public List<Vector3Int> gridPoints = new List<Vector3Int>();
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


    //void GetPlayerTilePositions()
    //{
        
    //}
}
