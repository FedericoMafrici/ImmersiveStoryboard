using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit;

public class UIAnchorManager : CharacterAnchorManager
{
    [SerializeField] private ARAnchorManager anchorManager;
    [SerializeField] private SceneAnchorManager sceneAnchorManager;
    public ARAnchor currentAnchor;
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
}
