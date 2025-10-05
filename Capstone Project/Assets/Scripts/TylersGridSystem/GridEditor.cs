using NaughtyAttributes;
using UnityEngine;

public class GridEditor : MonoBehaviour
{
    [SerializeField] private GridData gird;
    [SerializeField] private Vector2Int girdDimensions;
    [SerializeField] private float HexRadius;
    [SerializeField] private Vector3 grodLocation;

    [Button]
    void UpdateAndSaveGrid() {
        TylersGridManager.MakeGrid(grid);
    }
}
