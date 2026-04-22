/*************************************************
Author Names : 		Tyler Bouchard 
Date Created : 		4/21/2026
Date Last Modified : 	4/22/2026
Brief Description : 		controls the behavoir for the credits menu
***************************************************/
using UnityEngine;
using UnityEngine.UI;

public class CreditsMenuBehavior : MonoBehaviour
{
    public float topPosition;
    public float bottomPosition;

    public RectTransform rectTransform;
    public Scrollbar scrollbar;

    private float screenLength;

    /// <summary>
    /// runs on start
    /// </summary>
    private void Start()
    {
        screenLength = topPosition - bottomPosition;
        topPosition = rectTransform.position.y;
    }

    /// <summary>
    /// updates the position of the credits page when the scrollbar moves
    /// </summary>
    public void UpdatePagePosition()
    {
        Vector3 newPos = new Vector3 (rectTransform.position.x, topPosition + (scrollbar.value * screenLength), rectTransform.position.z);
        rectTransform.position = newPos;
    }
}
