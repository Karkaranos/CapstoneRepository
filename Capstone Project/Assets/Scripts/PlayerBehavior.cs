/*************************************************
Author Names : 		    Aidan Ratcliffe
Date Created : 		    10/1/2025
Date Last Modified : 	10/8/2025
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
    #region player variables
    [Tooltip("reference to the player movement and its actions")]
    public Input playerInput;
    [SerializeField] private InputAction playerClick;
    [SerializeField] private InputAction playermoveClick;

    [Tooltip("references the player's game object")]
    public GameObject player;

    [Tooltip("Player Position and the position it wants to go to")]
    private Vector2Int playerPosition;
    public Vector2Int targetPosition;

    [Tooltip("Scripts the playerbehavior is deriving from")]
    private TileBehavior tileBehavior;
    private GridManager gridManager;
    private ButtonManager buttonManager;

    [Tooltip("bool to check to see if the mouse input is activated")]
    public bool MouseIsClicked;
    #endregion playervariables

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //Sets player position and target position to reference the grid manager's player position and
    //target position to the TileBehaviour Index 
    void Start()
    {
        buttonManager = FindFirstObjectByType<ButtonManager>();
        playerPosition = new Vector2Int(GridManager.playerPosition.x, GridManager.playerPosition.y);
        targetPosition = GridManager.playerPosition;
    }

    #region player input
    public void OnEnable()
    {
        playerClick.Enable();
        playermoveClick.Enable();
        playermoveClick.started += playermoveClickPerformed;
    }

    //Sets the boolean to true when left mouse button is clicked
    private void playermoveClickPerformed(InputAction.CallbackContext context)
    {
        MouseIsClicked = true;
    }

    void OnDisable()
    {
        playerClick.Disable();
        playermoveClick.Disable();
        playermoveClick.started -= playermoveClickPerformed;
    }

    /// <summary>
    /// Sends a raycast from the where mouse clicks to the points on the grid.
    /// Reads the mouse input that sets the MouseIsClicked bool to true, allowing 
    /// the raycast to hit the player's desired target position on the grid.
    /// </summary>
    private void FixedUpdate()
    {
        if (MouseIsClicked)
        {
            Ray ray = Camera.main.ScreenPointToRay(playerClick.ReadValue<Vector2>());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 temp = hit.transform.gameObject.transform.position;
                Vector2Int temp2 = targetPosition;
                targetPosition = hit.transform.gameObject.GetComponentInParent<TileBehaviour>().IndexInGrid;
                gameObject.transform.position = temp;
                GridManager.MoveToTile(temp2, targetPosition, -3);
                buttonManager.confirmCanvas.SetActive(true);
            }
            MouseIsClicked = false;
        }
    }
    #endregion
}
