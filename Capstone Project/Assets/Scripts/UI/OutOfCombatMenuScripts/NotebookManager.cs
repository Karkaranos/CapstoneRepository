using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotebookManager : MonoBehaviour
{


    [SerializeField] private GameObject[] pages;
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI title2;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI description2;
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

    public void UpdateTextDescription(NotebookSpellNodeBehavior node)
    {
        title.text = node.runeData.RuneName;
        description.text = node.runeData.RuneDescription;
        title2.text = node.runeData.RuneName;
        description2.text = node.runeData.RuneDescription;
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
