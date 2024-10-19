using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BoundingBoxInteractionManager : MonoBehaviour
{
    public GameObject labelUI;

    public SnapToPlane interactable;

    public SnapToPlane boundingBoxPlane;

    public Vector3 boundingBoxSize=Vector3.zero; // dimensione della bounding box aggiornata 

    public Vector3 initialPlaneLocalPosition; //posizione locale iniziale del piano
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void Awake()
    {
            BoundingBoxManagerUI.OnBoundingBoxPlacementCompleted += AllowLabeling;
            BoundingBoxManagerUI.OnSceneInizializationCompleted += StopShowingLabeling;
            ControllerManager.OnBoundingBoxPlaneEdit += AllowEditPlane;
            Debug.Log("Iscrizione all'evento OnBoundingBoxPlacement completata");
            labelUI = this.transform.Find("BoundingBoxLabel").gameObject;
            interactable = this.GetComponent<SnapToPlane>();
            if (interactable == null || labelUI==null)
            {
                Debug.LogError("errore nel reperimento del componente Snap To Plane o della label o del piano");
            }
            SetLabel("default");
    }
    
    public void SetLabel(string text)
    {
       Text label= labelUI.transform.Find("Scrollview Canvas/Pannello/Viewport/Content/Scroll View Item/Text").GetComponent<Text>();
       label.text = text;
      // this.GetComponent<CharacterManager>().type = text;
    }

    public void Update()
    {
    //    ConstrainPlaneMovement();
    }

    public void StopShowingLabeling(object sender,EventArgs e)
    {
        this.transform.Find("Front").gameObject.SetActive(false);
    }
    

    public void AllowLabeling(object  sender,EventArgs obj)
    { 
        DisableMoving();
        this.GetComponent<SnapToPlane>().selectEntered.AddListener(DisplayLabeling);
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
    
    public void DisplayLabeling(SelectEnterEventArgs args)
    {
       var front= this.transform.Find("Front");
       front.gameObject.SetActive(true);
    }
    public void DisableMoving()
    {
        XRGrabInteractable _xrInt = this.GetComponentInParent<SnapToPlane>();
        if (_xrInt != null)
        {
            _xrInt.trackPosition = false;
            _xrInt.trackRotation = false;
            _xrInt.trackScale = false;
        }
    }

    public void AllowEditPlane(object sender,EventArgs e)
    {
        interactable.transform.Find("BoundingBoxWrapper/Cube").GetComponent<BoxCollider>().enabled = false;
        SetInteractionLayer(LayerMask.NameToLayer("InteractionDisabled"),interactable);
      //  SetInteractionLayer(LayerMask.NameToLayer("Default"),boundingBoxPlane);
        
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
    
 
    
    private void OnRelease(SelectExitEventArgs args)
    {
        // Quando il piano viene rilasciato, assicurati che non sia uscito dai confini
        // ConstrainPlaneMovement();
    }
    
}
