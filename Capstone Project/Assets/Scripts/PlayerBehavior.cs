using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public GameObject player;
    public Vector3 playerPosition;
    public Vector3 tilePosition;
    public GridManager gridManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManager = gridManager.gameObject.GetComponent<GridManager>();
        player = player.gameObject.GetComponent<GameObject>();
        playerPosition = tilePosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
