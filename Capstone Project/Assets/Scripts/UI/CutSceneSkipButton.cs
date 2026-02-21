using UnityEngine;

public class CutSceneSkipButton : MonoBehaviour
{
    [SerializeField] private GameObject videoCanvas;
    public void SkipCutscene()
    {
        videoCanvas.SetActive(false);
    }
}
