using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehavoir : MonoBehaviour
{
    [HideInInspector] public MenuBehavoir previousMenu;
    private void Awake()
    {
        previousMenu = null;
    }
    public void LoadScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    public void ActivateSubMenu(GameObject obj)
    {
        obj.SetActive(true);
        obj.GetComponent<MenuBehavoir>().previousMenu = GetComponent<MenuBehavoir>();
        gameObject.SetActive(false);
    }
    public void Quit()
    {
        print("quit");
        Application.Quit();
    }
    public void Return()
    {
        if (previousMenu != null)
        {
            previousMenu.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}