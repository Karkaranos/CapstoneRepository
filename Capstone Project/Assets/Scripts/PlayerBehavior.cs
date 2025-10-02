using Unity.VisualScripting;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public GameObject player;
    private Vector3 playerPosition;
    private Vector2 tilePosition;
    private TileBehavior tileBehavior;
    private GridManager gridManager;
    public bool PlayerCanMove;
    public bool PlayerHasMoved;
    //public bool PlayerCanAttack;
    //public bool PlayerHasAttacked;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManager = gridManager.gameObject.GetComponent<GridManager>();
        player = player.gameObject.GetComponent<GameObject>();
        tilePosition = tileBehavior.TileIntPosition;
        playerPosition = player.gameObject.transform.position;
    }

    void OnMouseDown()
    {
        PlayerCanMove = true;
        if(PlayerCanMove == true)
        {
           playerPosition = tilePosition;
        }
        else
        {
            PlayerCanMove = false;
        }
    }
}
