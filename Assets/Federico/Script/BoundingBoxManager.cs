using System;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.OpenXR.Features.Meta;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class BoundingBoxManager : MonoBehaviour
{
    // Material da assegnare alle bounding box
    public Material boundingBoxMaterial;
    public ConsoleDebugger _debuggingWindow;
    public GameObject _boundingBoxPrefab;
    // Lista delle bounding box rilevate
    private List<ARBoundingBox> boundingBoxes = new List<ARBoundingBox>();
    
    [SerializeField] private ARBoundingBoxManager bbManager; // Databa
    public static EventHandler<EventArgs> onBoundingBoxChanged;
    void Start()
    {
    
            if (LoaderUtility
                    .GetActiveLoader()?
                    .GetLoadedSubsystem<XRBoundingBoxSubsystem>() != null)
            {
                // XRBoundingBoxSubsystem was loaded. The platform supports bounding box detection.
            }
            else
            {
                Debug.LogError("il meta quest non supporta la detection delle bounding box molto strano");
            }
        
        
        // Registrazione dell'evento di rilevamento delle bounding box
      //  ARBoundingBoxManager.boundingBoxDetected += OnBoundingBoxDetected;
      if (bbManager == null)
      {
          Debug.LogError("cannot find bounding box manager ");
          
      }
      bbManager.trackablesChanged.AddListener(OnTrackablesChanged);
    }
    public void CreateCubeFromBoundingBox(ARBoundingBox boundingBox)
    {
        Debug.Log("cubo instanziato");
        _debuggingWindow.SetText("ho appena instanziato una bounding box");
        // instanziato l'empty e reperiamo la bounding box per modificare la scala
        GameObject prefab = GameObject.Instantiate(_boundingBoxPrefab);
        GameObject wrapper = prefab.transform.GetChild(3).gameObject;
        // vogliamo spostare tutto il prefab ma scalare solo la bounding box non la label
        wrapper.transform.localScale = boundingBox.size;
        prefab.transform.position = boundingBox.transform.position;
        prefab.transform.rotation = boundingBox.transform.rotation;

        BoundingBoxScaler scaler = prefab.GetComponent<BoundingBoxScaler>();
       //facciamo in modo che il prefab possa regolare la sua dimensione in modo appropriato e settare la label
        scaler.SetBoundingBox(prefab,wrapper,boundingBox);
    }
    
    void OnDestroy()
    {
        // Rimozione dell'evento
    //    ARBoundingBoxManager.boundingBoxDetected -= OnBoundingBoxDetected;
    }

    // Funzione chiamata quando viene rilevata una bounding box
    private void OnBoundingBoxDetected(ARBoundingBox box)
    {
       CreateCubeFromBoundingBox(box);
    }

   

    // Applica il materiale alla bounding box
    private void ApplyMaterialToBoundingBox(ARBoundingBox box)
    {
        var renderer = box.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = boundingBoxMaterial;
        }
    }
    
    public void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARBoundingBox> changes)
    {
       
        foreach (var boundingBox in changes.added)
        {
           
            _debuggingWindow.SetText("Bounding box trovata la aggiungo");
            OnBoundingBoxDetected(boundingBox);
        }

        if (changes.updated.Count != 0)
        {
            onBoundingBoxChanged?.Invoke(this,EventArgs.Empty);
        }
        /*
        foreach (var boundingBox in changes.updated)
        {
            
        }
        
        foreach (var boundingBox in changes.removed)
        {
            // handle removed bounding boxes
        }
        */
    }
    void Update()
    {
        // Opzionale: aggiornamenti e logiche aggiuntive
    }
}
