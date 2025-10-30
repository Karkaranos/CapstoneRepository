using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    [SerializeField] private ArtifactData data;
    private ArtifactMenuManager AMM;
    [SerializeField] private TMP_Text buttonTxt;

    #region GETTERS AND SETTERS

    public void SetArtifactData(ArtifactData data)
    {
        this.data = data;
    }

    public ArtifactData GetArtifactData()
    {
        return data;
    }

    #endregion

    private void Start()
    {
        if (data != null)
        {
            InsVars();
        }
    }
    public void InsVars()
    {
        AMM = FindFirstObjectByType<ArtifactMenuManager>();
        buttonTxt.text = data.name;
    }

    public void ButtonClicked()
    {
        AMM.ArtifactPickedUp(data, true);
    }

    public void OnHover()
    {
        AMM.ButtonHovered(data);
    }

}
