using UnityEngine;

public class NotebookManager : MonoBehaviour
{
    [SerializeField] private GameObject[] pages;
    private int currentPage;

    public void LoadPage(int page) {
        HideAllPages();
        pages[page].SetActive(true);
        currentPage = page;
    }

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

    private void HideAllPages() { 
        foreach (GameObject page in pages)
        {
            page.SetActive(false);  
        }
    }
}   
