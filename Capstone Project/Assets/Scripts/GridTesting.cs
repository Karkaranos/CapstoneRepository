/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 9/26/2025
 * Last Modified: 9/26/2025
 * Brief: Temporary script to test if the grid works. Non-temporary scripts 
 * should be added to the game manager after this is added to working
 * ***************************************************************************/
using UnityEngine;
using NaughtyAttributes;

public class GridTesting : MonoBehaviour
{
    [SerializeField] private Vector2Int gridDimensions;

    [Button("Populate Grid")]

    private void PopulateGrid()
    {
        GridManager.SetGrid(gridDimensions);
    }

    [Button("Show Grid In Console")]

    private void ShowGrid()
    {
        GridManager.DisplayGridAsText();
    }
}
