using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaneManager : MonoBehaviour
{
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private ARAnchorManager anchorManager;
    [SerializeField] private ConsoleDebugger debuggingWindow;
    [SerializeField] private GameObject planePrefab; // Prefab fisico da instanziare per ogni piano rilevato
    private float detectionTimeout = 10f; // Tempo di attesa per il rilevamento dei piani
    private Coroutine detectionCoroutine;
    private NavMeshSurface navMeshSurface;
    [SerializeField] private Material navMeshMaterial; // Materiale per la visualizzazione della NavMesh
    private float rebuildInterval = 40f; // Intervallo in secondi
    public static EventHandler<EventArgs> onNavMeshRebuildRequest;
    
    
    private void OnEnable()
    {
        planeManager.planesChanged += OnPlanesChanged;
        onNavMeshRebuildRequest += UpdateNavMesh;
    }

    private void OnDisable()
    {
        planeManager.planesChanged -= OnPlanesChanged;
        onNavMeshRebuildRequest -= UpdateNavMesh;
    }

    private void Start()
    {
        navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
        navMeshSurface.collectObjects = CollectObjects.Children;
        StartCoroutine(RequestNavMeshRebuild());
       // StartCoroutine(DisablePlaneManager());
    }

    private IEnumerator DisablePlaneManager()
    {
        yield return new WaitForSeconds(5); 
        planeManager.enabled = false;
        planeManager.planesChanged -= OnPlanesChanged;
        debuggingWindow.SetText("Ho interroto l'update ai piani non dovrebbero più essere aggiornati");
       
    }

    private IEnumerator RequestNavMeshRebuild()
    {
        while (true)
        {
            yield return new WaitForSeconds(rebuildInterval);
            
            // Lancia l'evento di richiesta di ricostruzione della NavMesh
            onNavMeshRebuildRequest?.Invoke(this, EventArgs.Empty);
            Debug.Log("Evento di richiesta ricostruzione NavMesh lanciato.");
        }
    }
    private void OnPlanesChanged(ARPlanesChangedEventArgs eventArgs)
    {
      //  debuggingWindow.SetText("è stata chiamata la OnPlanesChanged");
        // Gestisci piani aggiunti
        foreach (var plane in eventArgs.added)
        {
            if (!IsUnwantedPlane(plane.classifications))
            {
                // Aggiungi il renderer per visualizzare il materiale se necessario
                Renderer planeRenderer = plane.GetComponent<Renderer>();
                if (planeRenderer == null)
                {
                    planeRenderer = plane.gameObject.AddComponent<MeshRenderer>();
                }
                //planeRenderer.material = navMeshMaterial;
                
                // Imposta il piano come figlio per includerlo nella NavMesh
                plane.transform.SetParent(this.transform);
            }
        }

        // Gestisci piani aggiornati (se devono essere rimossi o aggiunti alla NavMesh)
        foreach (var plane in eventArgs.updated)
        {
            if (IsUnwantedPlane(plane.classifications))
            {
                plane.gameObject.SetActive(false);
            }
            else
            {
                plane.gameObject.SetActive(true);
            }
        }

        // Gestisci piani rimossi
        foreach (var plane in eventArgs.removed)
        {
            plane.gameObject.SetActive(false);
        }

        // Aggiorna la NavMesh per riflettere i cambiamenti nei piani
    }

    private void UpdateNavMesh(object sender, EventArgs e )
    {
        navMeshSurface.BuildNavMesh();
    }


   
   
    private bool IsUnwantedPlane(PlaneClassifications classifications )
    {
        List<PlaneClassification> labels = GetClassifications(classifications);
        bool value = true;
       // debuggingWindow.SetText("Piano etichette:");
        string buffer = "";
        foreach (var label in labels)
        {
            buffer += label;
            if (label==PlaneClassification.Ceiling || label==PlaneClassification.Floor || label==PlaneClassification.Wall)
            {
              //  debuggingWindow.SetText("Ho trovato un piano visualizzabile ");
                value=false;
            }
           
        }
     //   debuggingWindow.SetText(buffer);
        return value;
    }

    public List<PlaneClassification> GetClassifications(PlaneClassifications classifications)
    {
        List<PlaneClassification> list = new List<PlaneClassification>();
        if ((classifications & classifications & PlaneClassifications.Ceiling) != 0)
        {
            list.Add(PlaneClassification.Ceiling);
        }
        else if ((classifications & classifications & PlaneClassifications.Floor) != 0)
        {
            list.Add(PlaneClassification.Floor);
        }
        else if ((classifications & classifications & PlaneClassifications.Other) != 0)
        {
            list.Add(PlaneClassification.Other);
        }
        else if ((classifications & classifications & PlaneClassifications.WallFace) != 0)
        {
            list.Add(PlaneClassification.Wall);
        }
        else if ((classifications & classifications & PlaneClassifications.Ceiling) != 0)
        {
            list.Add(PlaneClassification.Ceiling);
        }
        else if ((classifications & classifications & PlaneClassifications.None) != 0)
        {
            list.Add(PlaneClassification.None);
        }
        return list;

    }
    // unused function
    private void InstantiatePhysicalPlane(ARPlane plane)
    {
        // Instanzia il prefab alla posizione e rotazione del piano AR
        GameObject physicalPlane = Instantiate(planePrefab, plane.transform.position, plane.transform.rotation);
        debuggingWindow.SetText("Copia fisica del piano instanziata.");

        // Imposta la scala del prefab in base alla dimensione del piano AR
        physicalPlane.transform.localScale = new Vector3(plane.size.x, 1, plane.size.y);

        // Aggiungi un'ancora al prefab per stabilizzarlo
        AddAnchorToPlane(physicalPlane, plane);

        // Disabilita o rimuovi il componente ARPlane e ARPlaneMeshVisualizer dal prefab istanziato
        DisableARComponents(physicalPlane);
    }

    
    private void DisableARComponents(GameObject planeObject)
    {
        // Rimuovi o disabilita i componenti AR dal prefab istanziato
        ARPlane arPlane = planeObject.GetComponent<ARPlane>();
        if (arPlane != null)
        {
            Destroy(arPlane); // Rimuove il componente ARPlane
        }

        ARPlaneMeshVisualizer meshVisualizer = planeObject.GetComponent<ARPlaneMeshVisualizer>();
        if (meshVisualizer != null)
        {
            Destroy(meshVisualizer); // Rimuove il visualizzatore di mesh
        }

        MeshRenderer meshRenderer = planeObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            Destroy(meshRenderer); // Rimuove il MeshRenderer se necessario
        }

        debuggingWindow.SetText("Componenti AR disabilitati dal prefab istanziato.");
    }
    
    private IEnumerator WaitForPlanesDetection()
    {
        // Attendi per un periodo definito senza rilevamenti di nuovi piani
        yield return new WaitForSeconds(detectionTimeout);

        // Disabilita l'ARPlaneManager dopo il timeout
        planeManager.enabled = false;
        debuggingWindow.SetText("Plane Manager disabilitato.");
        Debug.Log("Rilevamento dei piani completato, ARPlaneManager disabilitato.");

        // Annulla l'iscrizione all'evento planesChanged
        planeManager.planesChanged -= OnPlanesChanged;
    }
    
    private async void AddAnchorToPlane(GameObject planeObject, ARPlane originalPlane)
    {
        if (anchorManager != null && planeObject != null)
        {
            var result = await anchorManager.TryAddAnchorAsync(new Pose(planeObject.transform.position, planeObject.transform.rotation));
            if (result.status.IsSuccess())
            {
                var anchor = result.value; // Nuova ancora specifica per questo piano
                planeObject.transform.parent = anchor.transform; // Rende l'oggetto figlio dell'ancora
                debuggingWindow.SetText("Ancora creata ed assegnata con successo.");
                Debug.Log("Ancora creata e assegnata con successo.");
            }
            else
            {
                Debug.LogWarning("Impossibile creare un'ancora alla posizione specificata.");
            }
        }
    }
}