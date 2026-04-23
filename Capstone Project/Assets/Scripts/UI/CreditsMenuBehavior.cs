/*************************************************
Author Names : 		Tyler Bouchard 
Date Created : 		4/21/2026
Date Last Modified : 	4/22/2026
Brief Description : 		controls the behavoir for the credits menu
***************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreditsMenuBehavior : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 endPos;

    public RectTransform rectTransform;
    public Scrollbar scrollbar;

    public float scrollDuration = 5f;

    private float screenLength;

    public bool autoScroll;

    /// <summary>
    /// runs on start
    /// </summary>
    private void Start()
    {
        screenLength = startPos.y - endPos.y;

        if (autoScroll)
        {
            startPos = rectTransform.localPosition;
            scrollbar.gameObject.SetActive(false);
            StartCoroutine(AutoScroll());
        }
        else {
            startPos = rectTransform.position;
        }

    }

    /// <summary>
    /// updates the position of the credits page when the scrollbar moves
    /// </summary>
    public void UpdatePagePosition()
    {
        Vector3 newPos = new Vector3 (rectTransform.position.x, startPos.y - (scrollbar.value * screenLength), rectTransform.position.z);
        rectTransform.position = newPos;
    }

    private IEnumerator AutoScroll()
    {
        print(startPos + " " + endPos);
        float time = 0f;

        while (time < scrollDuration)
        {
            time += Time.deltaTime;

            float t = time / scrollDuration;
            rectTransform.localPosition = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }
        rectTransform.localPosition = endPos;
    }
}
