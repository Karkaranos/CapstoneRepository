/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		11/02/2025
Date Last Modified : 11/02/2025
Brief Description : Manages the equipped artifact buttons
External Resources : 	
	***************************************************/

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquippedArtifactButton : MonoBehaviour, IPointerEnterHandler
{
    #region VARS
    //not enough vars in the inspector to justify having the naughtyattributes stuff

    //the data in this button
    private ArtifactData data;

    //refs to gameobjects in scene
    [SerializeField] private TMP_Text buttonText;
    private ArtifactMenuManager menuManager;

    /// <summary>
    /// setter for the button's data
    /// </summary>
    /// <param name="data"> the new data for the button </param>
    public void SetArtifactData(ArtifactData data)
    {
        this.data = data;
        if (data == null)
        {
            UpdateName(true);
        }
        else
        {
            UpdateName(false);
        }
    }

    /// <summary>
    /// getter for the button's data
    /// </summary>
    /// <returns> the button's data </returns>
    public ArtifactData GetArtifactData()
    {
        return data;
    }

    #endregion VARS

    /// <summary>
    /// sets refs and initializes
    /// </summary>
    private void Start()
    {
        menuManager = FindFirstObjectByType<ArtifactMenuManager>();
    }

    /// <summary>
    /// runs equipartifact when the button is clicked on
    /// </summary>
    public void ButtonClicked()
    {
        Debug.Log("CHh");
        menuManager.EquipArtifact(this);
    }

    /// <summary>
    /// updates the name on the button
    /// </summary>
    /// <param name="isNull"> if the bool is null it sets the text to default</param>
    private void UpdateName(bool isNull)
    {
        if (buttonText != null)
        {
            if (isNull)
            {
                buttonText.text = "Artifact Slot";
            }
            else
            {
                buttonText.text = data.Name;
            }
        }
    }

    /// <summary>
    /// Updates the description when the player's mouse enters the button's hitbox
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        menuManager.ButtonHovered(data);
    }
}
