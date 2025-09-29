using NaughtyAttributes;
using UnityEngine;

public class GridTesting : MonoBehaviour
{
    
    [Button]
    void SpawnPlayer()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("Player"));
        GridManager.currentGrid.AddObjectToGrid(obj);
    }
    [Button]
    void SpawnEnemy()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("Enemy"));
        GridManager.currentGrid.AddObjectToGrid(obj);
    }
    [Button]
    void SpawnObsticle()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("Obsticle"));
        GridManager.currentGrid.AddObjectToGrid(obj);
    }


    private void Start()
    {
        GenerateGrid();
    }
    public void GenerateGrid() {
        GridManager.MakeGrid("Grid1", new Vector2Int(10,10), 1, Vector3.zero);
    }
}
