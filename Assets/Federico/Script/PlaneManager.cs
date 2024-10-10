using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaneManager : MonoBehaviour
{
  
    [SerializeField] private ARPlaneManager planeManager;

    void OnEnable()
    {
        planeManager.planesChanged += OnPlanesChanged;
    }

    void OnDisable()
    {
        planeManager.planesChanged -= OnPlanesChanged;
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        foreach (var plane in args.added)
        {
            // Controlla la classificazione del piano
            var planeClassification = plane.GetComponent<ARPlane>().classification;

            // Disabilita il tracking per i piani che non ti interessano
            if (IsUnwantedPlane(planeClassification))
            {
                plane.gameObject.SetActive(false); // Disabilita il piano
            }
        }
    }

    private bool IsUnwantedPlane(PlaneClassification classification )
    {
        if (classification == PlaneClassification.Other ||
            classification == PlaneClassification.None)
        {
            return true;
        }

        // Esempio di classificazione da disabilitare
        return classification != PlaneClassification.Seat;
        // || classification != PlaneClassification.Table;
    }
}

