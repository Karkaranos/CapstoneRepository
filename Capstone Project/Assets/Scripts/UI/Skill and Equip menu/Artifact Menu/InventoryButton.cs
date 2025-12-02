/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		11/02/2025
Date Last Modified : 11/24/2025
Brief Description : Manages the inventory artifact buttons
External Resources : 	
	***************************************************/

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryButton : MonoBehaviour, IPointerEnterHandler
{
    #region VARS
    //not enough vars in the inspector to justify having the naughtyattributes stuff

    //the data in this button
    [SerializeField] private ArtifactData data;

    //refs to objects in scene
    private ArtifactMenuManager AMM;
    [SerializeField] private TMP_Text buttonTxt;
    private SkillAndArtifactManager skillArtMan;

    #region GETTERS AND SETTERS

    /// <summary>
    /// sets the artifact's data
    /// </summary>
    /// <param name="data"> the data to set </param>
    public void SetArtifactData(ArtifactData data)
    {
        this.data = data;
    }

    /// <summary>
    /// gets the artifact's data
    /// </summary>
    /// <returns></returns>
    public ArtifactData GetArtifactData()
    {
        return data;
    }

    #endregion
    #endregion VARS

    /// <summary>
    /// initializes the variables
    /// </summary>
    public void InsVars()
    {
        AMM = FindFirstObjectByType<ArtifactMenuManager>();
        skillArtMan = FindFirstObjectByType<SkillAndArtifactManager>();
        buttonTxt.text = data.Name;
    }

    /// <summary>
    /// tells the artifact manager when the button is clicked
    /// </summary>
    public void ButtonClicked()
    {
        if (AMM.heldArtifact != data && !ArtifactManager.CurrentArtifacts.Contains(data))
        {
            AMM.ArtifactPickedUp(data);
        }
        
    }

    /// <summary>
    /// tells the artifact manager when the button is hovered over
    /// currently broken - fuck scroll bars
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        AMM.ButtonHovered(data);
    }
}
