using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class InteractionManagerAddOn : MonoBehaviour
{
    [SerializeField] public Material hoverMaterial;
    [SerializeField] public Material selectMaterial;
    [SerializeField] public SimulationManager _SimulationManager;
    [SerializeField] public MenuManager menuManager;
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
    }

    void Start()
    {
        _SimulationManager=  GameObject.Find("SimulationManager").GetComponent<SimulationManager>() ;
        if (_SimulationManager==null)
        {
            Debug.LogError("Simulation manager di :" + gameObject.name + " non trovato");
        }

        SimulationManager.startStoryboarding += DisableMoving;
        SimulationManager.pauseStoryboarding += EnableMoving;
    }

    // Update is called once per frame
    void Update()
    {
      /*
        if (_objectHovered)
        {
            _time += Time.deltaTime;
            if (_time > 3.0f && _currHoveredObj!=null )
            {
                _currHoveredObj.transform.Find("Front").gameObject.SetActive(true);
                if (_currHoveredObj.transform.Find("Front") == null)
                {
                    Debug.Log("UI non trovata");
                }
                // reset del menu 
                _time = 0;
                _objectHovered = false;
            }
        }
        */
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
         XRGrabInteractable _xrInt = this.GetComponentInParent<SnapToPlane>();
         if (_xrInt != null)
         {
             _xrInt.trackPosition = false;
             _xrInt.trackRotation = false;
             _xrInt.trackScale = false;
             
             interactionEnabled = true;
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
        Debug.Log("hai selezionato l'oggetto: " +this.gameObject.name);
        _currSelectedObj = args.interactableObject.transform.gameObject;
        if (_currSelectedObj == null)
        {
            Debug.LogError("On Selection Enter "+ this.name+ "null reference exception");
            return;
        }
       if (interactionEnabled)
       {
            onObjectSelected.Invoke(this,EventArgs.Empty);
           Debug.Log("Set Active character chiamata");
           _SimulationManager.SetActiveCharacter(_currSelectedObj);
           //interactionEnabled = false;
       }
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
    public void onSelection(SelectEnterEventArgs args)
    {
        Debug.Log("Hai selezionato l'oggetto");
        var obj = args.interactableObject.transform.gameObject;
        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = selectMaterial;

        }
       
    }

    public void onselectionExit(SelectExitEventArgs args)
    {
        Debug.Log("Hai deselezionato l'oggetto");
        if (_currSelectedObj != null)
        {
            XRGrabInteractable _xrInt = _currSelectedObj.GetComponent<XRGrabInteractable>();
            _xrInt.enabled = true;
            
          //  _currSelectedObj.transform.Find("Front").gameObject.SetActive(false);
            _currSelectedObj = null;
        }
    }
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

 
}