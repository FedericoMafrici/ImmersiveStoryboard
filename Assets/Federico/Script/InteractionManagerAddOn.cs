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
    public ConsoleDebugger debuggingWindow;
    private ARAnchor _currentAnchor;
    private bool _objectHovered = false;
    private GameObject _currHoveredObj = null;
    private float _time = 0.0f;
    
    private bool _objectSelected = false;
    public bool interactionEnabled = false;
    private GameObject _currSelectedObj = null;

    public static EventHandler<EventArgs> onObjectSelected;
    // Start is called before the first frame update
    private void Awake()
    {
       _SimulationManager=  GameObject.Find("SimulationManager").GetComponent<SimulationManager>() ;
       if (_SimulationManager==null)
       {
           Debug.LogError("Simulation manager di :" + gameObject.name + " non trovato");
       }
     //  CreateAnchor();
       debuggingWindow= FindObjectOfType<ConsoleDebugger>();
       SimulationManager.startStoryboarding += DisableMoving;
       SimulationManager.pauseStoryboarding += EnableMoving;
       
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
        Debug.Log("l'oggetto è pronto per la fase di storyboarding");
         XRGrabInteractable _xrInt = this.GetComponentInParent<SnapToPlane>();
         if (_xrInt != null)
         {
             _xrInt.trackPosition = false;
             _xrInt.trackRotation = false;
             _xrInt.trackScale = false;
             
             interactionEnabled = true;
         }
         else
         {
             Debug.LogError("componenete grabbable non trovato");
             debuggingWindow.SetText("Componente xrGRABBBABLE NON TROVATO ATTENTION!!");
         }

         if (this.gameObject.CompareTag("Player"))
         {
             var navmeshAgent = this.GetComponent<NavMeshAgent>();
             var animator = this.GetComponent<Animator>();
             if (navmeshAgent != null && this.gameObject.CompareTag("Player"))
             { 
                 navmeshAgent.enabled = true;
                PlaceOnNavMesh();   
             }

             if (animator != null)
             {
                 animator.enabled = true;
             }
         }
         else
         {
             var navMeshObstacle = this.GetComponent<NavMeshObstacle>();
             if (navMeshObstacle != null)
             {
                 navMeshObstacle.enabled = true;
             }
         }
         
    }

    public void EnableMoving(object sender, EventArgs obj)
    {
        XRGrabInteractable _xrInt = this.GetComponentInParent<SnapToPlane>(); 
        if (_xrInt != null)
        {
            _xrInt.trackPosition = true;
            _xrInt.trackRotation = true;
            _xrInt.trackScale = true;
            
            interactionEnabled = false;
        }

        if (this.gameObject.CompareTag("Player"))
        {
            var navmeshAgent = this.GetComponent<NavMeshAgent>();
            var animator = this.GetComponent<Animator>();
            if (navmeshAgent != null && this.gameObject.CompareTag("Player"))
            {
                navmeshAgent.enabled = false;
            }

            if (animator != null)
            {
                animator.enabled = false;
            }
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
    // Deve essere void per poter essere visualizzata nell'Inspector
    public void onSelectionEnter(SelectEnterEventArgs args)
    {
        debuggingWindow.SetText("Hai interagito con il personaggio interaction settata a" +interactionEnabled);
        debuggingWindow.SetText(this.transform.parent.gameObject.name);
        if (!interactionEnabled)
        {
            Debug.Log("hai selezionato l'oggetto: " + this.gameObject.name);
            _currSelectedObj = args.interactableObject.transform.gameObject;
           // Destroy(_currentAnchor.gameObject);
            _currentAnchor = null;
            transform.parent = null; // Rimuove la parentela per la manipolazione libera
            
        }
        else
        {
            debuggingWindow.SetText("Ho chiamato la Set Active Character");
            onObjectSelected.Invoke(this,EventArgs.Empty);
           Debug.Log("Set Active character chiamata");
           _SimulationManager.SetActiveCharacter(_currSelectedObj);
           //interactionEnabled = false;
       }
    }
    public void OnSelectionExit(SelectExitEventArgs args)
    {
        // Crea una nuova ancora alla posizione attuale quando l'oggetto viene rilasciato
       // CreateAnchor();
    }
    
    public void MenuObjectSelected()
    {
    
        var txt=  this.transform.Find("Image/Text").GetComponent<Text>();
        
        if (txt == null)
        {
            Debug.LogError("l'oggetto selezionato non ha un campo nome errore");
            return;
        }
        menuManager.SelectObject(txt.text);
    
    }
    
    
    //Probabilmente non usate queste due 
    public void onHoverEnter(HoverEnterEventArgs args)
    {
        Debug.Log("hai appena iniziato a guardare l'oggetto");
        _time = 0.0f;
      //  _objectHovered = true;
        _currHoveredObj = args.interactableObject.transform.gameObject;
        
    }
    public void onHover(HoverEnterEventArgs args)
    {
     Debug.Log("stai guardando l'oggteto");
     //TODO 
    }
    
    public void onHoverExit(HoverExitEventArgs args)
    {
        Debug.Log("non stai piu guardando l'oggetto");
        _objectHovered = false;
        _time = 0.0f;
        _currHoveredObj= null;
    }
//@@@@@@ GESTIONE FUNZIONI ANCORAGGIO @@@@@@@
    private async void CreateAnchor()
    {
        // Controlla se esiste già un'ancora e la rimuove se presente
        if (_currentAnchor != null)
        {
            Destroy(_currentAnchor);
        }

        // Crea una nuova ancora alla posizione e rotazione attuali
        ARAnchorManager anchorManager = FindObjectOfType<ARAnchorManager>();
        if (anchorManager != null)
        {
            var result = await anchorManager.TryAddAnchorAsync(new Pose(transform.position, transform.rotation));
            if (result.status.IsSuccess())
            {
                _currentAnchor = result.value; // Assegna l'ancora creata a _currentAnchor
                transform.parent = _currentAnchor.transform; // Rende l'oggetto figlio dell'ancora
                Debug.Log("Ancora creata e assegnata con successo.");
            }
            else
            {
                Debug.LogWarning("Impossibile creare un'ancora alla posizione specificata.");
            }
        }
    }
    
 
}