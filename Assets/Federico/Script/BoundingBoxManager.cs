using System;
using System.Collections;
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
    [SerializeField] public ARAnchorManager anchorManager;
    private bool isListening = false;
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
       // _debuggingWindow.SetText("ho appena instanziato una bounding box");
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
    
    private void StartListeningToBoundingBoxChanges()
    {
        if (!isListening)
        {
            bbManager.trackablesChanged.AddListener(OnTrackablesChanged);
            isListening = true;
        }
    }

    private void StopListeningToBoundingBoxChanges()
    {
        if (isListening)
        {
            bbManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
            isListening = false;
        }
    }

    public void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARBoundingBox> changes)
    {
        // Esegui le azioni per ogni bounding box aggiunta
        foreach (var boundingBox in changes.added)
        {
            OnBoundingBoxDetected(boundingBox);
        }

        // Disiscriviti dall'evento e attendi prima di riscriversi
        StopListeningToBoundingBoxChanges();
        StartCoroutine(ReEnableBoundingBoxListening());
    }
    private IEnumerator ReEnableBoundingBoxListening()
    {
        yield return new WaitForSeconds(40); // Attendi un intervallo di tempo (es. 10 secondi)
        StartListeningToBoundingBoxChanges();
    }
    void Update()
    {
        // Opzionale: aggiornamenti e logiche aggiuntive
    }
}
