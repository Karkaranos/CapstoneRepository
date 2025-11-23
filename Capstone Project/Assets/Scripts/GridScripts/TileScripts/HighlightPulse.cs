/******************************************************************************
 * Author: Tyler Bouchard
 * Creation Date: 11/20/2025
 * Last Modified: 11/20/2025 (Tyler Bouchard)
 * Brief: pulses the tile highlight foe visual appeal
 * External Resources: N/A
 * ***************************************************************************/
using System.Collections;
using UnityEngine;

public class HighlightPulse : MonoBehaviour
{
    [SerializeField] private float targetOpacity;
    [SerializeField] private float pulseTime;

    private SpriteRenderer sr;
    private Color startColor;
    private Color targetColor;
    
    private void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// starts the pulse coroutine
    /// </summary>
    public void startPulse() {
        startColor = sr.color;
        targetColor = startColor;
        targetColor.a = targetOpacity;
        StartCoroutine(Pulse());
    }

    /// <summary>
    /// pulses the highlight
    /// </summary>
    /// <returns></returns>
    private IEnumerator Pulse()
    {
        float t = 0f;
        bool fadingOut = true;

        while (true)
        {
            t += Time.deltaTime;

            if (fadingOut)
            {
                sr.color = Color.Lerp(startColor, targetColor, t);
            }
            else
            {
                sr.color = Color.Lerp(targetColor, startColor, t);
            }

            if (t >= pulseTime)
            {
                t = 0f;
                fadingOut = !fadingOut;
            }

            yield return null;
        }
    }
}
