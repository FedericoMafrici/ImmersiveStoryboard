using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BoundingBoxInteractionManager : MonoBehaviour
{
    private GameObject labelUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
            BoundingBoxManagerUI.OnBoundingBoxPlacementCompleted += AllowLabeling;
            BoundingBoxManagerUI.OnSceneInizializationCompleted += StopShowingLabeling;
            Debug.Log("Iscrizione all'evento OnBoundingBoxPlacement completata");
            labelUI = this.transform.Find("BoundingBoxLabel").gameObject;
            if (labelUI==null)
            {
                Debug.LogError("errore nellinserimento della label della bounding box"+ this.name);
            }

            SetLabel("default");
    }
    
    public void SetLabel(string text)
    {
       Text label= labelUI.transform.Find("Scrollview Canvas/Pannello/Viewport/Content/Scroll View Item/Text").GetComponent<Text>();
       label.text = text;
      // this.GetComponent<CharacterManager>().type = text;
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
    
}
