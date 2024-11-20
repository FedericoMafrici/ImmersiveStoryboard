using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit;

public class CharacterAnchorManager : MonoBehaviour
{
    [SerializeField] private ARAnchorManager anchorManager;
    [SerializeField] private SceneAnchorManager sceneAnchorManager;
    public ARAnchor currentAnchor;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anchorManager = FindObjectOfType<ARAnchorManager>();
        sceneAnchorManager = FindObjectOfType<SceneAnchorManager>();
        if (anchorManager == null)
        {
            Debug.LogError("Character Anchor manager di :" + gameObject.name + " non trovato");
        }

        if (sceneAnchorManager == null)
        {
            Debug.LogError("Character Anchor manager di :" + gameObject.name + " non trovato");
        }
        #if !UNITY_EDITOR
        AttachObjectToAnchor();
        #endif
    }

    public void AttachObjectToAnchor()
    {
        if (sceneAnchorManager != null)
        {
            sceneAnchorManager.AttachObjectToAnchor(this);
        }
        else
        {
            Debug.LogError("lo scene anchor manager è nullo");
        }
    }

    public void DetachFromAnchor()
    {
        if (sceneAnchorManager != null)
        {
            sceneAnchorManager.DetachObjectFromAnchor(this);
        }
    }

    public void AttachObjectToAnchorFromSelection(SelectEnterEventArgs args)
    {
        if (sceneAnchorManager != null)
        {
            sceneAnchorManager.AttachObjectToAnchor(this);
        }
    }
    public void DetachObjectToAnchorFromSelection(SelectExitEventArgs args)
    {
        if (sceneAnchorManager != null)
        {
            sceneAnchorManager.DetachObjectFromAnchor(this);
        } 
    }
    
}
