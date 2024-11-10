using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BoundingBoxInteractionManager : MonoBehaviour
{
    public SimulationManager simulationManager;
    
    public GameObject labelUI;

    public SnapToPlane interactable;

    public SnapToPlane boundingBoxPlane;

    public GameObject boundingBoxplane;

    public Vector3 boundingBoxSize=Vector3.zero; // dimensione della bounding box aggiornata 

    public Vector3 initialPlaneLocalPosition; //posizione locale iniziale del piano

    public bool interactionEnabled;
    
    private GameObject _currSelectedObj = null;

    public bool planeRotation = false;

    public List<ObjectType> oggettiInScena;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public class ObjectType
    {
        public string type;
        public List<String> possibleStates;
        public string currentState;

        public ObjectType(string objectType,List<String> objectPossibleStates, string objectcurrentState)
        {
            type = objectType;
            possibleStates = objectPossibleStates;
            currentState = objectcurrentState;
        }
    }

    private Dictionary<String, ObjectType> ObjectsMap=new Dictionary<String, ObjectType>();
    
    public void Awake()
    {
            BoundingBoxManagerUI.OnBoundingBoxPlacementCompleted += AllowLabeling;
            BoundingBoxManagerUI.OnSceneInizializationCompleted += StopShowingLabeling;
            SimulationManager.startStoryboarding += DisableMoving;
            ControllerManager.OnBoundingBoxPlaneEdit += AllowEditPlane;
            ControllerManager.StopBoundingBoxPlaneEdit += StopEditPlane;
            simulationManager = FindObjectOfType<SimulationManager>();
            Debug.Log("Iscrizione all'evento OnBoundingBoxPlacement completata");
            labelUI = this.transform.Find("BoundingBoxLabel").gameObject;
            interactable = this.GetComponent<SnapToPlane>();
            if (interactable == null || labelUI==null)
            {
                Debug.LogError("errore nel reperimento del componente Snap To Plane o della label o del piano");
            }
            boundingBoxplane = this.transform.Find("Plane").gameObject;
            // qui andiamo ad aggiungere tutte le label degli oggetti, per ogni oggetti specifichiamo i campi con cui andare a corredare 
            // poi il character manager e lo state con tutte le informazioni necessarie.  
             List<String> possibleStates = new List<String>();
             List<ObjectType> objects = new List<ObjectType>();
             //chair 
             possibleStates.Add("lifted");
             possibleStates.Add("fallen");
             objects.Add(new ObjectType("chair", possibleStates, "lifted"));
             possibleStates.Clear();
             ObjectsMap.Add("chair",objects[0]);
             //wardrobe
             possibleStates.Add("closed");
             possibleStates.Add("open");
             objects.Add(new ObjectType("wardrobe", possibleStates, "closed"));
             possibleStates.Clear();
             ObjectsMap.Add("wardrobe",objects[1]);

           // SetLabel("chair");
    }

    public void OnSelectionEnter(SelectEnterEventArgs args)
    {
        Debug.Log("hai selezionato l'oggetto: " +this.gameObject.name);
        _currSelectedObj = this.gameObject;
        if (interactionEnabled)
        {
            simulationManager.SetActiveCharacter(_currSelectedObj);
            interactionEnabled = false;
        }
    }
    
    
  
    public void ObjectHoveredEntered(HoverEnterEventArgs args)
    {
        if (labelUI == null)
        {
            args.interactableObject.transform.Find("BoundingBoxLabel").gameObject.SetActive(true);
            return;
        }
        labelUI.SetActive(true);
    }

    
    public void ObjectHoveredExited(HoverExitEventArgs args)
    {
        if (labelUI == null)
        {
            args.interactableObject.transform.Find("BoundingBoxLabel").gameObject.SetActive(false);
            return;
        }
        labelUI.SetActive(false);
        
    }
    // OPERAZIONI LABEL BOUNDING BOX
    public void SetLabel(string text)
    {
        Text label= labelUI.transform.Find("Scrollview Canvas/Pannello/Viewport/Content/Scroll View Item/Text").GetComponent<Text>();
        label.text = text;
        //assegnazione del tipo
        this.GetComponent<CharacterManager>().type = text.ToLower() ;
        State objectState = this.GetComponent<State>();
        ObjectType obj;
        //assegnazione degli stati
        if (ObjectsMap.TryGetValue(text.ToLower(),out obj))
        {
            objectState.state.Clear();
            objectState.state.Add(obj.currentState);
            objectState.possibleStates.Clear();
            objectState.possibleStates = obj.possibleStates ;
        }
        
        if (isPlaneNeeded(text.ToLower())&& boundingBoxplane!=null)
        {
            boundingBoxplane.SetActive(true);
        }
    }
    

    public void StopShowingLabeling(object sender,EventArgs e)
    {
        this.transform.Find("Front").gameObject.SetActive(false);
        this.GetComponent<SnapToPlane>().selectEntered.RemoveListener(DisplayLabeling);
    }
    

    public void AllowLabeling(object  sender,EventArgs obj)
    { 
      
        DisableMoving(this,EventArgs.Empty);
        this.GetComponent<SnapToPlane>().selectEntered.AddListener(DisplayLabeling);
    }

    
    public void DisplayLabeling(SelectEnterEventArgs args)
    {
       var front= this.transform.Find("Front");
       front.gameObject.SetActive(true);
    }
    
    public void DisableMoving(object sender,EventArgs e)
    {
        Debug.Log("Disable Moving La bounding box non è più movibile");
        XRGrabInteractable _xrInt = this.GetComponentInParent<SnapToPlane>();
        if (_xrInt != null)
        {
            _xrInt.trackPosition = false;
            _xrInt.trackRotation = false;
            _xrInt.trackScale = false;
        }

        var navmeshObstacle = this.GetComponent<NavMeshObstacle>();
        if (navmeshObstacle != null)
        {
            navmeshObstacle.enabled = false;
        }

        interactionEnabled = true;
    }

    public void EnablePlaneRotation()
    {
        planeRotation = true;
    }
    
    public void DisablePlaneRotation()
    {
        planeRotation = false;
    }
    public void AllowEditPlane(object sender,EventArgs e)
    {
        interactable.transform.Find("BoundingBoxWrapper/Cube").GetComponent<BoxCollider>().enabled = false;
        SetInteractionLayer(LayerMask.NameToLayer("InteractionDisabled"),interactable);
      //  SetInteractionLayer(LayerMask.NameToLayer("Default"),boundingBoxPlane);
        
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
        boundingBoxplane.transform.rotation = Quaternion.Euler(
            boundingBoxplane.transform.rotation.eulerAngles.x,
            boundingBoxplane.transform.rotation.eulerAngles.y + rotationAmount,
            boundingBoxplane.transform.rotation.eulerAngles.z
        );
    }
    public void StopEditPlane(object sender, EventArgs e)
    {
        interactable.transform.Find("BoundingBoxWrapper/Cube").GetComponent<BoxCollider>().enabled = true;
        SetInteractionLayer(LayerMask.NameToLayer("Default"),interactable);
    }
    
    
    public void SetInteractionLayer(int layerIndex,SnapToPlane obj)
    {
        // Cambia l'Interaction Layer Mask con il layer specificato
        obj.interactionLayers = 1 << layerIndex;
        Debug.Log("Interaction Layer Mask cambiata al layer: " + layerIndex);
    }

    public void setBoundingBoxSize(Vector3 size)
    {
        boundingBoxSize = size;
    }

    public bool isPlaneNeeded(string label)
    {
        if (label == "chair" || label == "table")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    private void OnRelease(SelectExitEventArgs args)
    {
        // Quando il piano viene rilasciato, assicurati che non sia uscito dai confini
        // ConstrainPlaneMovement();
    }
    
}
