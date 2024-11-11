using System.Collections;
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

    void OnEnable()
    {
        planeManager.planesChanged += OnPlanesChanged;
    }

    void OnDisable()
    {
        planeManager.planesChanged -= OnPlanesChanged;
    }

    private void Start()
    {
        detectionCoroutine = StartCoroutine(WaitForPlanesDetection());
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs eventArgs)
    {
        foreach (var plane in eventArgs.added)
        {
            if (detectionCoroutine == null)
            {
                plane.gameObject.SetActive(false); // Disabilita il piano
                return;
            }

            // Instanzia una copia fisica del piano rilevato e disattiva l'originale
            InstantiatePhysicalPlane(plane);
            plane.gameObject.SetActive(false); // Disabilita il piano originale per evitare aggiornamenti
        }
    }

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
}
