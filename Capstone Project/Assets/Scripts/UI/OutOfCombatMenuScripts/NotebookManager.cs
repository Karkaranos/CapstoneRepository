using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NotebookManager : MonoBehaviour
{
    [SerializeField] private GameObject[] pages;
    [SerializeField] private Canvas canvas;
    private int currentPage;

    /// <summary>
    /// loads a page based of its index in the pages list
    /// </summary>
    /// <param name="page"></param>
    public void LoadPage(int page) {
        HideAllPages();
        pages[page].SetActive(true);
        currentPage = page;
    }
    
    /// <summary>
    /// goes to the next page
    /// </summary>
    public void NextPage() {
        if (currentPage >= pages.Length - 1) 
        {
            currentPage = 0;
        } 
        else 
        {
            currentPage++;
        }
        LoadPage(currentPage);
    }
    
    /// <summary>
    /// goes to the previous page
    /// </summary>
    public void PreviousPage()
    {
        if (currentPage <= 0)
        {
            currentPage = pages.Length - 1;
        }
        else
        {
            currentPage--;
        }
        LoadPage(currentPage);
    }

    public void SpawnSpellNode(GameObject nodeToSpawn) {
        GameObject node = Instantiate(nodeToSpawn, nodeToSpawn.transform.position, Quaternion.identity);
        node.transform.SetParent(canvas.transform, false);
        node.GetComponent<SpellNodeBehavior>().canvas = canvas;
    }

    /// <summary>
    /// hides all of the pages in the book
    /// </summary>
    private void HideAllPages() { 
        foreach (GameObject page in pages)
        {
            page.SetActive(false);  
        }
    }
}   
