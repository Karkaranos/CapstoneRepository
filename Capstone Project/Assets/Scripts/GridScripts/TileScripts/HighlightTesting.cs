/******************************************************************************
 * Author: Tyler Bouchard
 * Creation Date: 11/20/2025
 * Last Modified: 11/20/2025 (Tyler Bouchard)
 * Brief: calls highlight finctions in the grid manager to test the tile highlight
 * External Resources: N/A
 * ***************************************************************************/
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

    /// <summary>
    /// calls HiglightTilesInRange with the variables that you wanted to test
    /// </summary>
    [Button]
    public void ShowHighlight() {
        GridManager.HiglightTilesInRange(orginTile, highlightRange, highlightColor);
    }

    /// <summary>
    /// calls GridManager.RemoveHighlight()
    /// </summary>
    [Button]
    public void ClearHighlight()
    {
        GridManager.RemoveHighlight();
    }
}
