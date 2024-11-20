using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.Feedback;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SnapToPlane : XRGrabInteractable
{
    [SerializeField] public ARPlaneManager planeManager;
    [SerializeField] private float snapDistanceThreshold = 0.2f; // Distanza alla quale avviene lo snap
    private bool isBeingManipulated = false; // Aggiungi questa variabile

    private ARPlane closestPlane;
    private bool isSnapping = false;
    
    
    protected void Start()
    {
       planeManager= GameObject.Find("XR Origin (XR Rig)").GetComponent<ARPlaneManager>();
       if (planeManager == null)
       {
           Debug.LogError("planemanager non trovato");
       }

       IXRInteractor leftInteractor = GameObject.Find("XR Origin (XR Rig)/Camera Offset/Left Controller/Near-Far Interactor").GetComponent<IXRInteractor>();
       IXRInteractor righInteractor = GameObject.Find("XR Origin (XR Rig)/Camera Offset/Right Controller/Near-Far Interactor").GetComponent<IXRInteractor>();
       if (leftInteractor == null || righInteractor == null)
       {
           Debug.LogError("non ho reperito i near far interactor components");
       }
       var hapticFeedbackComponents = this.gameObject.GetComponents<SimpleHapticFeedback>();
       if (hapticFeedbackComponents.Length != 0)
       {
           hapticFeedbackComponents[0].enabled = true;
           hapticFeedbackComponents[1].enabled = true;
           hapticFeedbackComponents[0].SetInteractorSource(leftInteractor);
           hapticFeedbackComponents[1].SetInteractorSource(righInteractor);
       }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        selectExited.AddListener(OnSelectExitedHandler);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        selectExited.RemoveListener(OnSelectExitedHandler);
    }
    
    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        isBeingManipulated = true; // Indica che l'oggetto è in fase di manipolazione
    }
    
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        isBeingManipulated = false; // Indica che l'oggetto non è più in fase di manipolazione
        // Calcola la distanza dal piano più vicino quando l'oggetto viene rilasciato
        if (isSnapping && closestPlane != null)
        {
            SnapToClosestPlane();
        }
    }

    private void Update()
    {
        /*
        // Verifica se l'oggetto è abbastanza vicino a un piano
        if (isSelected && TryGetClosestPlane(out closestPlane) && !isBeingManipulated)
        {
            // Se il piano più vicino è entro la distanza di snap, imposta isSnapping su true
            float distanceToPlane = Vector3.Distance(transform.position, closestPlane.transform.position);
            isSnapping = distanceToPlane <= snapDistanceThreshold;
        }
        else
        {
            isSnapping = false;
        }
        */
    }

    private bool TryGetClosestPlane(out ARPlane closestPlane)
    {
        closestPlane = null;
        float minDistance = float.MaxValue;

        foreach (var plane in planeManager.trackables)
        {
            float distance = Vector3.Distance(transform.position, plane.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPlane = plane;
            }
        }

        return closestPlane != null;
    }
    private void OnSelectExitedHandler(SelectExitEventArgs args)
    {
        if (isSnapping && closestPlane != null)
        {
            SnapToClosestPlane();
        }
    }

    public override string ToString()
    {
        string res;
        res = "Questo oggetto ha il componente SnapToPlane attivo ed operativo";
        return res;
    }
   
    

    private void SnapToClosestPlane()
    {
        if (closestPlane == null)
            return;

        Vector3 planePosition = closestPlane.transform.position;
        Vector3 planeNormal = closestPlane.transform.up;
        Vector3 objectPosition = transform.position;
        if (Vector3.Dot(planeNormal, Vector3.down) > 0.5f && transform.name == "Chandelier")
        {
            planeNormal = -planeNormal; // Inverti la normale
        }
        float distance = Vector3.Dot(planeNormal, objectPosition - planePosition);
        transform.position = objectPosition - distance * planeNormal;

        // Assicurati che l'oggetto sia allineato
        transform.rotation = Quaternion.LookRotation(transform.forward, planeNormal);

        if (Mathf.Abs(distance) < snapDistanceThreshold)
        {
            Debug.Log("Oggetto" +this.name+ " agganciato al piano.");
            isSnapping = false; // Imposta snapping su false per continuare a manipolare l'oggetto
        }
    }

}
