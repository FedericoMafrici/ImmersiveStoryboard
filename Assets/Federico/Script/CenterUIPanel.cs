using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.Hands;

public class CenterUIPanel : MonoBehaviour
{
    public Transform userCamera;       // Riferimento alla camera dell'utente (es. Main Camera)
    public float distanceFromUser = 0.1f; // Distanza desiderata dal pannello all'utente
    [SerializeField] private List<GameObject> UIPanels;
    [SerializeField] private List<GameObject> currentHiddenPanels;
    [SerializeField] private List<GameObject> UIhandles;
    [SerializeField] private float panelSpacing = 2.0f; // Distanza tra i pannelli
    [SerializeField] private List<GameObject> notHiddenPanels; // Lista per salvare i pannelli attivi prima di nasconderli
   
    void Start()
    {
        if (userCamera == null)
        {
            // Trova la camera principale se non è stata assegnata
            userCamera = Camera.main.transform;
        }
        
        // Avvia la coroutine che attende l'inizializzazione della camera
        StartCoroutine(WaitForCameraAndCenterPanels());
        BoundingBoxManagerUI.OnShowScriptPanel += ShowScriptPanel;
        SimulationManager.pauseStoryboarding += ShowObjectMenu;
    }

    private void ShowScriptPanel(object sender, EventArgs e)
    {
       GameObject  menuOggetti= ShowSpecificPanel(0);
       menuOggetti.GetComponentInChildren<MenuManager>().SetShowSscript();
    }

    private void ShowObjectMenu(object sender, EventArgs e)
    {
        GameObject  menuOggetti= ShowSpecificPanel(0);
        menuOggetti.GetComponentInChildren<MenuManager>().SetSceneObjects();
    }

    private void OnDisable()
    {
        BoundingBoxManagerUI.OnShowScriptPanel -= ShowScriptPanel;
    }

    public void CenterPanels()
    {
        Debug.Log("Eseguo la CenterPanels");
        // Check if userCamera is null
        if (userCamera == null)
        {
            Debug.LogError("userCamera is null!");
            return;
        }

        // Calculate the desired position in front of the user
        Vector3 desiredPosition = userCamera.position + userCamera.forward * distanceFromUser;
        int index = 0;
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

            // Calculate the horizontal offset for this panel
            Vector3 horizontalOffset = userCamera.right * panelSpacing * index;

            // Calculate the position for this panel
            Vector3 panelPosition = desiredPosition + horizontalOffset;

            Debug.Log($"Nuova posizione per {obj.name}: {panelPosition}");
#if !UNITY_EDITOR
            UIhandles[index].transform.position = panelPosition;
            anchorManager.AttachObjectToAnchor();
#else
            UIhandles[index].transform.position = panelPosition;
#endif
            index++;
//RotationHandler.onChangeOrientation?.Invoke(this, EventArgs.Empty);
        }
    }
   private IEnumerator WaitForCameraAndCenterPanels()
   { 
       yield return new WaitForSeconds(3);
    
       // Ora la camera dovrebbe essere inizializzata, puoi chiamare CenterPanels()
       CenterPanels();
   }
   
   public void HidePanels()
   {
       // Inizializza o svuota la lista dei pannelli non nascosti
       notHiddenPanels = new List<GameObject>();

       foreach (var obj in UIPanels)
       {
           if (obj != null)
           {
               // Se il pannello era attivo, lo aggiungiamo alla lista
               if (obj.activeSelf)
               {
                   notHiddenPanels.Add(obj);
               }
               obj.SetActive(false);
           }
       }
   }
   
    public void ShowPanels()
    {
        foreach (var obj in UIPanels)
        {
            obj.SetActive(true);
        }
    }
    public void AddHiddenPanel(GameObject panel)
    {
        currentHiddenPanels.Add(panel);
        panel.SetActive(false);
    }
    public void ShowHiddenPanels()
    {
        foreach (var panel in currentHiddenPanels)
        {
                panel.SetActive(true);    
        }
        currentHiddenPanels.Clear();
    }

    public void ShowNotHiddenPanels()
    {
        Debug.Log("ShowNotHiddenPanels chiamata");
        foreach (var panel in notHiddenPanels)
        {
            if (panel != null)
            {
                panel.SetActive(true);
            }
        }
        // Svuotiamo la lista dopo aver mostrato i pannelli
        notHiddenPanels.Clear();
    }

    public GameObject ShowSpecificPanel(int index)
    {
        var panel= UIPanels[index];
        panel.SetActive(true);
        return panel;

    }
}

