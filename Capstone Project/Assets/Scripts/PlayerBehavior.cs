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

public class PlayerBehavior : MonoBehaviour
{
    public Input playerInput;
    [SerializeField] private InputAction playerClick;
    [SerializeField] private InputAction playermoveClick;
    public List<Vector2> playergridPoints;
    public GameObject player;
    public GameObject gridTile;
    private Vector2Int playerPosition;
    public Vector2Int tilePosition;
    public Vector2Int targetPosition;
    private TileBehavior tileBehavior;
    private GridPathfinding gridPathfinding;
    private GridManager gridManager;
    public bool PlayerCanMove;
    public bool PlayerHasMoved;
    public bool MouseIsClicked;
    //public bool PlayerCanAttack;
    //public bool PlayerHasAttacked;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerPosition = new Vector2Int(GridManager.playerPosition.x, GridManager.playerPosition.y);
        targetPosition = GetComponentInParent<TileBehaviour>().IndexInGrid;
    }

    void OnEnable()
    {
        playerClick.Enable();
        playermoveClick.Enable();
        playermoveClick.started += playermoveClickPerformed;
        //playerClick.performed += PlayerClickPerformed;
    }

    private void playermoveClickPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("I'm being called");
        MouseIsClicked = true;
    }

    void OnDisable()
    {
        Debug.Log("I'm being called early, grrr");
        playerClick.Disable();
        playermoveClick.Disable();
        //playerClick.started -= PlayerClickPerformed;
    }

    private void FixedUpdate()
    {
        if (MouseIsClicked)
        {
            Ray ray = Camera.main.ScreenPointToRay(playerClick.ReadValue<Vector2>());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(targetPosition);
                Vector3 temp = hit.transform.gameObject.transform.position;
                Vector2Int temp2 = targetPosition;
                targetPosition = hit.transform.gameObject.GetComponentInParent<TileBehaviour>().IndexInGrid; /*new Vector2Int((int)temp.x, (int)temp.z);*/
                gameObject.transform.position = temp;
                GridManager.MoveToTile(temp2, targetPosition, -3);
            }
            MouseIsClicked = false;
        }
        else
        {
            MouseIsClicked = false;
        }
       
    }

    public void GetTargetPosition(Vector2Int newTarget)
    {
        ///Sets the target position to where the player wants to go
        targetPosition = newTarget;

    }

    public void PlayerMove(Vector2Int newTarget)
    {
        ///Sets the player's position to the new target position
        playerPosition = newTarget;
    }
}
