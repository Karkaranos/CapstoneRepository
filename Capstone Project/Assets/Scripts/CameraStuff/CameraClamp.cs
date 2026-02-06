using NaughtyAttributes;
using UnityEngine;
public class CameraClamp : MonoBehaviour
{

    #region variables
    private enum Cameras
    {
        Cameras,
        Refs
    }

    public Input playerInput;
    [SerializeField] private Cameras Cams;
    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] private Transform CameraTarget;
    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] Vector2 minPos;
    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] Vector2 maxPos;
    #endregion

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(CameraTarget.position.x, minPos.x, maxPos.x), 
            Mathf.Clamp(CameraTarget.position.y, minPos.y, maxPos.y), CameraTarget.position.z);
    }
}
