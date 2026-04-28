/*************************************************
Author Names : 		Tyler Bouchard
Date Created : 		2/2/2026
Date Last Modified : 2/10/2026
Brief Description : this holds the references and functions for the notebook in the out of combat menu
***************************************************/
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NotebookManager : MonoBehaviour
{
    [SerializeField] private GameObject[] pages;
    [SerializeField] private Canvas canvas;
    [SerializeField] List<Button> tabButtons = new List<Button>();
    
    // text boxes
    [SerializeField] private TextMeshProUGUI artifactTitle;
    [SerializeField] private TextMeshProUGUI artifactDescription;

    //idk if this is gonna be needed but it here if I do
    private int currentPage;

    TextBoxManager tm;
    

    /// <summary>
    /// loads a page based of its index in the pages list
    /// </summary>
    /// <param name="page"></param>
    public void LoadPage(int page) {
        HideAllPages();
        pages[page].SetActive(true);
        currentPage = page;

        if(tm != null)
        {
            tm.CanClick = true;
            tm.ShowTextBox();
        }
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

    /// <summary>
    /// I believe this is the ready button
    /// </summary>
    public void continueToLevel() {
        PublicEvents.StartBattle.Invoke();
    }

    /// <summary>
    /// Creates a reference to the text box manager used in the tutorial
    /// </summary>
    /// <param name="tm"></param>
    public void CreateTutorialReference(TextBoxManager tm)
    {
        this.tm = tm;
    }

    /// <summary>
    /// Determines if the buttons hover state should show
    /// </summary>
    private void FixedUpdate()
    {
        if(tm != null)
        {
            switch(tm.tutorialCheck)
            {
                //Go to lightning tab
                case 1:
                    tabButtons[1].interactable = true;
                    break;
                //Go to artifact tab
                case 2:
                    tabButtons[2].interactable = true;
                    break;
                //Ready up
                case 3:
                    tabButtons[0].interactable = true;
                    break;
            }
        }
    }
}   
