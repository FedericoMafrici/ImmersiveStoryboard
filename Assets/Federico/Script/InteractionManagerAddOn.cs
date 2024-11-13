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
    private ARAnchor _currentAnchor;
    private bool _objectHovered = false;
    private GameObject _currHoveredObj = null;
    private float _time = 0.0f;
    
    private bool _objectSelected = false;
    public bool interactionEnabled = false;
    private GameObject _currSelectedObj = null;

    public static EventHandler<EventArgs> onObjectSelected;
    // Start is called before the first frame update
    private void Start()
    {
       _SimulationManager=  GameObject.Find("SimulationManager").GetComponent<SimulationManager>() ;
       if (_SimulationManager==null)
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
       debuggingWindow= FindObjectOfType<ConsoleDebugger>();
       
       SimulationManager.startStoryboarding += DisableMoving;
       SimulationManager.pauseStoryboarding += EnableMoving;
       if (characterAnchorManager == null)
       {
           Debug.LogError(" il componenete character Anchor Manager del personaggio non è stato configurato");
       }
       else
       {
           characterAnchorManager.CreateAnchor();     
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
         Debug.Log("l'oggetto"+ this.name +"è pronto per la fase di storyboarding");
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
       // debuggingWindow.SetText("Hai interagito con il personaggio interaction settata a" +interactionEnabled);
       // debuggingWindow.SetText(this.transform.parent.gameObject.name);
        characterAnchorManager.DestroyAnchor();
        if (this.GetComponent<SnapToPlane>() != null)
        {
          //  debuggingWindow.SetText(this.GetComponent<SnapToPlane>().ToString());
        }
        else
        {
         //   debuggingWindow.SetText("componenete SnapToPlane non trovato: oggetto" + this.gameObject.name + " padre" + this.transform.parent.gameObject.name);
        }
        if (!interactionEnabled)
        {
            Debug.Log("hai selezionato l'oggetto: " + this.gameObject.name);
            _currSelectedObj = args.interactableObject.transform.gameObject;
            
        }
        else
        {
            //debuggingWindow.SetText("Ho chiamato la Set Active Character per l'oggetto +"+ _currSelectedObj.name);
            onObjectSelected.Invoke(this,EventArgs.Empty);
            _currSelectedObj = args.interactableObject.transform.gameObject;
            Debug.Log("Set Active character chiamata nome oggetto"+_currSelectedObj.gameObject.name + "\n figli:"+_currSelectedObj.transform.GetChild(0).name);
           _SimulationManager.SetActiveCharacter(_currSelectedObj);
           //interactionEnabled = false;
       }
    }
    public void OnSelectionExit(SelectExitEventArgs args)
    {
        characterAnchorManager.CreateAnchor();
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
}