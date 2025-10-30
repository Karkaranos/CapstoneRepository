using TMPro;
using UnityEngine;

public class EquippedArtifactButton : MonoBehaviour
{
    #region VARS
    private ArtifactData data;
    [SerializeField] private TMP_Text buttonText;
    private ArtifactMenuManager menuManager;

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

    public ArtifactData GetArtifactData()
    {
        return data;
    }

    #endregion VARS

    public int index;

    private void Start()
    {
        menuManager = FindFirstObjectByType<ArtifactMenuManager>();
    }

    public void ButtonClicked()
    {
        menuManager.EquipArtifact(index);
    }

    private void UpdateName(bool isNull)
    {
        if (isNull)
        {
            buttonText.text = string.Empty;
        }
        else
        {
            buttonText.text = data.name;
        }
    }
}
