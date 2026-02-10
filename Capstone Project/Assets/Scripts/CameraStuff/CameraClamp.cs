/*************************************************
Author Names : 		    Aidan Ratcliffe
Date Created : 		    2/2/2026
Date Last Modified : 	2/6/2026 
Brief Description : 	This is here just in case I need to clamp the camera
External Resources : 	N/A
***************************************************/
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

    #region functions
    // Update is called once per frame
    /// <summary>
    /// Clamps the camera's x and y position
    /// </summary>
    void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(CameraTarget.position.x, minPos.x, maxPos.x), 
            Mathf.Clamp(CameraTarget.position.y, minPos.y, maxPos.y), CameraTarget.position.z);
    }
    #endregion
}
