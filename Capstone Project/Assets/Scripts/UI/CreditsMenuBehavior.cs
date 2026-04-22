using Unity.VisualScripting;
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

    private void Update()
    {
        Vector3 newPos = new Vector3 (rectTransform.position.x, topPosition + (scrollbar.value * screenLength), rectTransform.position.z);
        rectTransform.position = newPos;
    }
}
