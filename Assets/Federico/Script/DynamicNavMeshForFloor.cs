using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using Unity.AI.Navigation;

public class DynamicNavMeshForFloor : MonoBehaviour
{
    public ARPlaneManager planeManager;  // Il Plane Manager per rilevare i piani
    public Material planeMaterial;  // Materiale per rendere visibili i piani

    void OnEnable()
    {
        planeManager.planesChanged += OnPlanesChanged;
    }

    void OnDisable()
    {
        planeManager.planesChanged -= OnPlanesChanged;
    }
    void Start()
    {
        // Solo per editor, non usare in build finale
        if (Application.isEditor)
        {
            SimulateARPlane();
        }
    }

    void SimulateARPlane()
    {
        // Simula un piano AR nell'editor
        GameObject simulatedPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        simulatedPlane.transform.position = new Vector3(0, 0, 0);  // Posizione nel mondo

        // Aggiungi NavMeshSurface al piano simulato
        var navSurface = simulatedPlane.AddComponent<NavMeshSurface>();
        navSurface.collectObjects = CollectObjects.All;
        navSurface.BuildNavMesh();

        // Applica un materiale visibile se fornito
        if (planeMaterial != null)
        {
            simulatedPlane.GetComponent<MeshRenderer>().material = planeMaterial;
        }

        Debug.Log("Piano simulato con NavMesh aggiunto nell'editor.");
    }

    
    private void AddNavMeshSurface(ARPlane arPlane)
    {
        // Crea un nuovo piano digitale
        GameObject newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        newPlane.transform.position = arPlane.transform.position;
        newPlane.transform.rotation = arPlane.transform.rotation;

        // Assicurati che il piano digitale segua le dimensioni dell'ARPlane
        newPlane.transform.localScale = new Vector3(arPlane.size.x / 10f, 1f, arPlane.size.y / 10f);

        // Applica un materiale visibile se fornito
        if (planeMaterial != null )
        {
            newPlane.GetComponent<MeshRenderer>().material = planeMaterial;
            var color = newPlane.GetComponent<MeshRenderer>().material.color;
            color.a = 0.0f;
            newPlane.GetComponent<MeshRenderer>().material.color=color;
        }

        // Aggiungi il componente NavMeshSurface
        var navSurface = newPlane.AddComponent<NavMeshSurface>();
        navSurface.collectObjects = CollectObjects.All;
        navSurface.BuildNavMesh();

        Debug.Log("NavMesh aggiunta su piano generato.");
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // Per ogni piano aggiunto
        foreach (var plane in args.added)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                AddNavMeshSurface(plane);
            }
        }

        planeManager.enabled = false;

        // Se desideri rimuovere i NavMesh quando i piani vengono eliminati
        foreach (var plane in args.removed)
        {
            var navSurface = plane.gameObject.GetComponent<NavMeshSurface>();
            if (navSurface != null)
            {
                Destroy(navSurface.gameObject);  // Elimina il piano generato e la NavMesh
                Debug.Log("NavMesh rimossa.");
            }
        }
    }
}
