/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		11/02/2025
Date Last Modified : 11/02/2025
Brief Description : Manages the equipped artifact buttons
External Resources : 	
	***************************************************/

using TMPro;
using UnityEngine;

public class EquippedArtifactButton : MonoBehaviour
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
        menuManager.EquipArtifact(this);
    }

    /// <summary>
    /// updates the name on the button
    /// </summary>
    /// <param name="isNull"> if the bool is null it sets the text to default</param>
    private void UpdateName(bool isNull)
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
