/*************************************************
Author Names : 		    Aidan Ratcliffe
Date Created : 		    10/1/2025
Date Last Modified : 	10/2/2025
Brief Description : 	This how the player will detect where the grid is
External Resources : 	N/A
***************************************************/
using NUnit.Framework;
using PlayerInputActions;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerBehavior : GridPathfinding
{
    public Input playerInput;
    [SerializeField] private InputAction clickAction;
    public List<Vector3> playergridPoints;
    public GameObject player;
    public GameObject gridTile;
    private Vector2 playerPosition;
    public Vector2Int tilePosition;
    private TileBehavior tileBehavior;
    private GridManager gridManager;
    public bool PlayerCanMove;
    public bool PlayerHasMoved;
    public bool MouseIsClicked;
    //public bool PlayerCanAttack;
    //public bool PlayerHasAttacked;

    Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        playerPosition = new Vector2Int(1, 1);
        tilePosition = GetComponent<TileBehaviour>().IndexInGrid;
    }

    void OnEnable()
    {
        clickAction.Enable();
        clickAction.performed += OnClickPerformed;
    }

    void OnDisable()
    {
        clickAction.Disable();
        clickAction.performed -= OnClickPerformed;
    }

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        
    }

    private void fixedUpdate()
    {
        if (MouseIsClicked)
        {
           
        }
    }
}
