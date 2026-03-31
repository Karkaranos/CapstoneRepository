/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		2/12/2026
Date Last Modified : 	2/12/2026
Brief Description : 		Handles collection of pips 
External Resources : 	
***************************************************/
using UnityEngine;

public class Pip : MonoBehaviour
{
    [SerializeField] private int pipsGrantedOnPickup = 1;

    private GameManager gameManager;

    public TileBehaviour tile; 
    /// <summary>
    /// If collider is player destory pip 
    /// Change current Pip on field count
    /// Increment action points
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        RemovePip();
    }

    /// <summary>
    /// Remove pip from field
    /// </summary>
    public void RemovePip()
    {
        --PipManager.Instance.currentPipsOnField;
        gameManager.IncrementActionPoints(pipsGrantedOnPickup);
        GridManager.combatGrid[tile.IndexInGrid.x, tile.IndexInGrid.y].entityOnGrid = -1;
        Destroy(this.gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
