/*************************************************
Author Names : 		Tyler Bouchard
Date Created : 		2/2/2026
Date Last Modified : 2/10/2026
Brief Description : this is the behavior of the spell slots in the notebook
this also stores if its locked or unlocked 
***************************************************/
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

    /// <summary>
    /// initialization
    /// </summary>
    private void Start()
    {
        //finds the out of combat menu canvas
        AllCanvas = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas c in AllCanvas) {
            if (c.gameObject.name == "UICanvas2") {
                canvas = c; 
                break;
            }
        }

    }

    /// <summary>
    /// when the Spell node gets equiped it will turn itself to 10% opacity to showcase it
    /// </summary>
    /// <param name="b"></param>
    public void Equip(bool b) {
        if (b) {
            equipped = true;
            gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.1f);
        }
        else { 
            equipped= false;
            gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
            //PublicEvents.RuneUnequipped?.Invoke(runeData);
        }
    }

    /// <summary>
    /// this is where the spell node gets spawned
    /// </summary>
    /// <param name="nodeToSpawn"></param>
    public void SpawnSpellNode(GameObject nodeToSpawn)
    {
        if (!locked && !equipped) {
            node = Instantiate(nodeToSpawn, nodeToSpawn.transform.position, Quaternion.identity);
            node.transform.SetParent(canvas.transform, false);
            
            SpellNodeBehavior snb = node.GetComponent<SpellNodeBehavior>();
            snb.canvas = canvas;
            snb.runeData = runeData;
            snb.notebookSpellNode = this.GetComponent<NotebookSpellNodeBehavior>();

            node.transform.SetParent(gameObject.transform.parent.parent.parent);//stupid but works
            node.transform.localScale *= 1.8f;
        } 
    }
}
