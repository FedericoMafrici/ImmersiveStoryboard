using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class BoundingBoxScaler : MonoBehaviour
{
    private ARBoundingBox _boundingBox; // riferimento alla bounding box relativa alla scatola in questione
    [SerializeField] public ConsoleDebugger debugging_window;
    [SerializeField] public GameObject box; //scatola modificabile 

    [SerializeField] public GameObject boxEmptyController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        box=GameObject.Find("BoundingBoxWrapper");
        debugging_window = FindObjectOfType<ConsoleDebugger>();
        BoundingBoxManager.onBoundingBoxChanged += updateBoxSize;
    }

  

    public void updateBoxSize(object sender,EventArgs e)
    {
        box.transform.localScale = _boundingBox.size;
        boxEmptyController.transform.position = _boundingBox.transform.position;
        boxEmptyController.transform.rotation = _boundingBox.transform.rotation;
        boxEmptyController.GetComponent<BoundingBoxInteractionManager>().setBoundingBoxSize(_boundingBox.size);
    }
    public void SetBoundingBox(GameObject emptyController, GameObject cube, ARBoundingBox boundingBox)
    {
        _boundingBox = boundingBox;
        box = cube;
        boxEmptyController = emptyController;
        debugging_window.SetText("setbounding box chiamata"+ cube.name+" "+ _boundingBox.name);
        BoundingBoxInteractionManager bbint = emptyController.GetComponent<BoundingBoxInteractionManager>();
        bbint.SetLabel(boundingBox.classifications.ToString());
        bbint.setBoundingBoxSize(boundingBox.size);
    }
}
