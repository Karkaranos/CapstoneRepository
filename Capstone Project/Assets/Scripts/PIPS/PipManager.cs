/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		2/10/2026
Date Last Modified : 	2/10/2026
Brief Description : 		Pip system manager
External Resources : 	
***************************************************/
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.VFX;
using System.Threading.Tasks;
using System.ComponentModel;

public class PipManager : MonoBehaviour
{

    [System.Serializable]
    public class SpawnLocations
    {
        public List<TileBehaviour> spawningLocations;
    }

    [SerializeField] private int maxNumberOfPipsOnField;
    [SerializeField] private List<TileBehaviour> currentSpawnableTiles;
    [SerializeField] private GameObject pip;
    [SerializeField] private List<SpawnLocations> spawningLocationsPerLevel;

    [SerializeField] int currentLevel = 0;

    [HideInInspector] public static PipManager Instance { get; private set; }
    [HideInInspector] public int currentPipsOnField;

    private void OnEnable()
    {
        TurnPublicEvents.BeginPlayerTurn += SpawnPips;
        PublicEvents.LoadingGrid += SetCurrentSpawnLocations;
    }

    private void OnDisable()
    {
        TurnPublicEvents.BeginPlayerTurn -= SpawnPips;
        PublicEvents.LoadingGrid -= SetCurrentSpawnLocations;
    }
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
        currentLevel = 0; 
        currentPipsOnField = 0;
        currentSpawnableTiles = spawningLocationsPerLevel[0].spawningLocations;
    }

    /// <summary>
    /// Function to spawn pips
    /// uses a temp array to make sure we don't get index out of bounds erros 
    /// </summary>
    public void SpawnPips()
    {
        Debug.Log("SPAWNING PIPS");
        List<TileBehaviour> temp = new List<TileBehaviour>(); 
        while(currentPipsOnField < maxNumberOfPipsOnField)
        {
            int index = Random.Range(0, currentSpawnableTiles.Count);
            if(!GridManager.TileIsEmpty(currentSpawnableTiles[index].IndexInGrid))
            {
                continue; 
            }
            temp.Add(currentSpawnableTiles[index]);
            currentSpawnableTiles[index].AddPip(pip);
            currentSpawnableTiles.Remove(currentSpawnableTiles[index]);
            
            ++currentPipsOnField;
        }

        foreach (TileBehaviour tileBehaviour in currentSpawnableTiles)
        {
            temp.Add(tileBehaviour);
        }
        currentSpawnableTiles = temp;
        spawningLocationsPerLevel[currentLevel].spawningLocations = currentSpawnableTiles;
        TurnPublicEvents.TurnActionComplete?.Invoke(); 
    }

    private async void SetCurrentSpawnLocations(int i)
    {
        spawningLocationsPerLevel[currentLevel].spawningLocations = currentSpawnableTiles;
        currentLevel = i; 
        currentSpawnableTiles = spawningLocationsPerLevel[i].spawningLocations;
        currentPipsOnField = 0;
        await Task.Delay(500);
        SpawnPips();
    }
}
