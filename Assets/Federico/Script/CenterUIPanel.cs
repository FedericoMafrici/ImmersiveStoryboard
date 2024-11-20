using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.Hands;

public class CenterUIPanel : MonoBehaviour
{
    public Transform userCamera;       // Riferimento alla camera dell'utente (es. Main Camera)
    public float distanceFromUser = 0.1f; // Distanza desiderata dal pannello all'utente
    [SerializeField] private List<GameObject> UIPanels;
    void Start()
    {
        if (userCamera == null)
        {
            // Trova la camera principale se non è stata assegnata
            userCamera = Camera.main.transform;
        }
        
        CenterPanels();
    }

   public void CenterPanels()
{
    // Check if userCamera is null
    if (userCamera == null)
    {
        Debug.LogError("userCamera is null!");
        return;
    }

    // Calculate the desired position
    Vector3 desiredPosition = userCamera.position + userCamera.forward * distanceFromUser;

    // Iterate through UIPanels
    foreach (var obj in UIPanels)
    {
        // Get CharacterAnchorManager component
        var anchorManager = obj.GetComponent<CharacterAnchorManager>();
        if (anchorManager == null)
        {
            Debug.LogError($"CharacterAnchorManager not found on {obj.name}.");
            continue;
        }
        
        #if !UNITY_EDITOR
                Debug.Log("Detaching from anchor and moving the component");
                anchorManager.DetachFromAnchor();
        #endif
        if (obj == null)
        {
            Debug.LogError("An entry in UIPanels is null.");
            continue;
        }

        // Check if obj has at least one child
        if (obj.transform.childCount == 0)
        {
            Debug.LogError($"{obj.name} has no children.");
            continue;
        }

        // Get the first child
        Transform firstChild = obj.transform.GetChild(0);
        Debug.Log("figlio raccolto"+firstChild.name);
        if (firstChild == null)
        {
            Debug.LogError($"First child of {obj.name} is null.");
            continue;
        }

        // Find "TableOffset"
        Transform tableOffset = firstChild.Find("TableOffset");
        if (tableOffset == null)
        {
            Debug.LogError($"'TableOffset' not found under {firstChild.name}.");
            continue;
        }

        // Get TransformSync component
        var handle = tableOffset.GetComponent<TransformSync>();
        if (handle == null)
        {
            Debug.LogError($"TransformSync component not found on {tableOffset.name}.");
            continue;
        }

        handle.enabled = false;

       

        #if !UNITY_EDITOR
        obj.transform.position = desiredPosition;
        anchorManager.AttachObjectToAnchor();
        #else
        obj.transform.position = desiredPosition;
        #endif

        handle.enabled = true;
    }
}

    public void HidePanels()
    {
        foreach (var obj in UIPanels)
        {
           obj.SetActive(false);
        }
    }
    
    public void ShowPanels()
    {
        foreach (var obj in UIPanels)
        {
            obj.SetActive(true);
        }
    }
    
}