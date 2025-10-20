using UnityEngine;

public class TileBehavior : MonoBehaviour
{
    public Vector2Int TileIntPosition;
    [SerializeField]
    private Vector2 TilePlacement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TileIntPosition.x = Mathf.CeilToInt(transform.position.x / TilePlacement.x);
        TileIntPosition.y = Mathf.CeilToInt(transform.position.z / TilePlacement.y);
    }
}
