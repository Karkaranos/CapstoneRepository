/*************************************************
Author Names : 	    	Cade Naylor
Date Created : 		    2/5/2026
Date Last Modified : 	2/5/2026
Brief Description : 	This class has pretty brief behavior for a damage/stat update indicator in combat
External Resources : 
***************************************************/
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusIndicator : MonoBehaviour
{
    #region Variables
    [Header("Colors")]
    [SerializeField, Tooltip("What color denotes positive effects for the given entity")]
    private Color positiveColor = Color.green;
    [SerializeField, Tooltip("What color denotes negative effects for the given entity")]
    private Color negativeColor = Color.red;

    [Header("Timing")]
    [SerializeField, Tooltip("How long the text is visible, in total")]
    private float time = 3f;
    [SerializeField, Tooltip("Whether this object rises or not")]
    private bool rising = true;
    [SerializeField, Tooltip("What percentage of the screen's height it rises, as a decimal"), Range(0f,1f)]
    private float riseHeight = .1f;
    [SerializeField, Tooltip("How long before the text starts fading out")]
    private float timeBeforeFade = 1f;

    [Header("References")]
    [SerializeField, Tooltip("The text component of the object")]
    private TMP_Text text;
    [SerializeField, Tooltip("The image component of the object")]
    private Image image;

    Coroutine routine = null;
    #endregion

    /// <summary>
    /// Initializes this text display
    /// </summary>
    /// <param name="displayText">What it displays as</param>
    /// <param name="isPositive">Whether the effect is positive for the given entity or not</param>
    /// <param name="relatedSprite">Any related sprites for elemental damage</param>
    public void Initialize(string displayText, bool isPositive, Sprite relatedSprite = null)
    {
        text.color = isPositive ? positiveColor : negativeColor;
        text.text = displayText;
        if (relatedSprite != null)
        {
            image.sprite = relatedSprite;
        }
        else
        {
            image.gameObject.SetActive(false);
        }


        if (routine == null)
        {
            routine = StartCoroutine(Timer());
        }
    }

    private IEnumerator Timer()
    {
        Vector3 newPos = transform.position;
        float t = 0;

        Color alphaForText = text.color;
        Color alphaForImage = image.color;
        float a = 1;

        while (t < time)
        {
            t += Time.deltaTime;
            if (rising)
            {
                newPos.y += Screen.height * riseHeight * Time.deltaTime * .01f;
                transform.position = newPos;
            }

            if (t > timeBeforeFade)
            {
                a = 1 - (t / (time - timeBeforeFade)); 
                alphaForImage.a = a;
                alphaForText.a = a;
                text.color = alphaForText;
                image.color = alphaForImage;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
   
}
