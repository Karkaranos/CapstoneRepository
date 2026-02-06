using UnityEngine;
using UnityEngine.UI;

public class NotebookSpellNodeBehavior : MonoBehaviour
{
    public RuneData runeData;
    [SerializeField] public bool locked = false;
    [HideInInspector] public bool equipped = false;
    
    private Canvas[] AllCanvas;
    private Canvas canvas;
    private GameObject node;

    private void Start()
    {
        //finds the out of combat menu canvas
        AllCanvas = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas c in AllCanvas) {
            if (c.gameObject.name == "NewOutOfCombatMenu") {
                canvas = c; 
                break;
            }
        }

    }

    public void Equip(bool b) {
        if (b) {
            equipped = true;
            gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.1f);
        }
        else { 
            equipped= false;
            gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
        }
    }

    public void SpawnSpellNode(GameObject nodeToSpawn)
    {
        if (!locked && !equipped) {
            node = Instantiate(nodeToSpawn, nodeToSpawn.transform.position, Quaternion.identity);
            node.transform.SetParent(canvas.transform, false);
            
            SpellNodeBehavior snb = node.GetComponent<SpellNodeBehavior>();
            snb.canvas = canvas;
            snb.runeData = runeData;
            snb.notebookSpellNode = this.GetComponent<NotebookSpellNodeBehavior>();
            
            node.transform.localScale *= 1.2f;
        } 
    }
}
