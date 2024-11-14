using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

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
        AttachObjectToAnchor();
    }

    public void AttachObjectToAnchor()
    {
        sceneAnchorManager.AttachObjectToAnchor(this);
    }

    public void DetachFromAnchor()
    {
        if (sceneAnchorManager != null)
        {
            sceneAnchorManager.DetachObjectFromAnchor(this);
        }
    }
}
