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
    private Coroutine scrollCoroutine;

    /// <summary>
    /// sets the starting phase
    /// </summary>
    private void Awake()
    {
        startPos = rectTransform.localPosition;
    }

    /// <summary>
    /// runs on start
    /// </summary>
    private void Start()
    {
        screenLength = startPos.y - endPos.y;
    }

    /// <summary>
    /// scrolls when the screen gets set active
    /// </summary>
    private void OnEnable()
    {
        if (autoScroll)
        {
            rectTransform.localPosition = startPos;
            scrollbar.gameObject.SetActive(false);
            StartAutoScrolling();
        }
        else
        {
            startPos = rectTransform.position;
        }
    }

    /// <summary>
    /// stops scrolling when you leave the screen
    /// </summary>
    private void OnDisable()
    {
        if (scrollCoroutine != null)
        {
            StopCoroutine(scrollCoroutine);
            scrollCoroutine = null;
        }
    }

    /// <summary>
    /// when this function is called it starts the Acto scroll of the credits menu
    /// </summary>
    public void StartAutoScrolling() {
        if (scrollCoroutine == null) {
            scrollCoroutine = StartCoroutine(AutoScroll());
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

    /// <summary>
    /// this is the functionality of the auto scroll behavior
    /// </summary>
    /// <returns></returns>
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
