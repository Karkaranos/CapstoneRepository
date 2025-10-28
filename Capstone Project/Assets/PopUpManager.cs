using UnityEngine;
using NaughtyAttributes;

public class PopUpManager : MonoBehaviour
{
    private Animator popUpAnimator;
    [SerializeField] private GameObject popUpPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Flip()
    {
        popUpAnimator.SetTrigger("Flip");
    }
}
