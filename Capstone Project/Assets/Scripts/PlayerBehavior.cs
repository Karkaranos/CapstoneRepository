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
using System;
using UnityEngine.AI;

public class PlayerBehavior : MonoBehaviour
{
    public Input playerInput;
    [SerializeField] private InputAction clickAction;
    public List<Vector3> playergridPoints;
    private int currentIndex;
    public float moveSpeed = 5f;
    public NavMeshAgent agent;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        playerPosition = new Vector2Int(1, 1);
        agent = GetComponent<NavMeshAgent>();
        //tilePosition = GetComponent<TileBehaviour>().IndexInGrid;
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
        MouseIsClicked = true;
    }

    private void Update()
    {
        if (MouseIsClicked)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    currentIndex++;
                    if (currentIndex >= playergridPoints.Count)
                    {
                        currentIndex = 0;
                    }
                }
            }
            if (playergridPoints.Count > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, playergridPoints[currentIndex], moveSpeed * Time.deltaTime);
            }
        }
    }


    private void UpdateGridPosition()
    {

    }

    //public void GetPlayerGridTilePosition()
    //{
    //    playergridPoints.Add(new Vector2(1, 0));
    //    playergridPoints.Add(new Vector2(2, 0));
    //    playergridPoints.Add(new Vector2(1, 1));
    //    playergridPoints.Add(new Vector2(2, 1));
    //    playergridPoints.Add(new Vector2(3, 1));
    //    playergridPoints.Add(new Vector2(0, 2));
    //    playergridPoints.Add(new Vector2(1, 2));
    //    playergridPoints.Add(new Vector2(1, 3));
    //}

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



}
