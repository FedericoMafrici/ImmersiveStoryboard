using System;
using System.Collections.Generic;
using System.Linq;
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
   
    private CharacterAnchorManager _characterAnchorManager;

    private SnapToPlane _snapToPlaneComponent;

    public static EventHandler<EventArgs> onPlaneSpawned;

    public Material featheredPlane;
    //[SerializeField] private BoundingBoxInteractionManager interactionManagerAddOn;
    
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

    private bool labelChoosen = false;

    public void OnDestroy()
    {
        BoundingBoxManagerUI.OnBoundingBoxPlacement -= AllowMoving;
       //  BoundingBoxManagerUI.OnBoundingBoxPlacementCompleted -= DisableMoving;
        BoundingBoxManagerUI.OnBoundingBoxAllowLabel -= AllowLabeling;
        SimulationManager.startStoryboarding -= DisableMoving;
        ControllerManager.OnBoundingBoxPlaneEdit -= AllowEditPlane;
        ControllerManager.StopBoundingBoxPlaneEdit -= StopEditPlane;
        SimulationManager.startStoryboarding -=DontShowLabelOnHovering;
        SimulationManager.pauseStoryboarding -= ShowLabelOnHovering;
        ScreenshotManager.OnHidePanels -= HideBoundingBoxMaterial;
        ScreenshotManager.screenShotTaken -= ShowBoundingBoxMaterial; 
    }

    public void Awake()
    {
        BoundingBoxManagerUI.OnBoundingBoxPlacement += AllowMoving;
      //  BoundingBoxManagerUI.OnBoundingBoxPlacementCompleted += DisableMoving;
        BoundingBoxManagerUI.OnBoundingBoxAllowLabel += AllowLabeling;
      //  BoundingBoxManagerUI.OnSceneInizializationCompleted += StopShowingLabeling;
        SimulationManager.startStoryboarding += DisableMoving;
        SimulationManager.startStoryboarding += AllowLabeling;
        ControllerManager.OnBoundingBoxPlaneEdit += AllowEditPlane;
        ControllerManager.StopBoundingBoxPlaneEdit += StopEditPlane;
        SimulationManager.startStoryboarding+=DontShowLabelOnHovering;
        SimulationManager.pauseStoryboarding += ShowLabelOnHovering;
        ScreenshotManager.OnHidePanels += HideBoundingBoxMaterial;
        ScreenshotManager.screenShotTaken += ShowBoundingBoxMaterial; 
        // ACQUISIZIONE DEI COMPONENTI PRINCIPALI

        _snapToPlaneComponent = this.GetComponent<SnapToPlane>();
        if (_snapToPlaneComponent == null)
        {
            Debug.LogError("Componente SnapToPlane non trovato errore!");
        }
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
        objects.Add(new ObjectType("chair", possibleStates.ToList(), "lifted"));
        possibleStates.Clear();
        ObjectsMap.Add("chair",objects[0]);
        //wardrobe
        possibleStates.Add("closed");
        possibleStates.Add("open");
        objects.Add(new ObjectType("wardrobe", possibleStates.ToList(), "closed"));
        possibleStates.Clear();
        ObjectsMap.Add("wardrobe",objects[1]);
        //coffe machine
        possibleStates.Add("broken");
        possibleStates.Add("fixed");
        objects.Add(new ObjectType("coffe machine", possibleStates.ToList(), "broken"));
        ObjectsMap.Add("coffe machine",objects[2]);
        possibleStates.Clear();
        //gestione ancore
        _characterAnchorManager = this.GetComponent<CharacterAnchorManager>();
             if (_characterAnchorManager == null)
             {
                 Debug.LogError("Character Anchor manager di :" + gameObject.name + " non trovato");
             }
        _snapToPlaneComponent.hoverEntered.AddListener(ObjectHoveredEntered);
        _snapToPlaneComponent.hoverExited.AddListener(ObjectHoveredExited);
       // SetLabel("wardrobe");
       DisableMoving(this,EventArgs.Empty);
    }

    private void HideBoundingBoxMaterial(object sender, EventArgs e)
    {
        if (featheredPlane != null)
        {
            // Assicurati che il nome della proprietà sia corretto
            string propertyName = "_TexTintColor"; // Modifica il nome della proprietà se necessario

            if (featheredPlane.HasProperty(propertyName))
            {
                // Ottieni il colore attuale
                Color currentColor = featheredPlane.GetColor(propertyName);

                // Modifica l'alpha del colore
                currentColor.a = 0;

                // Assegna il colore aggiornato al materiale
                featheredPlane.SetColor(propertyName, currentColor);

                Debug.Log($"La trasparenza del materiale è stata impostata a 0 per la proprietà {propertyName}.");
            }
            else
            {
                Debug.LogError($"La proprietà {propertyName} non è presente nel materiale.");
            }
        }
        else
        {
            Debug.LogError("Il materiale featheredPlane non è assegnato.");
        }
    }
    private void ShowBoundingBoxMaterial(object sender, EventArgs e)
    {
        if (featheredPlane != null)
        {
            // Assicurati che il nome della proprietà sia corretto
            string propertyName = "_TexTintColor"; // Modifica il nome della proprietà se necessario

            if (featheredPlane.HasProperty(propertyName))
            {
                // Ottieni il colore attuale
                Color currentColor = featheredPlane.GetColor(propertyName);

                // Modifica l'alpha del colore
                currentColor.a = 1.0f;

                // Assegna il colore aggiornato al materiale
                featheredPlane.SetColor(propertyName, currentColor);

                Debug.Log($"La trasparenza del materiale è stata impostata a 0 per la proprietà {propertyName}.");
            }
            else
            {
                Debug.LogError($"La proprietà {propertyName} non è presente nel materiale.");
            }
        }
        else
        {
            Debug.LogError("Il materiale featheredPlane non è assegnato.");
        }
    }
    public void HideCurrentLabel()
    {
        if (labelUI == null)
        {
            Debug.LogError("Non è possibile chiamare la HideCurrentLabel perchè labelUI è nullo");
            return;
        }

        labelUI.SetActive(false);

    }

    public void ShowLabelOnHovering(object sender,EventArgs e)
    {
        this.transform.Find("Front").gameObject.SetActive(false);
        this.GetComponent<SnapToPlane>().selectEntered.AddListener(DisplayLabeling);
        
        _snapToPlaneComponent.hoverEntered.AddListener(ObjectHoveredEntered);
        _snapToPlaneComponent.hoverExited.AddListener(ObjectHoveredExited);
    }
    public void DontShowLabelOnHovering(object sender,EventArgs e)
    {
        this.transform.Find("Front").gameObject.SetActive(false);
        this.GetComponent<SnapToPlane>().selectEntered.RemoveListener(DisplayLabeling);
        
        _snapToPlaneComponent.hoverEntered.RemoveListener(ObjectHoveredEntered);
        _snapToPlaneComponent.hoverExited.RemoveListener(ObjectHoveredExited);
    }
    public void ShowCurrentLabel()
    {
        if (labelUI == null)
        {
            Debug.LogError("Non è possibile chiamare la HideCurrentLabel perchè labelUI è nullo");
            return;
        }

        labelUI.SetActive(true);
    }
    
    
    public void OnSelectionEnter(SelectEnterEventArgs args)
    {
        Debug.Log("hai selezionato l'oggetto: " +this.gameObject.name);
        _currSelectedObj = this.gameObject;
        _characterAnchorManager.DetachFromAnchor();
        if (interactionEnabled)
        {
            simulationManager.SetActiveCharacter(_currSelectedObj);
            interactionEnabled = false;
        }
    }
    public void OnSelectionExit(SelectExitEventArgs args)
    {
        _characterAnchorManager.AttachObjectToAnchor();
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
        label.text = text.ToUpper();
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
            Debug.Log("Etichetta configurata con successo");
        }
        
        if (isPlaneNeeded(text.ToLower())&& boundingBoxplane!=null)
        {
            boundingBoxplane.SetActive(true);
            // spawn del piano ? chiamo l'evento per far ricomparire il tutorial 
            onPlaneSpawned?.Invoke(this,EventArgs.Empty);
            this.transform.Find("Front").gameObject.SetActive(false);
            ControllerManager.OnBoundingBoxPlaneEdit?.Invoke(this,EventArgs.Empty);
        }
        else
        {
            boundingBoxplane.SetActive(false);
        }
        this.transform.Find("Front").gameObject.SetActive(false);
        _snapToPlaneComponent.hoverEntered.AddListener(ObjectHoveredEntered);
        _snapToPlaneComponent.hoverExited.AddListener(ObjectHoveredExited);
        
    }
    

    public void StopShowingLabeling(object sender,EventArgs e)
    {
        this.transform.Find("Front").gameObject.SetActive(false);
        this.GetComponent<SnapToPlane>().selectEntered.RemoveListener(DisplayLabeling);
        //Mostriamo nuovamente L'etichetta'
        _snapToPlaneComponent.hoverEntered.AddListener(ObjectHoveredEntered);
        _snapToPlaneComponent.hoverExited.AddListener(ObjectHoveredExited);
    }
    public void AllowMoving(object  sender,EventArgs obj)
    {
        EnableMoving(this,EventArgs.Empty);
        _snapToPlaneComponent.selectEntered.RemoveListener(DisplayLabeling);
    }    
    
    public void AllowLabeling(object  sender,EventArgs obj)
    { 
      
        DisableMoving(this,EventArgs.Empty);
        _snapToPlaneComponent.selectEntered.AddListener(DisplayLabeling);
        
    }public void OnAllowLabelingEvent(object sender, EventArgs obj)
    {
        _snapToPlaneComponent.selectEntered.AddListener(DisplayLabeling);
    }
    
    public void DisplayLabeling(SelectEnterEventArgs args)
    {
       
        // rimozione dell'evento di hovering
        _snapToPlaneComponent.hoverEntered.RemoveListener(ObjectHoveredEntered);
        _snapToPlaneComponent.hoverExited.RemoveListener(ObjectHoveredExited);
        HideCurrentLabel();
        
        //mostro il menu a tendina 
        var front= this.transform.Find("Front");
        front.gameObject.SetActive(true);
      
      
       
    }
    
    public void DisableMoving(object sender,EventArgs e)
    {
        Debug.Log("Disable Moving La bounding box non è più movibile");
        XRGrabInteractable _xrInt = this._snapToPlaneComponent;
        if (_xrInt != null)
        {
            _xrInt.trackPosition = false;
            _xrInt.trackRotation = false;
            _xrInt.trackScale = false;
        }

        var navmeshObstacle = this.GetComponent<NavMeshObstacle>();
        if (navmeshObstacle != null)
        {
            Debug.Log("componente navmesh obstacle abilitato");
            navmeshObstacle.enabled = false;
        }
        interactionEnabled = true;
    }
    public void EnableMoving(object sender,EventArgs e)
    {
        Debug.Log("Enable Moving La bounding box  è nuovamente movibile");
        XRGrabInteractable _xrInt = this._snapToPlaneComponent;
        if (_xrInt != null)
        {
            _xrInt.trackPosition = true;
            _xrInt.trackRotation = true;
            _xrInt.trackScale = true;
        }

        var navmeshObstacle = this.GetComponent<NavMeshObstacle>();
        if (navmeshObstacle != null)
        {
            navmeshObstacle.enabled = false;
        }
        interactionEnabled = false;
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
        if (label == "chair")
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
