using UnityEngine;
using System.Collections;

public class GridMovement : MonoBehaviour
{
    public Tile currentTile;
    public bool moving;
    public float moveTime = 0.3f; 

    void Start()
    {
        currentTile = TylersGridManager.GetTileWithObject(gameObject);
    }

    public void MoveTo(Tile neighbor)
    {
        if (neighbor != null && neighbor.isEmpty() && !moving)
        {
            currentTile.objectOnTile = null;
            currentTile = neighbor;
            currentTile.objectOnTile = gameObject;
            StartCoroutine(SmoothMove(transform.position, currentTile.worldPosition));
        }
        else
        {
            // print("Can't move to " + neighbor.coordinate);
        }
    }

    private IEnumerator SmoothMove(Vector3 startPosition, Vector3 targetPos)
    {
        moving = true;
        float endTime = Time.time + moveTime;

        while (Time.time < endTime)
        {
            float elapsedTime = moveTime - (endTime - Time.time);
            float percentage = elapsedTime / moveTime;
            transform.position = Vector3.Lerp(startPosition, targetPos, percentage);
            yield return null;
        }

        transform.position = targetPos;
        moving = false;
    }
}
