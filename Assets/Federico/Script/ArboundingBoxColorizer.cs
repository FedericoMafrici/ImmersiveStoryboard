using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ArboundingBoxColorizer : MonoBehaviour
{
    [SerializeField] public ARBoundingBox _boundingBox;

    private MeshRenderer _meshRenderer;

    private ARBoundingBoxManager _boundingBoxManager;

    private ConsoleDebugger _console;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
      
    }

    private void Awake()
    {
        /*
        _console = GameObject.Find("Debugging Window").GetComponent<ConsoleDebugger>();
        if (_console == null)
        {
            Debug.LogError("errore console debugger non ottenuto ");
        }
        */
        _boundingBoxManager = GameObject.Find("XR Origin (XR Rig)").GetComponent<ARBoundingBoxManager>();
        if (_boundingBoxManager == null)
        {
            Debug.LogError("errore ar bounding box Manager non ottenuto ");
        }
        _boundingBoxManager.trackablesChanged.AddListener(UpdateBoxColor);
        _boundingBox = GetComponent<ARBoundingBox>();
        if (_boundingBox == null)
        {
            Debug.LogError("errore ar bounding box non ottenuto");
        }
        
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

  public void UpdateBoxColor(ARTrackablesChangedEventArgs<ARBoundingBox> b)
    {
        
        Color boxMatColor = GetColorByClassification(_boundingBox.classifications);
        boxMatColor.a = 0.0f;
        _meshRenderer.material.color = boxMatColor;
 //   _console.SetText("ho fatto partire la update box color: valore del meshrender:"+_meshRenderer.transform.localScale.ToString());
    
    }

// Update is called once per frame
    void Update()
    {
        if (_meshRenderer != null)
        {
            _meshRenderer.transform.localScale = _boundingBox.size;
        }
        else
        {
            Debug.LogError("meshrender non trovato");
        }
        
    }

    private Color GetColorByClassification(BoundingBoxClassifications classifications)
    {
        if (classifications.HasFlag(BoundingBoxClassifications.Couch)) return Color.blue;
        if (classifications.HasFlag(BoundingBoxClassifications.Table)) return Color.yellow;
        if (classifications.HasFlag(BoundingBoxClassifications.Bed)) return Color.cyan;
        if (classifications.HasFlag(BoundingBoxClassifications.Lamp)) return Color.magenta;
        if (classifications.HasFlag(BoundingBoxClassifications.Plant)) return Color.green;
        if (classifications.HasFlag(BoundingBoxClassifications.Screen)) return Color.white;
        if (classifications.HasFlag(BoundingBoxClassifications.Storage)) return Color.red;
        
        return Color.gray; // Other - Default color
    }
}
