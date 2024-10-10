using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] public GameObject menuUIElement;
    [Header("Manager della simulazione")] 
    [SerializeField] public ControllerManager controllerManager;
    [Header("Element To spawn")] [SerializeField]
    public GameObject[] sceneObjects; 
   
    [Header("Componenti UI del menu ")]
    [SerializeField] public GameObject sceneObjectPanel;
    [SerializeField] public GameObject sceneObjectPanelOutline;
    [SerializeField] public GameObject  boundingBoxesPanel;
    [SerializeField] public GameObject  boundingBoxesPanelOutline;
    [SerializeField] public GameObject tableTopObjectsPanel;
    [SerializeField] public GameObject tableTopObjectsPanelOutline;
    [SerializeField] private List<string> keys = new List<string>();
    [SerializeField] private List<GameObject> values = new List<GameObject>();
    [SerializeField] public Dictionary<string, GameObject> myDictionary = new Dictionary<string, GameObject >();
    
    // Start is called before the first frame update
    void Start()
    {
       /*
        foreach (var obj in sceneObjects)
        {
            GameObject.Instantiate(menuUIElement,this.transform);
            
        }
        */
       // Popola il dizionario dalle liste
       for (int i = 0; i < keys.Count && i < values.Count; i++)
       {
           if (!myDictionary.ContainsKey(keys[i]))
           {
               myDictionary.Add(keys[i], values[i]);
           }
       }
       SetTableTop();
       SelectObject("Woman");
    }

    public void SetSceneObjects()
    {
        boundingBoxesPanel.SetActive(false);
        boundingBoxesPanelOutline.SetActive(false);
       
        tableTopObjectsPanel.SetActive(false);
        tableTopObjectsPanelOutline.SetActive(false);
      
        sceneObjectPanel.SetActive(true);
        sceneObjectPanelOutline.SetActive(true);
    }

    public void SetBoundingBoxes()
    {
        sceneObjectPanel.SetActive(false);
        sceneObjectPanelOutline.SetActive(false);
      
        tableTopObjectsPanel.SetActive(false);
        tableTopObjectsPanelOutline.SetActive(false);
      
        boundingBoxesPanel.SetActive(true);
        boundingBoxesPanelOutline.SetActive(true);
    }

    public void SetTableTop()
    {
        
        sceneObjectPanel.SetActive(false);
        sceneObjectPanelOutline.SetActive(false);
       
        boundingBoxesPanel.SetActive(false);
        boundingBoxesPanelOutline.SetActive(false);
      
        tableTopObjectsPanel.SetActive(true);
        tableTopObjectsPanelOutline.SetActive(true);
    }
    
    public void SelectObject(string obj )
    {
        GameObject prefab;
        if(myDictionary.TryGetValue(obj,out  prefab))
        {
            controllerManager.SetObjectToSpawn(prefab);
        }
        else
        {
            Debug.LogError("oggetto non trovato stringa passata " + obj);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
