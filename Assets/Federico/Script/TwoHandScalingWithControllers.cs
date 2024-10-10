using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TwoHandScalingWithControllers : MonoBehaviour
{
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable; // Riferimento all'oggetto XRGrabInteractable
    private UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor firstInteractor; // Il primo interattore (controller o mano)
    private UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor secondInteractor; // Il secondo interattore (controller o mano)
    private float initialDistance; // Distanza iniziale tra i due controller/mano
    private Vector3 initialScale; // Scala iniziale dell'oggetto
    private Vector3 currentScale; // Scala corrente durante la manipolazione

    private void OnEnable()
    {
        // Sottoscrivi gli eventi quando un interattore afferra l'oggetto
        grabInteractable.selectEntered.AddListener(OnSelectEntered);
        grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    private void OnDisable()
    {
        // Rimuovi gli eventi quando l'oggetto non è più attivo
        grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
        grabInteractable.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Se non c'è un primo interattore, lo assegniamo
        if (firstInteractor == null)
        {
            firstInteractor = args.interactorObject;
        }
        // Se c'è già un primo interattore, assegniamo il secondo interattore
        else if (secondInteractor == null)
        {
            secondInteractor = args.interactorObject;

            // Quando viene afferrato da due mani/controller, salviamo la distanza iniziale e la scala iniziale
            initialDistance = Vector3.Distance(firstInteractor.transform.position, secondInteractor.transform.position);
            initialScale = transform.localScale;
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        // Se il secondo interattore rilascia, lo resettiamo
        if (args.interactorObject == secondInteractor)
        {
            secondInteractor = null;
        }
        // Se il primo interattore rilascia, spostiamo il secondo al primo posto
        else if (args.interactorObject == firstInteractor)
        {
            firstInteractor = secondInteractor;
            secondInteractor = null;
        }

        // Dopo il rilascio di uno dei due controller, aggiorniamo la scala corrente
        if (firstInteractor == null && secondInteractor == null)
        {
            initialScale = currentScale; // Impostiamo la scala attuale come nuova scala iniziale
        }
    }

    private void Update()
    {
        // Se entrambi i controller/mano sono presenti, calcoliamo la nuova scala
        if (firstInteractor != null && secondInteractor != null)
        {
            // Calcola la distanza attuale tra i due interattori (controller/mano)
            float currentDistance = Vector3.Distance(firstInteractor.transform.position, secondInteractor.transform.position);
            // Fattore di scala in base al rapporto tra distanza attuale e iniziale
            float scaleMultiplier = currentDistance / initialDistance;
            // Applichiamo la nuova scala all'oggetto
            currentScale = initialScale * scaleMultiplier;
            transform.localScale = currentScale;
        }
    }
}
