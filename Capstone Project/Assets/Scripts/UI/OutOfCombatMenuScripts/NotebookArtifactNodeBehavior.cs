using UnityEngine;
using UnityEngine.UI;

public class NotebookArtifactNodeBehavior : MonoBehaviour
{
    public ArtifactData artifactData;
    [SerializeField] public bool locked = false;
    [HideInInspector] public bool equipped = false;

    private Canvas[] AllCanvas;
    private Canvas canvas;
    private GameObject node;

    private void Start()
    {
        //finds the out of combat menu canvas
        AllCanvas = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas c in AllCanvas)
        {
            if (c.gameObject.name == "NewOutOfCombatMenu")
            {
                canvas = c;
                break;
            }
        }

    }

    public void Equip(bool b)
    {
        if (b)
        {
            equipped = true;
            gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.1f);
        }
        else
        {
            equipped = false;
            gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
        }
    }

    public void SpawnSpellNode(GameObject nodeToSpawn)
    {
        if (!locked && !equipped)
        {
            node = Instantiate(nodeToSpawn, nodeToSpawn.transform.position, Quaternion.identity);
            node.transform.SetParent(canvas.transform, false);

            ArtifactNodeBehavior anb = node.GetComponent<ArtifactNodeBehavior>();
            anb.canvas = canvas;
            anb.artifactData = artifactData;
            anb.notebookArtifactNode = this.GetComponent<NotebookArtifactNodeBehavior>();

            node.transform.localScale *= 1.2f;
        }
    }
}