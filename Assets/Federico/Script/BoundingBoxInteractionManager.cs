using System;
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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
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

          //  SetLabel("chair");
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
        this.GetComponent<CharacterManager>().type = text.ToLower() ;
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
    
    public void AllowEditPlane(object sender,EventArgs e)
    {
        interactable.transform.Find("BoundingBoxWrapper/Cube").GetComponent<BoxCollider>().enabled = false;
        SetInteractionLayer(LayerMask.NameToLayer("InteractionDisabled"),interactable);
      //  SetInteractionLayer(LayerMask.NameToLayer("Default"),boundingBoxPlane);
        
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
