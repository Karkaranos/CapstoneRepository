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
using System.Threading.Tasks;
using System.ComponentModel;

public class PipManager : MonoBehaviour
{
    private int currentPipsOnField;

    [System.Serializable]
    public class SpawnLocations
    {
        public List<TileBehaviour> spawningLocations;
    }

    [SerializeField] private int maxNumberOfPipsOnField;
    [SerializeField] private List<TileBehaviour> spawnableTiles;
    [SerializeField] private GameObject pip;
    [SerializeField] private List<SpawnLocations> spawningLocationsPerLevel;

    [SerializeField] int currentLevel = 0;

    [HideInInspector] public PipManager Instance; 

    /// <summary>
    /// Ensure singleton
    /// Sets currentPipsOnField to 0
    /// Spawn pips 
    /// </summary>
    private async void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        currentLevel = 1; 
        currentPipsOnField = 0;
        await Task.Delay(25);
        SpawnPips();
    }

    /// <summary>
    /// Function to spawn pips
    /// uses a temp array to make sure we don't get index out of bounds erros 
    /// </summary>
    public void SpawnPips()
    {
        List<TileBehaviour> temp = new List<TileBehaviour>(); 
        while(currentPipsOnField < maxNumberOfPipsOnField)
        {
            int index = Random.Range(0, spawnableTiles.Count);
            if(!GridManager.TileIsEmpty(spawnableTiles[index].IndexInGrid))
            {
                continue; 
            }
            temp.Add(spawnableTiles[index]);
            spawnableTiles[index].AddPip(pip);
            spawnableTiles.Remove(spawnableTiles[index]);
            
            ++currentPipsOnField;
        }

        foreach (TileBehaviour tileBehaviour in spawnableTiles)
        {
            temp.Add(tileBehaviour);
        }
        spawnableTiles = temp; 
    }
}
