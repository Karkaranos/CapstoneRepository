/*************************************************
Author Names : 		Tyler Bouchard
Date Created : 		2/2/2026
Date Last Modified : 2/10/2026
Brief Description : this holds the references and functions for the notebook in the out of combat menu
***************************************************/
using TMPro;
using UnityEngine;

public class NotebookManager : MonoBehaviour
{
    [SerializeField] private GameObject[] pages;
    [SerializeField] private Canvas canvas;
    
    // text boxes
    [SerializeField] private TextMeshProUGUI lightningTitle;
    [SerializeField] private TextMeshProUGUI windTitle;
    [SerializeField] private TextMeshProUGUI artifactTitle;
    [SerializeField] private TextMeshProUGUI lightningDescription;
    [SerializeField] private TextMeshProUGUI windDescription;
    [SerializeField] private TextMeshProUGUI artifactDescription;

    [SerializeField] private GameObject readyButton;

    //idk if this is gonna be needed but it here if I do
    private int currentPage;
    

    private void Update()
    {
        bool ready = false;
        foreach (RuneData runeData in EquipedRunesAndArtifacts.runes)
        {
            if (runeData != null)
            {
                ready = true;
            }
        }
        if (!ready)
        {
            readyButton.SetActive(false);
        }
        else {
            readyButton.SetActive(true);
        }
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
    /// updated the spell desctiption box on both the wind and lightning page
    /// </summary>
    /// <param name="node"></param>
    public void UpdateTextDescription(NotebookSpellNodeBehavior node)
    {
        lightningTitle.text = node.runeData.RuneName;
        lightningDescription.text = node.runeData.RuneDescription;
        windTitle.text = node.runeData.RuneName;
        windDescription.text = node.runeData.RuneDescription;
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
    /// test function to see if the runes and artifacts are equiping properly
    /// </summary>
    public void printData() {
        EquipedRunesAndArtifacts.PrintSpellsAndArtifacts();
    }
    public void continueToLevel() {
        PublicEvents.StartBattle.Invoke();
    }
}   
