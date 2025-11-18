/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		11/18/2025
Date Last Modified : 	11/18/2025
Brief Description : 		Base class for Range enemies
                    This is a seperate class from Enemy for 
                 sublogic of each enemy. 
External Resources : 	
***************************************************/
using UnityEngine;
using NaughtyAttributes;

public class RangedEnemy : Enemy
{
    #region VARS

    #region COMBAT VARS
    [Header("Ranged Enemy Specfic")]
    [ShowIf(nameof(currentSettings), Settings.Combat),
        SerializeField,
        Tooltip("The minimum amount of tiles away from the enemy the player must be to be attacked")]
    private int minimumAttackDistance;

    #endregion

    #region TEST VARS
    #endregion

    #region STATE MACHINE VARS
    #endregion

    #region OTHER NON INSPECTOR VARS 
    #endregion

    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
