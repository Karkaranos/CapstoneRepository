/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		2/10/2026
Date Last Modified : 	2/10/2026
Brief Description : 		Ranged enemy move state
External Resources : 	
***************************************************/
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.VFX;

public class PipManager : MonoBehaviour
{
    private int currentPipsOnField;

    [SerializeField] private int maxNumberOfPipsOnField;
    [SerializeField] private List<TileBehaviour> spawnableTiles;
    [SerializeField] private GameObject pip;

    [HideInInspector] public PipManager Instance; 

    /// <summary>
    /// Ensure singleton
    /// Sets currentPipsOnField to 0
    /// Spawn pips 
    /// </summary>
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        currentPipsOnField = 0;
        SpawnPips();
    }

    public void SpawnPips()
    {
        while(currentPipsOnField < maxNumberOfPipsOnField)
        {
            int index = Random.Range(0, spawnableTiles.Count);
            if(!GridManager.TileIsEmpty(spawnableTiles[index].IndexInGrid))
            {
                continue; 
            }
            spawnableTiles[index].AddPip(pip);
            ++currentPipsOnField;
        }
    }
}
