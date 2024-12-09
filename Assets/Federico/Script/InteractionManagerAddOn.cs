using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class InteractionManagerAddOn : MonoBehaviour
{
    [SerializeField] public Material hoverMaterial;
    [SerializeField] public Material selectMaterial;
    [SerializeField] public SimulationManager _SimulationManager;
    [SerializeField] public MenuManager menuManager;
    
    public CharacterAnchorManager characterAnchorManager;
    public ConsoleDebugger debuggingWindow;

    private bool _objectHovered = false;
    private GameObject _currHoveredObj = null;
    private float _time = 0.0f;

    private bool _objectSelected = false;
    public bool interactionEnabled = false;
    private GameObject _currSelectedObj = null;
    [SerializeField] public GameObject span;
    public static EventHandler<EventArgs> onObjectSelected;
    // Start is called before the first frame update


    // GESTIONE ROTAZIONE DELLA POSA DEL PERSONAGGIO: 
    public bool characterCanRotate = false;

    private void OnEnable()
    {
        SimulationManager.startStoryboarding += DisableMoving;
        SimulationManager.pauseStoryboarding += EnableMoving;
       
    }
    private void OnDisable()
    {
        SimulationManager.startStoryboarding -= DisableMoving;
        SimulationManager.pauseStoryboarding -= EnableMoving;
   
    }

    private void OnDestroy()
    {
        SimulationManager.startStoryboarding -= DisableMoving;
        SimulationManager.pauseStoryboarding -= EnableMoving;
    }


    private void Start()
    {
        _SimulationManager = GameObject.Find("SimulationManager").GetComponent<SimulationManager>();
        if (_SimulationManager == null)
        {
            Debug.LogError("Simulation manager di :" + gameObject.name + " non trovato");
        }

        if (this.CompareTag("Player"))
        {
            characterAnchorManager = this.GetComponentInParent<CharacterAnchorManager>();
        }
        else
        {
            characterAnchorManager = this.GetComponent<CharacterAnchorManager>();
        }

        if (characterAnchorManager == null)
        {
            Debug.LogError("Character Anchor manager di :" + gameObject.name + " non trovato");
        }

        debuggingWindow = FindObjectOfType<ConsoleDebugger>();
        if (debuggingWindow == null)
        {
            Debug.LogError("ATTENZIONE CONSOLE DI DEBUGGING NON TROVATA");
        }

        if (characterAnchorManager == null)
        {
            Debug.LogError(" il componenete character Anchor Manager del personaggio non è stato configurato");
            //   debuggingWindow.SetText("componenete characterAnchorManager non trovato per l'oggetto"+this.gameObject.name);
        }
        else
        {
#if !UNITY_EDITOR
           characterAnchorManager.AttachObjectToAnchor();
#endif
        }

    }

    public void PlaceOnNavMesh()
    {
        NavMeshHit navMeshHit;
        // Prova a campionare una posizione sulla NavMesh a partire dalla posizione attuale dell'oggetto
        if (NavMesh.SamplePosition(transform.position, out navMeshHit, 9999.0f, NavMesh.AllAreas))
        {
            // Posiziona l'oggetto sulla NavMesh
            transform.position = navMeshHit.position;
            Debug.Log("Oggetto posizionato sulla NavMesh a: " + navMeshHit.position);
        }
        else
        {
            Debug.LogError("Impossibile trovare una posizione sulla NavMesh vicina.");
        }
    }

    public void DisableMoving(object sender, EventArgs obj)
    {

        if (this.name != null)
        {
            Debug.Log("l'oggetto" + this.name + "è pronto per la fase di storyboarding");
        }

        XRGrabInteractable _xrInt = this.GetComponentInParent<SnapToPlane>();
        if (_xrInt != null)
        {
            _xrInt.trackPosition = false;
            _xrInt.trackRotation = false;
            interactionEnabled = true;
        }
        else
        {
            Debug.LogError("componenete grabbable non trovato");
            debuggingWindow.SetText("Componente xrGRABBBABLE NON TROVATO ATTENTION!!");
        }

        EnableNavMeshAgent();
    }

    public void EnableNavMeshAgent()
    {
        if (this == null || gameObject == null)
        {
            Debug.LogError("Questo oggetto o il gameObject è stato distrutto in EnableNavMeshAgent.");
            return;
        }

        var navmeshAgent = this.GetComponent<NavMeshAgent>();
        var animator = this.GetComponent<Animator>();
        if (navmeshAgent != null && this.gameObject.CompareTag("Player"))
        {
            navmeshAgent.enabled = true;
            Debug.Log("Oggetto piazzato nella navmesh");
            PlaceOnNavMesh();
        }
        else
        {
            var navMeshObstacle = this.GetComponent<NavMeshObstacle>();
            if (navMeshObstacle != null)
            {
                navMeshObstacle.enabled = true;
            }
        }

        Debug.Log("Funzione EnableNavMeshAgent terminata con successo");
    }

    public void DisableNavMeshAgent()
    {
        var navmeshAgent = this.GetComponent<NavMeshAgent>();
        var animator = this.GetComponent<Animator>();
        if (navmeshAgent != null && this.gameObject != null && this.gameObject.CompareTag("Player"))
        {
            navmeshAgent.enabled = false;
        }
        else
        {
            var navMeshObstacle = this.GetComponent<NavMeshObstacle>();
            if (navMeshObstacle != null)
            {
                navMeshObstacle.enabled = false;
            }
        }
    }

    public void DestroyObject()
    {
#if !UNITY_EDITOR
        characterAnchorManager.DetachFromAnchor();
#endif
        if (this.CompareTag("Player"))
        {
            Destroy(this.transform.parent.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    public void EnableMoving(object sender, EventArgs obj)
    {
        // debuggingWindow.SetText("Sto prendendo il componente grabbable di" + this.name);

        XRGrabInteractable _xrInt = this.GetComponentInParent<SnapToPlane>();
        if (_xrInt != null)
        {
            if (_xrInt != null)
            {
                _xrInt.trackPosition = true;
                _xrInt.trackRotation = true;
                interactionEnabled = false;
            }

            DisableNavMeshAgent();
        }

    }

    // Deve essere void per poter essere visualizzata nell'Inspector
    public void onSelectionEnter(SelectEnterEventArgs args)
    {
        if (args.interactableObject != null)
        {
            _currSelectedObj = args.interactableObject.transform.gameObject;
            Debug.Log("Oggetto selezionato: " + _currSelectedObj.name);
        }
        else
        {
            Debug.LogError("args.interactableObject è null.");
        }

#if !UNITY_EDITOR
    Debug.Log("Verifica: characterAnchorManager = " + (characterAnchorManager != null));
    if (characterAnchorManager != null)
    {
        characterAnchorManager.DetachFromAnchor();
    }
    else
    {
        Debug.LogError("characterAnchorManager è NULL!");
    }
#endif

        if (!interactionEnabled)
        {
            Debug.Log("hai selezionato l'oggetto: " + this.gameObject.name);
            if (args.interactableObject != null)
            {
                _currSelectedObj = args.interactableObject.transform?.gameObject;
                Debug.Log("Oggetto selezionato: " + _currSelectedObj?.name);
            }
            else
            {
                Debug.LogError("args.interactableObject è NULL!");
            }
        }
        else
        {
            Debug.Log("Interaction enabled, procedo con l'invocazione.");
            if (onObjectSelected != null)
            {
                onObjectSelected.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Debug.LogError("onObjectSelected è NULL!");
            }

            if (args.interactableObject != null)
            {
                _currSelectedObj = args.interactableObject.transform?.gameObject;
                Debug.Log("Set Active Character chiamato per: " + _currSelectedObj?.name);
                if (_SimulationManager != null)
                {
                    _SimulationManager.SetActiveCharacter(_currSelectedObj);
                }
                else
                {
                    Debug.LogError("_SimulationManager è NULL!");
                }
            }
            else
            {
                Debug.LogError("args.interactableObject è NULL!");
            }
        }
    }


    public void OnSelectionExit(SelectExitEventArgs args)
    {
        if (args.interactableObject != null)
        {
            _currSelectedObj = args.interactableObject.transform.gameObject;
            Debug.Log("Oggetto selezionato: " + _currSelectedObj.name);
        }
        else
        {
            Debug.LogError("args.interactableObject è null.");
        }

#if !UNITY_EDITOR
        characterAnchorManager.AttachObjectToAnchor();
      //  debuggingWindow.SetText("Ancora instanziata nuovamente");
#endif
    }

    public void MenuObjectSelected()
    {
        var txt = this.transform.Find("Image/Text").GetComponent<Text>();
        if (txt == null)
        {
            Debug.LogError("l'oggetto selezionato non ha un campo nome errore");
            return;
        }

        menuManager.SelectObject(txt.text);
    }

    // ROTAZIONE DEL PERSONAGGIO
    public void SetCharacterRotationBool(bool value)
    {
        characterCanRotate = value;
    }

    public void RotatePlane(int rot)
    {
        float rotationAmount = 25f; // Angolo di rotazione incrementale

        // Determina la direzione della rotazione
        if (rot < 0)
        {
            rotationAmount = -rotationAmount;
        }

        // Aggiunge la rotazione incrementale sull'asse Y
        this.transform.rotation = Quaternion.Euler(
            this.transform.rotation.eulerAngles.x,
            this.transform.rotation.eulerAngles.y + rotationAmount,
            this.transform.rotation.eulerAngles.z
        );
    }

    public void ShowSpan()
    {
        if (span == null)
            return;
        span.SetActive(true);
    }

    public void HideSpan()
    {
        if (span == null)
            return;
        span.SetActive(false);
    }
   /*
    public void HidePanels(object sender, EventArgs e)
    {
        personaggioAttivo.SetActive(false);
        characterUI.SetActive(false);
        getControl.SetActive(false);
    }

    public void ShowPanels(object sender, EventArgs e)
    {
        personaggioAttivo.SetActive(true);
        characterUI.SetActive(true);
        getControl.SetActive(true);
    }
    */
    
}