using System;
using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.PlayerLoop;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.Hands;

public class RotationHandler : MonoBehaviour
{
    private Camera xrCamera; // Riferimento alla camera dell'XR Origin

    [SerializeField] private GameObject _handleUI; // Pannello UI da orientare
    [SerializeField] private Transform posizioneCorrispondente; // Posizione corrispondente (se necessaria)
    
    [SerializeField] private Vector3 rotationOffset = Vector3.zero; // Offset di rotazione personalizzabile per ogni pannello
    [SerializeField] private float distanceFromUser = 1.0f; // Distanza desiderata dal pannello all'utente

    private Vector3 initialPosition; // Posizione iniziale del pannello
    public bool enableRotationUpdate = true;

    public static EventHandler<EventArgs> onChangeOrientation;
    void Awake()
    {
        // Trova l'XR Origin in scena e ottieni la camera
        var xrOrigin = FindObjectOfType<XROrigin>();
        if (xrOrigin != null)
        {
            xrCamera = xrOrigin.Camera;
        }
        else
        {
            Debug.LogError("XR Origin non trovato in scena! Assicurati che esista un oggetto XR Origin.");
        }

        if (_handleUI != null && this.CompareTag("UIPanel"))
        {
            // Posiziona il pannello davanti all'utente
            // PositionPanelInFrontOfUser();
            // Salva la posizione iniziale del pannello
            initialPosition = _handleUI.transform.position;
        }
        else if(_handleUI!=null)
        {
            initialPosition = _handleUI.transform.position;
        }
        else
        {
            Debug.LogError("_handleUI non è assegnato!");
        }

     //   onChangeOrientation += OrientTowardsUserForEvent;
    }

    private void LateUpdate()
    {
        if(enableRotationUpdate)
        {     OrientTowardsUser();}
    }

    void OnEnable()
    {
        Debug.Log($"OnEnable chiamato per {this.gameObject.name}");
        /*
        if (xrCamera != null)
        {
            string name = "";
            if (this.name == "front")
            {
                name = "pannelloUIpersonaggio";
            }
            else
            {
                name = this.name;
            }
            Debug.Log("chiamo al orientowardsuser per l'oggetto"+name);
            OrientTowardsUser();
        }
        else
        {
            Debug.LogError("Camera dell'XR Origin non trovata! Verifica che l'XR Origin sia configurato correttamente.");
        }
        */
        
        if (this.CompareTag("Player"))
        {
            CharacterManager.OnDestinationReached += AdjustPosition;
        }

        enableRotationUpdate = true;
    }

    private void OnDisable()
    {
        if (this.CompareTag("Player"))
        {
            CharacterManager.OnDestinationReached -= AdjustPosition;
        }

        enableRotationUpdate = false;
    }

    public void OnSelectionEnter(SelectEnterEventArgs args)
    {
        Debug.Log("rotazione disabilitata");
        enableRotationUpdate = false;
    }

    public void OnSelectionExit(SelectExitEventArgs args)
    {
        Debug.Log("rotazione riabilitata");
        if (this.enabled = true)
        {
            Debug.Log("oggetto attivo reimposto l'update della rotazione");
            enableRotationUpdate = true;
        }
        else
        {
            Debug.Log("oggetto disattivato ");
            enableRotationUpdate = false;
        }
    }
    public void OnDestroy()
    {
        onChangeOrientation -= OrientTowardsUserForEvent;
    }

    public void ResetInitialPosition(SelectExitEventArgs args)
    {
        initialPosition = args.interactableObject.transform.position;
    }

    private void AdjustPosition(object obj, EventArgs e)
    {
        Debug.Log("Adjust position chiamata");
        enableRotationUpdate = false;
        if (_handleUI != null && posizioneCorrispondente != null)
        {
            _handleUI.transform.position = posizioneCorrispondente.position;
            // Aggiorna la posizione iniziale
            initialPosition = _handleUI.transform.position;
        }
        else
        {
            Debug.LogError("Assicurati che _handleUI e posizioneCorrispondente siano assegnati.");
        }

        enableRotationUpdate = true;
    }
    
    public void OrientTowardsUser()
    {
        //initialPosition = _handleUI.transform.position;
        if (_handleUI == null)
        {
            Debug.LogError("_handleUI non è assegnato!");
            return;
        }
       // Debug.Log("orientowardUser chiamata");
        // Calcola la direzione verso l'utente
        Vector3 directionToUser = xrCamera.transform.position - _handleUI.transform.position;

        // Mantieni solo la direzione sull'asse X-Z (orizzontale), azzerando la Y
        directionToUser.y = 0;

        // Verifica che la direzione non sia nulla
        if (directionToUser.sqrMagnitude > 0.001f)
        {
            // Calcola la rotazione per guardare l'utente
            Quaternion targetRotation = Quaternion.LookRotation(directionToUser);

            // Applica l'offset di rotazione personalizzato
            targetRotation *= Quaternion.Euler(rotationOffset);

            // Mantieni solo la componente Y della rotazione
            _handleUI.transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

            // Reimposta la posizione iniziale per evitare spostamenti
            _handleUI.transform.position = initialPosition;

         //   Debug.Log($"Oggetto {_handleUI.name} orientato verso l'utente con offset. Rotazione Y: {_handleUI.transform.rotation.eulerAngles.y}");
        }
        else
        {
            Debug.LogWarning("La direzione verso l'utente è nulla. Assicurati che l'oggetto e la camera non siano nella stessa posizione.");
        }
    }

    public void OrientTowardsUserForEvent(object sender, EventArgs e)
    {
        initialPosition = _handleUI.transform.position;
      
        if (_handleUI == null)
        {
            Debug.LogError("_handleUI non è assegnato!");
            return;
        }
        Debug.Log("orientowardUser chiamata");
        // Calcola la direzione verso l'utente
        Vector3 directionToUser = xrCamera.transform.position - _handleUI.transform.position;

        // Mantieni solo la direzione sull'asse X-Z (orizzontale), azzerando la Y
        directionToUser.y = 0;

        // Verifica che la direzione non sia nulla
        if (directionToUser.sqrMagnitude > 0.001f)
        {
            // Calcola la rotazione per guardare l'utente
            Quaternion targetRotation = Quaternion.LookRotation(directionToUser);

            // Applica l'offset di rotazione personalizzato
            targetRotation *= Quaternion.Euler(rotationOffset);

            // Mantieni solo la componente Y della rotazione
            _handleUI.transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

            // Reimposta la posizione iniziale per evitare spostamenti
            _handleUI.transform.position = initialPosition;

            Debug.Log($"Oggetto {_handleUI.name} orientato verso l'utente con offset. Rotazione Y: {_handleUI.transform.rotation.eulerAngles.y}");
        }
        else
        {
            Debug.LogWarning("La direzione verso l'utente è nulla. Assicurati che l'oggetto e la camera non siano nella stessa posizione.");
        }
    }
    private void PositionPanelInFrontOfUser()
    {
        if (xrCamera == null)
        {
            Debug.LogError("xrCamera non è assegnata!");
            return;
        }
        Debug.Log("procedo ad eseguire la position Panel in front of user per l'oggetto"+this.gameObject.name);
        // Calcola la posizione davanti all'utente
        Vector3 positionInFrontOfUser = xrCamera.transform.position + xrCamera.transform.forward * distanceFromUser;

        // Imposta la posizione del pannello
        _handleUI.transform.position = positionInFrontOfUser;

        // Allinea il pannello all'altezza della camera (opzionale)
        _handleUI.transform.position = new Vector3(_handleUI.transform.position.x, xrCamera.transform.position.y, _handleUI.transform.position.z);
    }
}
