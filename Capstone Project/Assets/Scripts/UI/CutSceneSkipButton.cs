using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneSkipButton : MonoBehaviour
{
    [SerializeField] private GameObject videoCanvas;
    public void SkipCutscene()
    {
        videoCanvas.SetActive(false);
    }

    public void SkipBook()
    {
        SceneManager.LoadScene(2);
    }
}