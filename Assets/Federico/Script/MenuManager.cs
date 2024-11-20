using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    
    //SCRIPT 
    public int currentPage = 0;
    public int totalPageNumber=0;
    public List<Texture> imagesTexture = new List<Texture>();
    [SerializeField] private RawImage currentImage;

    [SerializeField] private GameObject previousButton;

    [SerializeField] private GameObject nextButton;
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
       SetSceneObjects();
       //SetTableTop();
      // SelectObject("Woman");
       ResetPageNumber();
       totalPageNumber=imagesTexture.Count;
    }

    public void SetSceneObjects()
    {
        boundingBoxesPanel.SetActive(false);
        boundingBoxesPanelOutline.SetActive(false);
       
        tableTopObjectsPanel.SetActive(false);
        tableTopObjectsPanelOutline.SetActive(false);
      
        sceneObjectPanel.SetActive(true);
        sceneObjectPanelOutline.SetActive(true);
     
        previousButton.SetActive(false);
        nextButton.SetActive(false);
        ResetPageNumber();
    }

    public void SetBoundingBoxes()
    {
        sceneObjectPanel.SetActive(false);
        sceneObjectPanelOutline.SetActive(false);
      
        tableTopObjectsPanel.SetActive(false);
        tableTopObjectsPanelOutline.SetActive(false);
      
        boundingBoxesPanel.SetActive(true);
        boundingBoxesPanelOutline.SetActive(true);
        ResetPageNumber();
        previousButton.SetActive(false);
        nextButton.SetActive(false);
        
    }
    public void SetTableTop()
    {
        
        sceneObjectPanel.SetActive(false);
        sceneObjectPanelOutline.SetActive(false);
       
        boundingBoxesPanel.SetActive(false);
        boundingBoxesPanelOutline.SetActive(false);
      
        tableTopObjectsPanel.SetActive(true);
        tableTopObjectsPanelOutline.SetActive(true);
        ResetPageNumber();
        previousButton.SetActive(true);
        nextButton.SetActive(true);
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
    public void ResetPageNumber()
    {
        currentPage = 0;
        currentImage.texture = imagesTexture[currentPage];
    }
    public void NextPage()
    {
        if(currentPage<totalPageNumber-1)
        {currentPage++;}
        currentImage.texture = imagesTexture[currentPage];
        
        
    }
    public void PreviousPage()
    {
        if (currentPage!=0)
        {
            currentPage--;
        }
        
        currentImage.texture = imagesTexture[currentPage];
    }
}
