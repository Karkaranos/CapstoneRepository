/*************************************************
Author Names : 		    Aidan Ratcliffe
Date Created : 		    10/1/2025
Date Last Modified : 	10/2/2025
Brief Description : 	This how the player will detect where the grid is
External Resources : 	N/A
***************************************************/
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using PlayerInputActions;

public class PlayerBehavior : MonoBehaviour
{
    private InputSystemActions playerInput;
    public GameObject player;
    public GameObject gridTile;
    private Vector2Int playerPosition;
    private Vector2Int gridPosition;
    private TileBehavior tileBehavior;
    private GridManager gridManager;
    public bool PlayerCanMove;
    public bool PlayerHasMoved;
    public bool MouseIsClicked;
    //public bool PlayerCanAttack;
    //public bool PlayerHasAttacked;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        playerPosition = new Vector2Int(1, 1);
        playerInput = new InputSystemActions();
        gridPosition = GetComponent<TileBehaviour>().IndexInGrid;
    }

    private void Update()
    {
        
    }

    public void PlayerWillMove(Vector2Int direction)
    {
        Vector2Int newPos = playerPosition + direction;


        if (MouseIsClicked)
        {

            playerPosition = newPos;

            player.transform.position = new Vector2(newPos.x, newPos.y);
        }
    }


    private void UpdateGridPosition()
    {

    }

    //public void GetTilePositions()
    //{
    //    gridPoints.Add(new Vector2Int(0, 0));
    //    gridPoints.Add(new Vector2Int(1, 0));
    //    gridPoints.Add(new Vector2Int(2, 0));
    //    gridPoints.Add(new Vector2Int(1, 1));
    //    gridPoints.Add(new Vector2Int(2, 1));
    //    gridPoints.Add(new Vector2Int(3, 1));
    //    gridPoints.Add(new Vector2Int(0, 2));
    //    gridPoints.Add(new Vector2Int(1, 2));
    //    gridPoints.Add(new Vector2Int(2, 2));
    //    gridPoints.Add(new Vector2Int(1, 3));
    //    gridPoints.Add(new Vector2Int(2, 3));
    //    gridPoints.Add(new Vector2Int(3, 3));
    //}

    //public void GetPlayerGridTilePosition()
    //{
    //    playergridPoints.Add(new Vector2Int(1, 0));
    //    playergridPoints.Add(new Vector2Int(2, 0));
    //    playergridPoints.Add(new Vector2Int(1, 1));
    //    playergridPoints.Add(new Vector2Int(2, 1));
    //    playergridPoints.Add(new Vector2Int(3, 1));
    //    playergridPoints.Add(new Vector2Int(0, 2));
    //    playergridPoints.Add(new Vector2Int(1, 2));
    //    playergridPoints.Add(new Vector2Int(1, 3));
    //}


    void OnEnable()
    {
        playerInput.Enable();
    }

    void OnDisable()
    {
        playerInput.Disable();
    }
}
