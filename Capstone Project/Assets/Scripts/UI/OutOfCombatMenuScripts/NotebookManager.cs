/*************************************************
Author Names : 		Tyler Bouchard
Date Created : 		2/2/2026
Date Last Modified : 2/10/2026
Brief Description : this holds the references and functions for the notebook in the out of combat menu
***************************************************/
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotebookManager : MonoBehaviour
{
    [SerializeField] private GameObject[] pages;
    [SerializeField] private Canvas canvas;
    
    // text boxes
    [SerializeField] private TextMeshProUGUI artifactTitle;
    [SerializeField] private TextMeshProUGUI artifactDescription;

    //idk if this is gonna be needed but it here if I do
    private int currentPage;

    /// <summary>
    /// this gets rid of the artifact carry over bug
    /// </summary>
    private void Start()
    {
        ArtifactManager.CurrentArtifacts = new List<ArtifactData>();
    }

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

    /// <summary>
    /// updated the attifact desctiption box on the artifact page
    /// </summary>
    /// <param name="node"></param>
    public void UpdateTextDescription(NotebookArtifactNodeBehavior node)
    {
        artifactTitle.text = node.artifactData.name;
        artifactDescription.text = node.artifactData.Description;
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


    public void continueToLevel() {
        PublicEvents.StartBattle.Invoke();
    }
}   
