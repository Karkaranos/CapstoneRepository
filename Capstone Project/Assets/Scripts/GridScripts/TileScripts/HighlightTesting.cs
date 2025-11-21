using UnityEngine;
using NaughtyAttributes;
public class HighlightTesting : MonoBehaviour
{
    public GameObject grid;
    public Vector2Int gridSize;

    public TileBehaviour orginTile;
    public Color highlightColor;
    public int highlightRange;

    private void Start()
    {
        GridManager.SetGrid(gridSize, grid);
    }

    [Button]
    public void ShowHighlight() {
        GridManager.HiglightTilesInRange(orginTile, highlightRange, highlightColor);
    }

    [Button]
    public void ClearHighlight()
    {
        GridManager.RemoveHighlight();
    }
}
