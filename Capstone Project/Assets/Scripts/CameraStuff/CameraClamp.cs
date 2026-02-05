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
    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] Vector2 moveInput;
    [SerializeField, ShowIf(nameof(Cams), Cameras.Refs)] public float panSpeed;
    #endregion


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
